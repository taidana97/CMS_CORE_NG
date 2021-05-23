using FunctionalService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using ModelService;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSvc : IEmailSvc
    {
        private readonly SendGridOptions _sendGridOptions;
        private readonly IFunctionalSvc _functionalSvc;
        private readonly SmtpOptions _smtpOptions;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EmailSvc(
            IOptions<SendGridOptions> sendGridOptions,
            IFunctionalSvc functionalSvc,
            IOptions<SmtpOptions> smtpOptions,
            IWebHostEnvironment hostingEnvironment)
        {
            _sendGridOptions = sendGridOptions.Value;
            _smtpOptions = smtpOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _functionalSvc = functionalSvc;
        }

        public Task SendMailAsync(string email, string subject, string message, string template)
        {
            var messageBody = BuildEmailBody(message, template, subject);
            if (_sendGridOptions.IsDefault)
            {
                _functionalSvc.SendEmailBySendGridAsync(
                    _sendGridOptions.SendGridKey,
                    _sendGridOptions.FromEmail,
                    _sendGridOptions.FromFullName,
                    subject,
                    messageBody,
                    email).Wait();
            }

            if (_smtpOptions.IsDefault == false) return Task.CompletedTask;

            if (string.IsNullOrEmpty(messageBody) == false)
            {
                _functionalSvc.SendEmailByGmailAsync(
                    _smtpOptions.FromEmail,
                    _smtpOptions.FromFullName,
                    subject,
                    messageBody,
                    email,
                    email,
                    _smtpOptions.SmtpUserName,
                    _smtpOptions.SmtpPassword,
                    _smtpOptions.SmtpHost,
                    _smtpOptions.SmtpPort,
                    _smtpOptions.SmtpSsl).Wait();
            }

            return Task.CompletedTask;
        }

        private string BuildEmailBody(string message, string templateName, string subject)
        {
            var messages = string.Empty;
            try
            {
                var templateFilePath = string.Format("{0}{1}{2}",
                    _hostingEnvironment.ContentRootPath,
                    "/EmailTemplates/",
                    templateName);
                var reader = new StreamReader(templateFilePath);
                messages = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            messages = messages.Replace(
                "[[[Title]]]",
                string.IsNullOrEmpty(subject)
                    ? "Notification => techhowdy.com"
                    : subject);
            messages = messages.Replace("[[[message]]]", message);
            return messages;
        }
    }
}
