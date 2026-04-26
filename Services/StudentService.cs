using kitucxa.Data;
using kitucxa.Models;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAll()
        {
            return _context.Student
                .Include(s => s.Room)
                .ToList();
        }

        public Student? GetById(int id)
        {
            return _context.Student
                .Include(s => s.Room)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Add(Student student)
        {
            var room = _context.Room.Find(student.RoomId);

            if (room == null)
            {
                throw new InvalidOperationException("Phòng không tồn tại.");
            }

            int currentStudentCount = _context.Student.Count(s => s.RoomId == student.RoomId);

            if (currentStudentCount >= room.Capacity)
            {
                throw new InvalidOperationException("Phòng đã đầy, không thể thêm sinh viên.");
            }

            _context.Student.Add(student);
            _context.SaveChanges();

            AddRoomHistory(student.Id, null, student.RoomId, "JoinRoom");

            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            var existingStudent = _context.Student
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent == null)
            {
                throw new InvalidOperationException("Sinh viên không tồn tại.");
            }

            var room = _context.Room.Find(student.RoomId);

            if (room == null)
            {
                throw new InvalidOperationException("Phòng không tồn tại.");
            }

            if (existingStudent.RoomId != student.RoomId)
            {
                int currentStudentCount = _context.Student.Count(s => s.RoomId == student.RoomId);

                if (currentStudentCount >= room.Capacity)
                {
                    throw new InvalidOperationException("Phòng đã đầy, không thể chuyển sinh viên vào phòng này.");
                }

                AddRoomHistory(student.Id, existingStudent.RoomId, student.RoomId, "TransferRoom");
            }

            _context.Student.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.Student.Find(id);

            if (student != null)
            {
                AddRoomHistory(student.Id, student.RoomId, null, "LeaveRoom");

                _context.Student.Remove(student);
                _context.SaveChanges();
            }
        }

        
        private void AddRoomHistory(int studentId, int? oldRoomId, int? newRoomId, string actionType)
        {
            var history = new StudentRoomHistory
            {
                StudentId = studentId,
                OldRoomId = oldRoomId,
                NewRoomId = newRoomId,
                ActionType = actionType,
                CreatedAt = DateTime.Now
            };

            _context.StudentRoomHistories.Add(history);
        }
        
        // thao tác của sinh viên
        public StudentDashboardVm? GetDashboard(int studentId)
        {
            var student = _context.Student
                .Include(s => s.Room)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                return null;
            }

            var rooms = _context.Room
                .Include(r => r.Students)
                .ToList();

            var model = new StudentDashboardVm
            {
                Student = student,
                MyRoom = student.Room,
                Rooms = rooms
            };

            return model;
        }
    }
}