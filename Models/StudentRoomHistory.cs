namespace kitucxa.Models
{
    public class StudentRoomHistory
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int? OldRoomId { get; set; }
        public Room? OldRoom { get; set; }

        public int? NewRoomId { get; set; }
        public Room? NewRoom { get; set; }

        public string ActionType { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}