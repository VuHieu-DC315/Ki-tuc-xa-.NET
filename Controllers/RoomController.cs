using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kitucxa.Models;
using kitucxa.Service;

namespace kitucxa.Controllers;

public class RoomController : Controller
{
    private readonly IRoomService _roomService;
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public IActionResult Index()
    {
        var Room = _roomService.GetAll();
        return View(Room);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Room room)
    {
        if (ModelState.IsValid)
        {
            _roomService.Add(room);
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    public IActionResult Edit(int id)
    {
        var room = _roomService.GetById(id);
        if (room == null)
        {
            return NotFound();
        }
        return View(room);
    }

    [HttpPost]
    public IActionResult Edit(Room room)
    {
        if (ModelState.IsValid)
        {
            _roomService.Update(room);
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    public IActionResult Delete(int id)
    {
        var room = _roomService.GetById(id);
        if (room == null)
        {
            return NotFound();
        }
        return View(room);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _roomService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}