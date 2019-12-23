using AuthorizationServer.Models;

namespace AuthorizationServer.IdentityManagement
{
    internal class MockClientGrantManager : IClientGrantManager
    {
        public bool ClientHasGrantType(string clientId, GrantType grantType)
        {
            return clientId == "tarik";
        }
    }
}