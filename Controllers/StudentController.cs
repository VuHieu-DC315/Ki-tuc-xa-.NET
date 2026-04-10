using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using kitucxa.Models;
using kitucxa.Service;

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

        public IActionResult Index()
        {
            var students = _studentService.GetAll();
            return View(students);
        }

        public IActionResult Create()
        {
            LoadRooms();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}