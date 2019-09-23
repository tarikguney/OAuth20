namespace AuthorizationServer.IdentityManagement
{
    public interface IClientManager
    {
        bool ValidateClientCredentials(string clientId, string clientSecret);
    }
}