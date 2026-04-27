using Microsoft.AspNetCore.Mvc;
using kitucxa.Models;
using kitucxa.Service;
using Microsoft.AspNetCore.Authorization;

namespace kitucxa.Controllers;

public class RoomController : Controller
{
    private readonly IRoomService _roomService;
    private readonly IRoomPermissionService _roomPermissionService;

    public RoomController(
        IRoomService roomService,
        IRoomPermissionService roomPermissionService)
    {
        _roomService = roomService;
        _roomPermissionService = roomPermissionService;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        try
        {
            var rooms = _roomService.GetAll();
            return View(rooms);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new List<Room>());
        }
    }

    [Authorize(Roles = "User")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            int studentId = GetCurrentStudentId();

            bool canView = await _roomPermissionService.CanViewRoomAsync(studentId, id);

            if (canView == false)
            {
                return Forbid();
            }

            var room = _roomService.GetRoomWithStudentsById(id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Dashboard", "Student");
        }
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Room room)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _roomService.Add(room);
                TempData["SuccessMessage"] = "Thêm phòng thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(room);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm phòng: " + ex.Message);
            return View(room);
        }
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        try
        {
            var room = _roomService.GetById(id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Room room)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _roomService.Update(room);
                TempData["SuccessMessage"] = "Cập nhật phòng thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(room);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật phòng: " + ex.Message);
            return View(room);
        }
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            var room = _roomService.GetById(id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _roomService.Delete(id);
            TempData["SuccessMessage"] = "Xóa phòng thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa phòng: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    private int GetCurrentStudentId()
    {
        string? studentIdClaim = User.FindFirst("StudentId")?.Value;

        if (studentIdClaim == null)
        {
            throw new InvalidOperationException("Không tìm thấy StudentId trong Claim.");
        }

        int studentId = int.Parse(studentIdClaim);

        return studentId;
    }
}