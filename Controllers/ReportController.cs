using kitucxa.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kitucxa.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index(
            string filterType = "day",
            DateTime? selectedDate = null,
            string? selectedMonth = null,
            int? selectedYear = null)
        {
            DateTime reportDate;

            if (filterType == "day")
            {
                reportDate = selectedDate ?? DateTime.Today;

                ViewBag.SelectedDate = reportDate.ToString("yyyy-MM-dd");
            }
            else if (filterType == "month")
            {
                if (string.IsNullOrEmpty(selectedMonth))
                {
                    selectedMonth = DateTime.Today.ToString("yyyy-MM");
                }

                var parts = selectedMonth.Split('-');

                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);

                reportDate = new DateTime(year, month, 1);

                ViewBag.SelectedMonth = selectedMonth;
            }
            else if (filterType == "year")
            {
                int year = selectedYear ?? DateTime.Today.Year;

                reportDate = new DateTime(year, 1, 1);

                ViewBag.SelectedYear = year.ToString();
            }
            else
            {
                filterType = "day";
                reportDate = DateTime.Today;

                ViewBag.SelectedDate = reportDate.ToString("yyyy-MM-dd");
            }

            ViewBag.FilterType = filterType;

            var model = _reportService.GetReport(filterType, reportDate);

            return View(model);
        }
    }
}