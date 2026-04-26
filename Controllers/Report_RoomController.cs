using kitucxa.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kitucxa.Models;

namespace kitucxa.Controllers
{
    
    public class Report_RoomController : Controller
    {
        private readonly IReport_RoomService _reportRoomService;

        public Report_RoomController(IReport_RoomService reportRoomService)
        {
            _reportRoomService = reportRoomService;
        }

        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult SendReport(Report_Room reportRoom)
        {
            if (ModelState.IsValid)
            {
                _reportRoomService.SendReport_Room(reportRoom);
                return RedirectToAction("Dashboard", "Student");
            }

            return View("Index", reportRoom);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageReports()
        {
            var reports = _reportRoomService.GetAll();
            return View(reports);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, string status)
        {
            _reportRoomService.UpdateStatus(id, status);
            return RedirectToAction("ManageReports");
        }
    }
}