using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetWork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // Display courses based on user roles
        [Authorize(Roles = "User,Admin,Instructor")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Course> courses;

            // Admin can see all courses, others see their own courses
            if (User.IsInRole("Admin") || User.IsInRole("Instructor") || User.IsInRole("Suber_Instructor")  || User.IsInRole("User"))
            {
                courses = await _context.Courses.Include(c => c.Lessons).ToListAsync();
            }
            else
            {
                courses = await _context.Courses
                    .Where(c => c.InstructorId == user.Id)  
                    .Include(c => c.Lessons)
                    .ToListAsync();
            }

            return View(courses);
        }

        // Admin-only page
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult About()
        {
            return View(); 
        }
    }
}