namespace kitucxa.Models
{
    public class StudentDashboardVm
    {
        public Student Student { get; set; } = new Student();

        public Room? MyRoom { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}