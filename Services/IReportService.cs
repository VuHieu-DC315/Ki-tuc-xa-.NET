using kitucxa.Models;

namespace kitucxa.Service
{
    public interface IReportService
    {
        ReportVm GetReport(string filterType, DateTime selectedDate);
    }
}