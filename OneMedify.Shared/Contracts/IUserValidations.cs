namespace OneMedify.Shared.Contracts
{
    public interface IUserValidations
    {
        bool IsEmailAlreadyExists(string email);
        bool IsMobileNumberAlreadyExists(string mobileNumber);
        bool IsMobileNumberValid(string mobilenumber);
        bool IsEmailValid(string email);

        
    }
}
