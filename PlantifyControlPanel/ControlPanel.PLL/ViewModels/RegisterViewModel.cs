using System.ComponentModel.DataAnnotations;

namespace ControlPanel.PLL.ViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage ="Username is Required!")]
        [MaxLength(20)]
        [MinLength(4)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage ="Invalid Email!")]
        
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Password Is Required")]
        public string Password { get; set; }


        [Required(ErrorMessage ="Please Check The Box ! ")]
        public bool IsAgree { get; set; }



    }
}
