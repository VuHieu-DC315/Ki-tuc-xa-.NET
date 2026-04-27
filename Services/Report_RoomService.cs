using kitucxa.Data;
using kitucxa.Models;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class Report_RoomService : IReport_RoomService
    {
        private readonly AppDbContext _context;

        public Report_RoomService(AppDbContext context)
        {
            _context = context;
        }

        public void SendReport_Room(Report_Room reportRoom)
        {
            try
            {
                if (reportRoom == null)
                {
                    throw new InvalidOperationException("Dữ liệu yêu cầu không hợp lệ.");
                }

                if (string.IsNullOrEmpty(reportRoom.Status))
                {
                    reportRoom.Status = "Pending";
                }

                _context.ReportRooms.Add(reportRoom);
                _context.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Không thể lưu yêu cầu đổi phòng vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi khi gửi yêu cầu đổi phòng.", ex);
            }
        }

        public List<Report_Room> GetAll()
        {
            try
            {
                return _context.ReportRooms
                    .Include(r => r.Student)
                    .Include(r => r.OldRoom)
                    .Include(r => r.NewRoom)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách yêu cầu đổi phòng.", ex);
            }
        }

        public void UpdateStatus(int id, string status)
        {
            try
            {
                if (status != "Confirm" && status != "Reject")
                {
                    throw new InvalidOperationException("Trạng thái không hợp lệ.");
                }

                var reportRoom = _context.ReportRooms.Find(id);

                if (reportRoom == null)
                {
                    throw new InvalidOperationException("Không tìm thấy yêu cầu đổi phòng.");
                }

                if (reportRoom.Status != "Pending")
                {
                    throw new InvalidOperationException("Yêu cầu này đã được xử lý rồi.");
                }

                if (status == "Confirm")
                {
                    var student = _context.Student.Find(reportRoom.StudentId);

                    if (student == null)
                    {
                        throw new InvalidOperationException("Không tìm thấy sinh viên.");
                    }

                    if (reportRoom.NewRoomId == null)
                    {
                        throw new InvalidOperationException("Phòng mới không được để trống.");
                    }

                    var newRoom = _context.Room.Find(reportRoom.NewRoomId.Value);

                    if (newRoom == null)
                    {
                        throw new InvalidOperationException("Không tìm thấy phòng mới.");
                    }

                    int currentStudentCount = _context.Student
                        .Count(s => s.RoomId == reportRoom.NewRoomId.Value);

                    if (currentStudentCount >= newRoom.Capacity)
                    {
                        throw new InvalidOperationException("Phòng đã đầy, không thể xác nhận yêu cầu.");
                    }

                    var studentRoomHistory = new StudentRoomHistory
                    {
                        StudentId = reportRoom.StudentId,
                        OldRoomId = reportRoom.OldRoomId,
                        NewRoomId = reportRoom.NewRoomId,
                        ActionType = reportRoom.OldRoomId == null ? "RegisterRoom" : "ChangeRoom",
                        CreatedAt = DateTime.Now
                    };

                    student.RoomId = reportRoom.NewRoomId.Value;

                    _context.Student.Update(student);
                    _context.StudentRoomHistories.Add(studentRoomHistory);
                }

                reportRoom.Status = status;

                _context.ReportRooms.Update(reportRoom);
                _context.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Không thể cập nhật trạng thái yêu cầu đổi phòng.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi khi cập nhật trạng thái yêu cầu đổi phòng.", ex);
            }
        }
    }
}