namespace AuthorizationServer.TokenManagement
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(string secret);
    }
}