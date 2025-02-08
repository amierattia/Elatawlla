    using NetWork.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NetWork.Services;

    namespace NetWork.Controllers;

    [Authorize(Roles = "Admin,Instructor,Suber_Instructor")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CourseServices _courseServices;
        private readonly ILogger<CourseController> _logger;


        public CourseController(ApplicationDbContext context, UserManager<IdentityUser> userManager, CourseServices courseServices, ILogger<CourseController> logger)
        {
            _context = context;
            _userManager = userManager;
            _courseServices = courseServices;
            _logger = logger;
        }

        // Add Course - GET
        [HttpGet]
        public IActionResult Add() => View();

        // Add Course - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Course course ,  IdentityUser user)
        {
            if (!ModelState.IsValid)
            {
                await _courseServices.AddCourse(course, user);
                return RedirectToAction("Dashboard", "Admin");
            }
        
            ModelState.AddModelError("", "Invalid course data");
            return View(course);
        }

        // Edit Course - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Course ID: {Id}", id);
                return RedirectToAction("Index", "Home");
            }

            var course = await _courseServices.GetCourseId(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {Id} not found!", id);
                ModelState.AddModelError("", "Course not found");
                return RedirectToAction("Index", "Home");
            }

            return View(course);
        }


        // Edit Course - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            if (!ModelState.IsValid)
            {
                if (course != null)
                {
                   await _courseServices.UpdateCourse(course);
                    return RedirectToAction("Index", "Home");
                }
            }
            
            ModelState.AddModelError("", "Invalid course data");
            return View(course);
        }

        // Delete Course - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                ModelState.AddModelError("", "Course not found");
                return RedirectToAction("Index", "Home");
            }

            return View(course);
        }

        // Delete Course - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Course course)
        {
            if (course == null)
            {
                ModelState.AddModelError("", "Course not found");
                return RedirectToAction("Index", "Home");
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "User")]
        // View Lessons of Course
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = User.IsInRole("Admin") || User.IsInRole("Suber_Instructor") 
                ? await _context.Courses.Include(c => c.Lessons).ToListAsync() 
                : await _context.Courses.Where(c => c.InstructorId == user.Id).Include(c => c.Lessons).ToListAsync();

            return View(courses);
        }
        
        
        [Authorize(Roles = "User")]
        // Lessons
        public IActionResult Lessons()
        {
            var lessons = _context.Lessons.ToList();
            return View(lessons);
        }
    }
