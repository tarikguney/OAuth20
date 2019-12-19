using AuthorizationServer.Controllers;
using AuthorizationServer.Models;

namespace AuthorizationServer.IdentityManagement
{
    public interface IClientManager
    {
        bool ValidateClientCredentials(string clientId, string clientSecret);
        bool IsValidClient(string clientId);
        bool AllowedToUseGrantType(string clientId, GrantType grantType);
    }
}