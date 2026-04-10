using System.ComponentModel.DataAnnotations;

namespace kitucxa.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Số phòng không được để trống")]
        public string RoomNumber { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Sức chứa phải từ 1 đến 20")]
        public int Capacity { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
    }
}