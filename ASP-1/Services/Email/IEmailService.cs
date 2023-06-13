namespace ASP_1.Services.Email
{
    public interface IEmailService
    {
        bool Send(String mailTemplate, String congrMailTemplate, object model);
    }
}
