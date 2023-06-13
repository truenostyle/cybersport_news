using System.Net;
using System.Net.Mail;

namespace ASP_1.Services.Email
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GmailService> _logger;

        public GmailService(IConfiguration configuration, ILogger<GmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool Send(string mailTemplate, string congrMailTemplate, object model)
        {
            String? template = null;
            String? congrTemplate = null;
            String[] filenames = new String[]
            {
                mailTemplate,
                mailTemplate + ".html",
                "Services/Email/" + mailTemplate + ".html"
            };
            String[] filenames2 = new String[]
            {
                congrMailTemplate,
                congrMailTemplate + ".html",
                "Services/Email/" + congrMailTemplate + ".html"
            };
            foreach (String filename in filenames)
            {
                if (System.IO.File.Exists(filename))
                {
                    template = System.IO.File.ReadAllText(filename);
                    break;
                }
            }
            foreach (String filename in filenames2)
            {
                if (System.IO.File.Exists(filename))
                {
                    congrTemplate = System.IO.File.ReadAllText(filename);
                    break;
                }
            }
            if (template is null)
            {
                throw new ArgumentException($"template '{mailTemplate}' not found");
            }
            if (congrTemplate is null)
            {
                throw new ArgumentException($"template '{congrMailTemplate}' not found");
            }
            String? host = _configuration["Smtp:Gmail:Host"];
            if (host is null) throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Host'");
            String? mailbox = _configuration["Smtp:Gmail:Email"];
            if (mailbox is null) throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Email'");
            String? password = _configuration["Smtp:Gmail:Password"];
            if (password is null) throw new MissingFieldException("Missing configuration 'Smtp:Gmail:Password'");
            int port = Convert.ToInt32(_configuration["Smtp:Gmail:Port"]);
            bool ssl = Convert.ToBoolean(_configuration["Smtp:Gmail:Ssl"]);


            String? userEmail = null;
            foreach (var prop in model.GetType().GetProperties())
            {
                
                if (prop.Name == "Email")
                {
                    userEmail = prop.GetValue(model)?.ToString();
                }
                String placeholder = $"{{{{{prop.Name}}}}}";
                if (template.Contains(placeholder))
                {
                    template = template.Replace(placeholder, prop.GetValue(model)?.ToString() ?? "");
                }
                if (congrTemplate.Contains(placeholder))
                {
                    congrTemplate = congrTemplate.Replace(placeholder, prop.GetValue(model)?.ToString() ?? "");
                }
            }
            if(userEmail is null)
            {
                throw new ArgumentException("No 'Email' property is model");
            }


            
            
            using SmtpClient smtpClient = new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
            MailMessage mailMessage1 = new()
            {
                From = new MailAddress(mailbox),
                Subject = "ASP-201 Project",
                IsBodyHtml = true,
                Body = template
            };

            MailMessage mailMessage2 = new()
            {
                From = new MailAddress(mailbox),
                Subject = "ASP-201 Project",
                IsBodyHtml = true,
                Body = congrTemplate
            };
            mailMessage1.To.Add(userEmail);
            mailMessage2.To.Add(userEmail);
            try
            {
                smtpClient.Send(mailMessage1);
                smtpClient.Send(mailMessage2);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Send Email exception {ex}", ex.Message);
                return false;
            }
        }
    }
}
