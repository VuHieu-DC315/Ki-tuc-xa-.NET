namespace kitucxa.Models
{
    public class StudentListItemDto
    {
        public int Id {get; set;}
        public string FullName {get; set;}
        public string PhoneNumber{get; set;}

        public string RoomNumber { get; set; } = string.Empty;
    }
}

