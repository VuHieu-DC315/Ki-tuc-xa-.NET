using kitucxa.Data;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class RoomPermissionService : IRoomPermissionService
    {
        private readonly AppDbContext _context;

        public RoomPermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanViewRoomAsync(int studentId, int roomId)
        {
            bool canView = await _context.Student
                .AsNoTracking()
                .AnyAsync(s => s.Id == studentId && s.RoomId == roomId);

            return canView;
        }
    }
}