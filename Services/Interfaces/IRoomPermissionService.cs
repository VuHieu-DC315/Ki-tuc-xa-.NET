namespace kitucxa.Service
{
    public interface IRoomPermissionService
    {
        Task<bool> CanViewRoomAsync(int userId, int roomId);
    }
}