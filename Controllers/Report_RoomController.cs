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
        [ValidateAntiForgeryToken]
        public IActionResult SendReport(Report_Room reportRoom)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _reportRoomService.SendReport_Room(reportRoom);
                    TempData["Success"] = "Gửi yêu cầu đổi phòng thành công.";
                    return RedirectToAction("Dashboard", "Student");
                }

                return View("Index", reportRoom);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", reportRoom);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageReports()
        {
            try
            {
                var reports = _reportRoomService.GetAll();
                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Report_Room>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            try
            {
                _reportRoomService.UpdateStatus(id, status);

                if (status == "Confirm")
                {
                    TempData["Success"] = "Đã duyệt yêu cầu thành công.";
                }
                else if (status == "Reject")
                {
                    TempData["Success"] = "Đã từ chối yêu cầu thành công.";
                }

                return RedirectToAction("ManageReports");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ManageReports");
            }
        }
    }
}