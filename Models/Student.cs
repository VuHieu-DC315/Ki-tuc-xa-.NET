using System.ComponentModel.DataAnnotations;

namespace kitucxa.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; } 

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; } = string.Empty;

        public int RoomId { get; set; }

        public Room? Room { get; set; }
    }
}