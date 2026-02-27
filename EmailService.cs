using System.Net;
using System.Net.Mail;

namespace Raven_Family.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendConfirmationEmailAsync(string email, string token, string baseUrl)
        {
            try
            {
                var confirmUrl = $"{baseUrl}confirm-email?token={token}";
                var subject = "🐦 Підтвердіть ваш email - Сім'я Воронів";
                var htmlMessage = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2 style='color: #c41e3a;'>🐦 Добро пожалувати до Сім'ї Воронів!</h2>
                        <p>Дякуємо за реєстрацію. Будь ласка, підтвердіть ваш email, натиснувши на посилання нижче:</p>
                        <div style='margin: 30px 0;'>
                            <a href='{confirmUrl}' style='background-color: #c41e3a; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                Підтвердити email
                            </a>
                        </div>
                        <p style='color: #666;'>Або скопіюйте це посилання:</p>
                        <p style='color: #666; word-break: break-all;'>{confirmUrl}</p>
                        <p style='color: #666; font-size: 12px;'>Це посилання дійсне протягом 24 годин.</p>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        <p style='color: #999; font-size: 12px;'>Якщо ви не реєструвалися, ігноруйте цей лист.</p>
                    </div>
                ";

                return await SendEmailAsync(email, subject, htmlMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при відправці email: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                var smtpServerConfig = _configuration["Smtp:Server"];
                var smtpServer = smtpServerConfig ?? "smtp.gmail.com";
                var smtpPortConfig = _configuration["Smtp:Port"];
                var smtpPort = int.Parse(smtpPortConfig ?? "587");
                var senderEmail = _configuration["Smtp:From"] ?? "";
                var senderPassword = _configuration["Smtp:Password"] ?? "";

                // Перевіряємо чи налаштовані дані
                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    _logger.LogWarning("SMTP не налаштований. Перевірте appsettings.json");
                    return false;
                }

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    client.Timeout = 10000; // 10 секунд таймаут

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email успішно відправлено на {to}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка SMTP: {ex.Message}");
                return false;
            }
        }
    }
}
