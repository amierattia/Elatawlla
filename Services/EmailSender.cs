using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NetWork.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var fromMail = "eng.amier@mail.ru";
        var fromPassword = "4aYBU7KepUq2JgduUEJ1";

        var message = new MailMessage
        {
            From = new MailAddress(fromMail),
            Subject = subject,
            Body = $"<html><body>{htmlMessage}</body></html>",
            IsBodyHtml = true
        };

        message.To.Add(email);

        var smtpClient = new SmtpClient("smtp.mail.ru")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(message);
    }
}