namespace Mag.Interfaces
{
    public interface IUserRegistrationService
    {
        bool RegisterUser(string userName);
        bool ConfirmUser(string userName, string code);
    }
}