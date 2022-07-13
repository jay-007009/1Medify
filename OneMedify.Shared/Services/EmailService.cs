using OneMedify.DTO.Response;
using OneMedify.Shared.Contracts;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OneMedify.Shared.Services
{
    public class EmailService : IEmailService
    {
        public async Task<ResponseDto> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                smtpClient.Credentials = new System.Net.NetworkCredential("onemedify@gmail.com", "xcczszmmdeuhoikk");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("onemedify@gmail.com", "One Medify");
                mail.To.Add(new MailAddress(toEmail));
                mail.Subject = subject;
                mail.Body = body;

                smtpClient.Send(mail);

                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500 };
            }
        }
    }
}
