using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailSvc
    {
        Task SendMailAsync(string email, string subject, string message, string template);
    }
}
