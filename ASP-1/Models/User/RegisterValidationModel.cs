namespace ASP_1.Models.User
{
    public class RegisterValidationModel
    {
        public String LoginResult { get; set; } = null!;
        public String PasswordResult { get; set; } = null!;
        public String RepeatPasswordResult { get; set; } = null!;
        public String EmailResult { get; set; } = null!;
        public String RealNameResult { get; set; } = null!;
        public String IsAgreeResult { get; set; } = null!;
        public String AvatarResult { get; set; } = null!;
    }
}
