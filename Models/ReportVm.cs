namespace kitucxa.Models
{
    public class ReportVm
    {
        public int TotalRegisteredStudents { get; set; }

        public int TotalJoinedRoomStudents { get; set; }

        public int TotalLeftOrTransferredStudents { get; set; }

        public string FilterType { get; set; } = "";

        public DateTime? SelectedDate { get; set; }
    }
}   