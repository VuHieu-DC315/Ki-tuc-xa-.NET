using kitucxa.Data;
using kitucxa.Models;
using kitucxa.Service.Cache;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cacheService;

        private static readonly TimeSpan RoomListCacheDuration = TimeSpan.FromMinutes(10);

        public RoomService(AppDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public List<Room> GetAll()
        {
            return _context.Room
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public async Task<List<RoomListItemDto>> GetAllCachedAsync()
        {
            var cachedRooms = await _cacheService.GetAsync<List<RoomListItemDto>>(CacheKeys.RoomList);

            if (cachedRooms != null)
            {
                return cachedRooms;
            }

            var roomsFromDatabase = await _context.Room
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .Select(r => new RoomListItemDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Capacity = r.Capacity,
                    CurrentStudentCount = r.Students.Count
                })
                .ToListAsync();

            await _cacheService.SetAsync(
                CacheKeys.RoomList,
                roomsFromDatabase,
                RoomListCacheDuration
            );

            return roomsFromDatabase;
        }

        public Room? GetById(int id)
        {
            return _context.Room.Find(id);
        }

        public void Add(Room room)
        {
            _context.Room.Add(room);
            _context.SaveChanges();

            ClearRoomListCache();
        }

        public void Update(Room room)
        {
            var existingRoom = _context.Room.FirstOrDefault(r => r.Id == room.Id);

            if (existingRoom == null)
            {
                throw new InvalidOperationException("Phòng không tồn tại.");
            }

            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.Capacity = room.Capacity;

            _context.SaveChanges();

            ClearRoomListCache();
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

                ClearRoomListCache();
            }
        }

        public Room? GetRoomWithStudentsById(int id)
        {
            return _context.Room
                .AsNoTracking()
                .Include(r => r.Students)
                .FirstOrDefault(r => r.Id == id);
        }

        private void ClearRoomListCache()
        {
            _cacheService.RemoveAsync(CacheKeys.RoomList).GetAwaiter().GetResult();
        }
    }
}