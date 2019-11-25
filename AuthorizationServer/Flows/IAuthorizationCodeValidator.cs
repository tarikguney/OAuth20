namespace AuthorizationServer.Flows
{
    public interface IAuthorizationCodeValidator
    {
        bool IsValidAuthorizationCode(string code, string clientId);
    }
}