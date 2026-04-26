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
            }

            _context.Student.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.Student.Find(id);
            if (student != null)
            {
                _context.Student.Remove(student);
                _context.SaveChanges();
            }
        }
    }
}