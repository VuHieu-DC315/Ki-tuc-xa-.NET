using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kitucxa.Models;
using kitucxa.Service;
using Microsoft.AspNetCore.Authorization;


namespace kitucxa.Controllers;

public class RoomController : Controller
{
    private readonly IRoomService _roomService;
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var Room = _roomService.GetAll();
        return View(Room);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Room room)
    {
        if (ModelState.IsValid)
        {
            _roomService.Add(room);
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Room room)
    {
        if (ModelState.IsValid)
        {
            _roomService.Update(room);
            return RedirectToAction(nameof(Index));
        }
        return View(room);
    }

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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