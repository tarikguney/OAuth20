namespace AuthorizationServer.IdentityManagement
{
    public interface IClientGrantManager
    {
        bool ClientHasGrantType(string clientId);
    }
}