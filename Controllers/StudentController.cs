using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using kitucxa.Models;
using kitucxa.Service;
using Microsoft.AspNetCore.Authorization;

namespace kitucxa.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IRoomService _roomService;

        public StudentController(IStudentService studentService, IRoomService roomService)
        {
            _studentService = studentService;
            _roomService = roomService;
        }

        private void LoadRooms(object? selectedRoom = null)
        {
            try
            {
                ViewBag.Rooms = new SelectList(_roomService.GetAll(), "Id", "RoomNumber", selectedRoom);
            }
            catch
            {
                ViewBag.Rooms = new SelectList(new List<Room>(), "Id", "RoomNumber", selectedRoom);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            try
            {
                var students = _studentService.GetAll();
                return View(students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Student>());
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            try
            {
                LoadRooms();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                LoadRooms();
                return View();
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _studentService.Add(student);
                    TempData["SuccessMessage"] = "Thêm sinh viên thành công.";
                    return RedirectToAction(nameof(Index));
                }

                LoadRooms(student.RoomId);
                return View(student);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                LoadRooms(student.RoomId);
                return View(student);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm sinh viên: " + ex.Message);
                LoadRooms(student.RoomId);
                return View(student);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                var student = _studentService.GetById(id);

                if (student == null)
                {
                    return NotFound();
                }

                LoadRooms(student.RoomId);
                return View(student);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Student student)
        {
            try
            {
                if (id != student.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _studentService.Update(student);
                    TempData["SuccessMessage"] = "Cập nhật sinh viên thành công.";
                    return RedirectToAction(nameof(Index));
                }

                LoadRooms(student.RoomId);
                return View(student);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                LoadRooms(student.RoomId);
                return View(student);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật sinh viên: " + ex.Message);
                LoadRooms(student.RoomId);
                return View(student);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var student = _studentService.GetById(id);

                if (student == null)
                {
                    return NotFound();
                }

                return View(student);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _studentService.Delete(id);
                TempData["SuccessMessage"] = "Xóa sinh viên thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa sinh viên: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult Dashboard()
        {
            try
            {
                var studentIdValue = User.FindFirst("StudentId")?.Value;

                if (!int.TryParse(studentIdValue, out int studentId))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                var model = _studentService.GetDashboard(studentId);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("AccessDenied", "Account");
            }
        }
    }
}