using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetWork.Models;

namespace NetWork.Controllers;

/// <summary>
/// Controller for managing lessons and their associated paragraphs.
/// Handles listing, updating, adding, and deleting lessons.
/// </summary>
[Authorize]
public class LessonController : Controller
{
    private readonly ApplicationDbContext _context;

    public LessonController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Accessible to User, Admin, Instructor, Suber_Instructor
    [Authorize(Roles = "User,Admin,Instructor,Suber_Instructor")]
    public IActionResult Index(int courseId)
    {
        var lessons = _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToList();

        ViewBag.CourseId = courseId;
        return View(lessons);
    }

    // Accessible to User, Admin, Instructor, Suber_Instructor
    [Authorize(Roles = "User,Admin,Instructor,Suber_Instructor")]
    public IActionResult Details(int id)
    {
        var lesson = _context.Lessons.Find(id);
        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    // Restricted to Admin, Instructor, Suber_Instructor
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public IActionResult Add(int courseId)
    {
        ViewBag.CourseId = courseId;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public async Task<IActionResult> Add(Lesson lesson)
    {
        if (ModelState.IsValid)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { courseId = lesson.CourseId });
        }

        ViewBag.CourseId = lesson.CourseId;
        return View(lesson);
    }

    // Restricted to Admin, Instructor, Suber_Instructor
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public IActionResult Edit(int id)
    {
        var lesson = _context.Lessons.Find(id);
        if (lesson == null) return NotFound();
        return View(lesson);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public async Task<IActionResult> Edit(Lesson lesson)
    {
        if (ModelState.IsValid)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { courseId = lesson.CourseId });
        }
        return View(lesson);
    }

    // Restricted to Admin, Instructor, Suber_Instructor
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return NotFound();

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", new { courseId = lesson.CourseId });
    }
}