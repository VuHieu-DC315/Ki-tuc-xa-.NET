using kitucxa.Data;
using kitucxa.Models;
using Microsoft.EntityFrameworkCore;

namespace kitucxa.Service
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public ReportVm GetReport(string filterType, DateTime selectedDate)
        {
            DateTime startDate;
            DateTime endDate;

            if (filterType == "day")
            {
                startDate = selectedDate.Date;
                endDate = startDate.AddDays(1);
            }
            else if (filterType == "month")
            {
                startDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                endDate = startDate.AddMonths(1);
            }
            else if (filterType == "year")
            {
                startDate = new DateTime(selectedDate.Year, 1, 1);
                endDate = startDate.AddYears(1);
            }
            else
            {
                startDate = selectedDate.Date;
                endDate = startDate.AddDays(1);
            }

            var totalRegisteredStudents = _context.Student
                .Count(s => s.CreatedAt >= startDate && s.CreatedAt < endDate);

            var totalJoinedRoomStudents = _context.StudentRoomHistories
                .Count(h =>
                    h.ActionType == "JoinRoom" &&
                    h.CreatedAt >= startDate &&
                    h.CreatedAt < endDate
                );

            var totalLeftOrTransferredStudents = _context.StudentRoomHistories
                .Count(h =>
                    (h.ActionType == "LeaveRoom" || h.ActionType == "TransferRoom") &&
                    h.CreatedAt >= startDate &&
                    h.CreatedAt < endDate
                );

            return new ReportVm
            {
                TotalRegisteredStudents = totalRegisteredStudents,
                TotalJoinedRoomStudents = totalJoinedRoomStudents,
                TotalLeftOrTransferredStudents = totalLeftOrTransferredStudents,
                FilterType = filterType,
                SelectedDate = selectedDate
            };
        }
    }
}