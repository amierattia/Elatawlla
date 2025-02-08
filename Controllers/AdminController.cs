using NetWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NetWork.Controllers;

[Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // main page 
    public async Task<IActionResult> Dashboard()
    {
        var userCount = await _userManager.Users.CountAsync();
        var courseCount = await _context.Courses.CountAsync();
        var courses = await _context.Courses.ToListAsync();
        var user = await _userManager.GetUserAsync(User);

        ViewBag.UserName = user?.UserName ?? "Admin";
        ViewBag.UserEmail = user?.Email ?? "admin@example.com";
        ViewBag.UserImage = "/images/default-profile.png"; 
        ViewBag.UserCount = userCount;
        ViewBag.CourseCount = courseCount;


        if (User.IsInRole("Admin") || User.IsInRole("Suber_Instructor"))
        {
            var allCourses = await _context.Courses
                .Include(c => c.Lessons)
                .ToListAsync();
            return View(allCourses);
        }

        var instructorCourses = await _context.Courses
            .Where(c => c.InstructorId == user.Id)
            .Include(c => c.Lessons)
            .ToListAsync();

        return View(instructorCourses);

    }

    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    // update 
    [HttpGet]
    public IActionResult Update(int id)
    {
        var fId = _context.Courses.FirstOrDefault(ID => ID.Id == id);
        if (fId == null)
        {
            ModelState.AddModelError("","Not Found ");
            return View();  
        }
        return View(fId);
    }
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(Course course)
    {
        if (course == null)
        {
            ModelState.AddModelError("","Not Found ");
            return View();
        }

        else
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpGet]
    // delete 
    public IActionResult Delete(int id)
    {
        var fId = _context.Courses.FirstOrDefault(ID => ID.Id == id);
        if (fId == null)
        {
            ModelState.AddModelError("","Not Found ");
            return View();  
        }
        return View(fId);
        
    }
    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Course course)
    {
        if (course == null)
        {
            ModelState.AddModelError("","Not Found ");
            return View();
        }

        else
        {
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}