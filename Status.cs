using System;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Nameplate
{
    enum ContactAvailability { Offline, Away, TemporarilyAway, Free, FreeIdle, Busy, BusyIdle, DoNotDisturb, Invalid, None };

    partial class TeamsStatus
    {
        partial class TokenProvider : IAccessTokenProvider
        {
            AuthenticationResult lastToken = null;
            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            // string tenantId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; // defined in Private.cs

            // Value from app registration
            // string clientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; // defined in Private.cs

            public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = default,
                CancellationToken cancellationToken = default)
            {
                if (lastToken != null && lastToken.ExpiresOn > DateTimeOffset.UtcNow)
                    return lastToken.AccessToken;

                var pca = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithTenantId(tenantId)
                    .Build();
                var scopes = new[] { /* "User.Read", */ "Presence.Read" };
                Debug.WriteLine("Trying to acquire new auth token");
                lastToken = await pca.AcquireTokenByIntegratedWindowsAuth(scopes).ExecuteAsync(cancellationToken);
                Debug.WriteLine($"{DateTime.Now}: NEW TOKEN ACQUIRED");
                // get the token and return it in your own way
                return lastToken?.AccessToken;
            }

            public AllowedHostsValidator AllowedHostsValidator { get; }
        }

        // From Intel IT:
        // We recommend the following:
        // 1) Ensure your application is properly registered at http://iap.intel.com. This is a requirement for step 2
        // 2) Visit http://goto.intel.com/engagesso and follow the application onboarding process. As part of that engagement you can specify that you need the presence.read permission for your application.
        // 'Nameplate' is registered as app #33900 (https://iap.intel.com/#/app/33900)

        #region public interface

        public TeamsStatus()
        {
            WorkerThread = new Thread(ThreadMethod);
        }

        ~TeamsStatus()
        {
            WorkerThread.Abort();
        }

        readonly Thread WorkerThread;
        readonly object lockObject = new object();
        class QueryResults
        {
            public string Availability;
            public string Activity;
            public string Name;
        };
        QueryResults saveResults; // Guarded by lock

        /// <summary>
        /// Using the special token provider TokenProvider, repeatedly (every second) try to get the user's Presence status
        /// There are a number of ways this can fail, some of which require stepping back the query rate.  In these cases,
        /// "needBackOff" will be set to true, and the next query waits 30 seconds instead of only 1
        /// </summary>

        private async void ThreadMethod()
        {

            IAuthenticationProvider authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider());
            GraphServiceClient graphClient = new GraphServiceClient(authenticationProvider);

            while (true)
            {
                QueryResults results = new QueryResults();
                bool needBackOff = false;
                try
                {
                    Presence presence = await graphClient.Me.Presence.GetAsync();
                    // User me = await graphClient.Me.GetAsync();
                    results.Availability = presence.Availability;
                    results.Activity = presence.Activity;
                    results.Name = ""; // me.DisplayName;
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    // In transition to/from VPN
                    Debug.WriteLine("{DateTime.Now:T} HttpRequestException");
                    results.Availability = results.Activity = results.Name = "Unknown";
                }
                catch (MsalUiRequiredException ex)
                {
                    Debug.WriteLine($"{DateTime.Now:T} MsalUiRequireException: {ex.Message}");
                    results.Availability = results.Activity = results.Name = "Unknown";
                    if (ex.Classification == UiRequiredExceptionClassification.BasicAction)
                    {
                        Debug.WriteLine("No VPN");
                        results.Activity = "Log into VPN";
                    }
                    else if (ex.Classification == UiRequiredExceptionClassification.None)
                    {
                        Debug.WriteLine($"{DateTime.Now:T} Throttling: need back off");
                        needBackOff = true;
                    }
                }
                catch (MsalClientException ex)
                {
                    Debug.WriteLine($"{DateTime.Now:T} MsalClientException: {ex.Message}");
                    results.Availability = results.Activity = results.Name = "Unknown";
                    Debug.WriteLine("No VPN");
                    results.Activity = "Log into VPN";
                    Debug.WriteLine($"{DateTime.Now:T} Throttling: need back off");
                    needBackOff = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now:T} An unknown error occurred trying to get Presence: {ex.GetType()}: {ex.Message}");
                    results.Availability = results.Activity = results.Name = "Unknown";
                }

                lock(lockObject) { saveResults = results; }

                Thread.Sleep(needBackOff ? 30000 : 1000);
            }
        }

        public void Begin()
        {
            WorkerThread.Start();
        }

        public bool QueryState(out ContactAvailability availability, out string activity, out string name)
        {
            QueryResults results;
            lock (lockObject)
            {
                results = saveResults;
            }

            switch (results?.Availability)
            {
                default:
                    availability = ContactAvailability.Invalid;
                    break;
                case "Available":
                    availability = ContactAvailability.Free;
                    break;
                case "AvailableIdle":
                    availability = ContactAvailability.FreeIdle;
                    break;
                case "Away":
                    availability = ContactAvailability.Away;
                    break;
                case "BeRightBack":
                    availability = ContactAvailability.TemporarilyAway;
                    break;
                case "Busy":
                    availability = ContactAvailability.Busy;
                    break;
                case "BusyIdle":
                    availability = ContactAvailability.BusyIdle;
                    break;
                case "DoNotDisturb":
                    availability = ContactAvailability.DoNotDisturb;
                    break;
                case "Offline":
                    availability = ContactAvailability.Offline;
                    break;
                case "PresenceUnknown":
                    availability = ContactAvailability.None;
                    break;
            }


            // Available, Away, BeRightBack, Busy, DoNotDisturb, InACall, InAConferenceCall, Inactive,
            // InAMeeting, Offline, OffWork, OutOfOffice, PresenceUnknown, Presenting, UrgentInterruptionsOnly.
            switch (results?.Activity)
            {
                case "BeRightBack":
                    activity = "Be Right Back";
                    break;
                case "DoNotDisturb":
                    activity = "Do Not Disturb";
                    break;
                case "InACall":
                    activity = "In a Call";
                    break;
                case "InAConferenceCall":
                    activity = "In a Conference Call";
                    break;
                case "InAMeeting":
                    activity = "In a Meeting";
                    break;
                case "OffWork":
                    activity = "Off Work";
                    break;
                case "OutOfOffice":
                    activity = "Out of Office";
                    break;
                case "PresenceUnknown":
                    activity = "Presence Unknown";
                    break;
                case "UrgentInterruptionsOnly":
                    activity = "Urgent Interruptions Only";
                    break;
                default:
                    activity = results?.Activity;
                    break;
            }
            name = results?.Name ?? "Unknown";
            return true;
        }
#endregion
    }
}
