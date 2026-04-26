using System.ComponentModel.DataAnnotations;

namespace kitucxa.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

        public int? StudentId { get; set; }

        public Student? Student { get; set; }
    }
}