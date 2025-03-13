using Contracts.Services;
using Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services
{
    public class SmtpEmailService : ISMTPEmailService
    {
        private readonly ILogger _logger;
        private readonly SMTPEmailSettings _settings;
        private readonly SmtpClient _smtpClient;

        public SmtpEmailService(ILogger logger, SMTPEmailSettings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _smtpClient = new SmtpClient();
        }

        public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body,
                }.ToMessageBody()
            };

            if (request.ToAddresses.Any())
            {
                foreach (var ToAddress in request.ToAddresses)
                {
                    emailMessage.To.Add(MailboxAddress.Parse(ToAddress));
                }
            }
            else
            {
                var toAdddress = request.ToAddress;
                emailMessage.To.Add(MailboxAddress.Parse(toAdddress));
            }

            try
            {
                await _smtpClient.ConnectAsync(_settings.SMTPServer, _settings.Port, _settings.UseSsl, cancellationToken);
                await _smtpClient.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
                await _smtpClient.SendAsync(emailMessage, cancellationToken);
                await _smtpClient.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                await _smtpClient.DisconnectAsync(true, cancellationToken);
                _smtpClient.Dispose();
            }
        }
    }
}
