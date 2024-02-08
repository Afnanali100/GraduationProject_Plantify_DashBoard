using ControlPanel.DAL.Models;

namespace ControlPanel.PLL.Helper
{
	public interface IEmailSetting
	{
		public  Task SendEmail(Email email);

	}
}
