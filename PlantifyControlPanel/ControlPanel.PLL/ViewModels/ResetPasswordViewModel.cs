using System.ComponentModel.DataAnnotations;

namespace ControlPanel.PLL.ViewModels
{
	public class ResetPasswordViewModel
	{
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password Is Required")]
		public string NewPassword { get; set; }
	}
}
