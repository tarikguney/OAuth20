namespace AuthorizationServer.IdentityManagement
{
    internal class MockClientManager : IClientManager
    {
        public bool ValidateClientCredentials(string clientId, string clientSecret)
        {
            return clientId == "tarik" && clientSecret == "guney";
        }

        public bool IsValidClient(string clientId)
        {
            return clientId == "tarik";
        }
    }
}