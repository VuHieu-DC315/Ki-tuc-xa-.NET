using kitucxa.Data;
using kitucxa.Models;
using kitucxa.Service.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace kitucxa.Service
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cacheService;

        public StudentService(AppDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<List<StudentListItemDto>> GetAllStudent(int page, int pageSize)
        {
            string key = CacheKeys.StudentPage(page, pageSize);

            var students = await _cacheService.GetAsync<List<StudentListItemDto>>(key);
            if (students == null)
            {
                students = await _context.Student
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentListItemDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    PhoneNumber = s.PhoneNumber,
                    RoomNumber = s.Room != null ? s.Room.RoomNumber : ""
                })
                .ToListAsync();
                await _cacheService.SetAsync(key, students, TimeSpan.FromMinutes(5));
            }

            return students;
        }

        public async Task<Student?> GetById(int id)
        {
            string key = CacheKeys.StudentById(id);

            var student = await _cacheService.GetAsync<Student>(key);
            if(student == null)
            {
                student = await _context.Student
                    .Include(s => s.Room)
                    .FirstOrDefaultAsync(s => s.Id == id);
                await _cacheService.SetAsync(key, student, TimeSpan.FromMinutes(5));
            }
            return student;
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