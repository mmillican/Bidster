using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Bidster.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Bidster.Services
{
    public class AmazonSesEmailSender : IEmailSender
    {
        private readonly IAmazonSimpleEmailService _sesService;
        private readonly EmailConfig _emailConfig;

        public AmazonSesEmailSender(IAmazonSimpleEmailService sesService,
            IOptions<EmailConfig> emailConfig)
        {
            _sesService = sesService;
            _emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var sendRequest = new SendEmailRequest
            {
                Source = _emailConfig.FromAddress,
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content(message)
                    }
                },
                Destination = new Destination
                {
                    ToAddresses = new List<string> { email }
                }
            };

            await _sesService.SendEmailAsync(sendRequest);
        }
    }
}