namespace AuthorizationServer.TokenManagement
{
    public interface IAuthorizationCodeGenerator
    {
        string Generate(string clientId);
    }
}