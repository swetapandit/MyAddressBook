using System;
using System.Net;
using System.Net.Mail;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
	public class EmailSender : IEmailSender
    {
        private readonly string _email = ""; 
        private readonly string _password = "";
        public EmailSender()
		{
		}
        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587) 
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_email, _password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                return client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g., logging it
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }

    }
}

