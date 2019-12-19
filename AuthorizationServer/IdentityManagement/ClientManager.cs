using AuthorizationServer.Controllers;
using AuthorizationServer.Models;

namespace AuthorizationServer.IdentityManagement
{
    class ClientManager : IClientManager
    {
        public bool ValidateClientCredentials(string clientId, string clientSecret)
        {
            return clientId == "tarik" && clientSecret == "guney";
        }

        public bool IsValidClient(string clientId)
        {
            return clientId == "tarik";
        }

        public bool AllowedToUseGrantType(string clientId, GrantType grantType)
        {
            return true;
        }
    }
}