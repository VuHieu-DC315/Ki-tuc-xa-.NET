using System.ComponentModel.DataAnnotations;

namespace kitucxa.Models
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = "";
    }
}