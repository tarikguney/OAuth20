using AuthorizationServer.Controllers;

namespace AuthorizationServer.IdentityManagement
{
    public interface IClientManager
    {
        bool ValidateClientCredentials(string clientId, string clientSecret);
        bool IsValidClient(string clientId);
    }
}