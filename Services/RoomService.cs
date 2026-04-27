using kitucxa.Data;
using kitucxa.Models;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> GetAll()
        {
            return _context.Room.ToList();
        }

        public Room? GetById(int id)
        {
            return _context.Room.Find(id);
        }

        public void Add(Room room)
        {
            _context.Room.Add(room);
            _context.SaveChanges();
        }

        public void Update(Room room)
        {
            _context.Room.Update(room);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            bool hasStudents = _context.Student.Any(s => s.RoomId == id);

            if (hasStudents)
            {
                throw new InvalidOperationException("Không thể xóa phòng vì phòng đang có sinh viên.");
            }

            var room = _context.Room.Find(id);
            if (room != null)
            {
                _context.Room.Remove(room);
                _context.SaveChanges();
            }
        }

        public Room? GetRoomWithStudentsById(int id)
        {
            var room = _context.Room
                .Include(r => r.Students)
                .FirstOrDefault(r => r.Id == id);

            return room;
        }
    }
}