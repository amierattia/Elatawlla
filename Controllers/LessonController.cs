using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetWork.Models;
using NetWork.Services;

namespace NetWork.Controllers;

/// <summary>
/// Controller for managing lessons and their associated paragraphs.
/// Handles listing, updating, adding, and deleting lessons and paragraphs.
/// </summary>
public class LessonController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly FileService _fileService;
    private readonly ParagraphServices _paragraphServices;

    public LessonController(ApplicationDbContext context, FileService fileService, ParagraphServices paragraphServices)
    {
        _context = context;
        _fileService = fileService;
        _paragraphServices = paragraphServices;
    }

    /// <summary>
    /// Displays a list of lessons for a selected course.
    /// </summary>
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Index(int? courseId)
    {
        PopulateCourses();
        if (courseId == null)
        {
            ModelState.AddModelError("CourseId", "Invalid Course ID");
            return View();
        }

        var lessons = await _paragraphServices.GetLessonsByCourseIdAsync(courseId.Value);
        ViewBag.Message = lessons.Any() ? null : "No lessons found for the selected course.";
        return View(lessons);
    }

    /// <summary>
    /// Fetches paragraphs for a specific lesson and displays them for editing.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public async Task<IActionResult> Update(int lessonId)
    {
        var paragraphs = await _paragraphServices.GetLessonParagraphsAsync(lessonId);
        ViewBag.Message = paragraphs.Any() ? null : "No paragraphs found for the selected lesson.";
        return View(paragraphs);
    }

    /// <summary>
    /// Updates lesson paragraphs based on user input.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(LessonParagraph lessonParagraph)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid Data");
            return View();
        }

        await _paragraphServices.UpdateLessonParagraphAsync(lessonParagraph);
        return RedirectToAction("Index", "Course");
    }

    /// <summary>
    /// Retrieves and displays paragraphs for a specific lesson.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor,User")]
    public async Task<IActionResult> Paragraphs(int lessonId)
    {
        var paragraphs = await _paragraphServices.GetLessonParagraphsAsync(lessonId);
        ViewBag.Message = paragraphs.Any() ? null : "No paragraphs found for the selected lesson.";
        return View(paragraphs);
    }

    /// <summary>
    /// Populates the course dropdown list based on the logged-in instructor.
    /// </summary>
    private void PopulateCourses()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var courses = _context.Courses.Where(c => c.InstructorId == instructorId).ToList();
        ViewData["Courses"] = new SelectList(courses, "Id", "Title");
    }

    /// <summary>
    /// Displays the form for adding a new lesson.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpGet]
    public IActionResult Add()
    {
        PopulateCourses();
        return View();
    }

    /// <summary>
    /// Handles adding a new lesson and associated paragraphs.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Lesson lesson, List<IFormFile> paragraphImages, List<IFormFile> paragraphVideos, List<IFormFile> paragraphAudios, List<string> paragraphTitles, List<string> paragraphContents, string recordedAudio)
    {
        PopulateCourses();
        if (ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid Data");
            return View(lesson);
        }

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        await _paragraphServices.AddParagraphs(lesson.Id, paragraphTitles, paragraphContents, paragraphImages, paragraphVideos, paragraphAudios, new List<string>());
        return RedirectToAction("Index", "Course");
    }

    /// <summary>
    /// Displays a confirmation page before deleting a lesson.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public async Task<IActionResult> Delete(int? lessonId)
    {
        if (lessonId == null)
        {
            return NotFound();
        }

        var lesson = await _paragraphServices.FindLesson(lessonId);
        return lesson == null ? NotFound() : View(lesson);
    }

    /// <summary>
    /// Handles lesson deletion after confirmation.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _paragraphServices.DeleteLesson(id);
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Updates a paragraph via AJAX.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpPost]
    public async Task<IActionResult> UpdateParagraph([FromBody] LessonParagraph updatedParagraph)
    {
        var success = await _paragraphServices.UpdateParagraphAsync(updatedParagraph);
        return Json(new { success });
    }

    /// <summary>
    /// Deletes a paragraph via AJAX.
    /// </summary>
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpDelete]
    public async Task<IActionResult> DeleteParagraph(int id)
    {
        var success = await _paragraphServices.DeleteParagraphAsync(id);
        return Json(new { success, message = success ? "" : "Paragraph not found" });
    }
}
