using kitucxa.Data;
using kitucxa.Models;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class Report_RoomService : IReport_RoomService
    {
        private readonly AppDbContext _context;

        public Report_RoomService(AppDbContext context)
        {
            _context = context;
        }

        public void SendReport_Room(Report_Room reportRoom)
        {
            _context.ReportRooms.Add(reportRoom);
            _context.SaveChanges();
        }

        public List<Report_Room> GetAll()
        {
            return _context.ReportRooms
                .Include(r => r.Student)
                .Include(r => r.OldRoom)
                .Include(r => r.NewRoom)
                .ToList();
        }

        public void UpdateStatus(int id, string status)
        {
            var reportRoom = _context.ReportRooms.Find(id);

            if (reportRoom == null)
            {
                throw new Exception("Report_Room not found");
            }

            reportRoom.Status = status;

            if (status == "Confirm")
            {
                var student = _context.Student.Find(reportRoom.StudentId);

                if (student == null)
                {
                    throw new Exception("Student not found");
                }

                if (reportRoom.NewRoomId == null)
                {
                    throw new Exception("New room is required");
                }

                var newRoom = _context.Room.Find(reportRoom.NewRoomId.Value);

                if (newRoom == null)
                {
                    throw new Exception("New room not found");
                }

                int currentStudentCount = _context.Student
                    .Count(s => s.RoomId == reportRoom.NewRoomId.Value);

                if (currentStudentCount >= newRoom.Capacity)
                {
                    throw new Exception("Phòng đã đầy, không thể xác nhận yêu cầu.");
                }

                var studentRoomHistory = new StudentRoomHistory
                {
                    StudentId = reportRoom.StudentId,
                    OldRoomId = reportRoom.OldRoomId,
                    NewRoomId = reportRoom.NewRoomId,
                    ActionType = reportRoom.OldRoomId == null ? "RegisterRoom" : "ChangeRoom",
                    CreatedAt = DateTime.Now
                };

                student.RoomId = reportRoom.NewRoomId.Value;

                _context.Student.Update(student);
                _context.StudentRoomHistories.Add(studentRoomHistory);
            }

            _context.SaveChanges();
        }
    }
}