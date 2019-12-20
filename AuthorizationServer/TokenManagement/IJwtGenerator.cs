namespace AuthorizationServer.TokenManagement
{
    public interface IJwtGenerator
    {
        string GenerateToken(string secret);
    }
}