using kitucxa.Models;

namespace kitucxa.Service
{
    public interface IReport_RoomService
    {
        void SendReport_Room(Report_Room reportRoom);

        List<Report_Room> GetAll();
        void UpdateStatus(int id, string status);
    }
}