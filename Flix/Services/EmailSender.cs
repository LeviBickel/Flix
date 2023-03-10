using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Flix.Services
{
    public class EmailSender : IEmailSender
    {
        public readonly IConfiguration _configuration;
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var response = await Execute(Options.SendGridKey, subject, message, email);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // log or throw
                throw new Exception("Could not send email: " + await response.Body.ReadAsStringAsync());
            }
        }

        private async Task<Response> Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(_configuration["Email:SendGrid:APIToken"]);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("bickelsolutions@gmail.com", "Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return await client.SendEmailAsync(msg);
        }
    }
}
