using System.ComponentModel.DataAnnotations;

namespace ControlPanel.PLL.ViewModels
{
    public class ForgetPasswordViewModel
    {

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Invalid Email!")]
        public string Email { get; set; }


    }
}
