using ASP_1.Models.User;

namespace ASP_1.Services.Random
{
    public interface IRandomService
    {
        public string ConfirmCode(int length);
        public string RandomString(int length);
        public string RandomAvatar(RegistrationModel registrationModel, string ext);
    }
}
