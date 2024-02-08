using ControlPanel.DAL.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ControlPanel.PLL.Helper
{
	public class EmailSetting : IEmailSetting
	{
		private  MailSetting option;

		public EmailSetting(IOptions<MailSetting> option)
        {
			this.option = option.Value;
		}


        public async Task SendEmail(Email email)
		{
			//sender
			var mail = new MimeMessage
			{
				Sender=MailboxAddress.Parse(option.Email),
				Subject=email.Subject
			};

			//To
			mail.To.Add(MailboxAddress.Parse(email.To));

			//body
			var builder=new BodyBuilder();
			builder.TextBody=email.Body;
		    mail.Body=	builder.ToMessageBody();

			mail.From.Add(new MailboxAddress(option.DisplayName, option.Email));

         //open connection
		 var smtp=new MailKit.Net.Smtp.SmtpClient();
		await smtp.ConnectAsync(option.Host, option.Port, SecureSocketOptions.StartTls);
				smtp.Authenticate(option.Email,option.Password);
			smtp.Send(mail);
			await smtp.DisconnectAsync(true);



		}
	}
}
