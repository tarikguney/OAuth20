namespace AuthorizationServer.UserManagement
{
    public class MockUserCredentialValidator: IUserCredentialValidator
    {
        public bool ValidateCredentials(string username, string password)
        {
            return username == "tarik" && password == "guney";
        }
    }
}