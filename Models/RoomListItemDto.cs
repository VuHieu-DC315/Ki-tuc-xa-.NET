namespace kitucxa.Models
{
    public class RoomListItemDto
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public int CurrentStudentCount { get; set; }

        public int AvailableSlots => Capacity - CurrentStudentCount;
    }
}