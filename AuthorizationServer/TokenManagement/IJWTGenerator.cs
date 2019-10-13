namespace AuthorizationServer.TokenManagement
{
    public interface IJWTGenerator
    {
        string GenerateToken(string secret);
    }
}