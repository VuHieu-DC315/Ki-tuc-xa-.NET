using kitucxa.Models;

namespace kitucxa.Service.Cache
{
    public static class CacheKeys
    {
        // Cache danh sách phòng
        public const string RoomList = "rooms:list:v1";
        // Cache theo từng trang cho Student
        public static string StudentPage(int page, int pageSize)
            => $"students:page:{page}:size:{pageSize}";
        // Cache theo từng Id sinh viên
        public static string StudentById(int StudentId)
            => $"student:{StudentId}";
        
    }
}