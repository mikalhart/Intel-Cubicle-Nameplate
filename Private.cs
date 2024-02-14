using Microsoft.Kiota.Abstractions.Authentication;

namespace Nameplate
{
    partial class TeamsStatus
    {
        partial class TokenProvider : IAccessTokenProvider
        {
            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            string tenantId = "46c98d88-e344-4ed4-8496-4ed7712e255d";// "common";

            // Value from app registration
            string clientId = "62c3145e-4fcb-4d1f-9ed3-30179db63c7a";// "YOUR_CLIENT_ID";
        }
    }
}
