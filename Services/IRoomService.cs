using kitucxa.Models;

namespace kitucxa.Service
{
    public interface IRoomService
    {
        List<Room> GetAll();
        Room? GetById(int id);
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
        Room? GetRoomWithStudentsById(int id);
    }
}