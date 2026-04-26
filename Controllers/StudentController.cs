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
            ViewBag.Rooms = new SelectList(_roomService.GetAll(), "Id", "RoomNumber", selectedRoom);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var students = _studentService.GetAll();
            return View(students);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            LoadRooms();
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _studentService.Add(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            LoadRooms(student.RoomId);
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            LoadRooms(student.RoomId);
            return View(student);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _studentService.Update(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            LoadRooms(student.RoomId);
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _studentService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public IActionResult Dashboard()
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
    }
}