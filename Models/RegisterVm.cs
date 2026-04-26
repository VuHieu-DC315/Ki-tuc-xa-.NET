using System.ComponentModel.DataAnnotations;

namespace kitucxa.Models
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string Name {get; set; } = "";
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email {get; set;} = "";
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password {get; set;} = "";
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword {get; set;} = "";
    }
}