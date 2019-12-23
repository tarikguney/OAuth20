using AuthorizationServer.Models;

namespace AuthorizationServer.IdentityManagement
{
    public interface IClientGrantManager
    {
        bool ClientHasGrantType(string clientId, GrantType grantType);
    }
}