namespace AuthorizationServer.UserManagement
{
    public interface IUserCredentialValidator
    {
        bool ValidateCredentials(string username, string password);
    }
}