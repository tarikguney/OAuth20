namespace AuthorizationServer.IdentityManagement
{
    internal class MockClientGrantManager : IClientGrantManager
    {
        public bool ClientHasGrantType(string clientId)
        {
            return clientId == "tarik";
        }
    }
}