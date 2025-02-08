using NetWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetWork.Models;

namespace NetWork.Services;

public class CourseServices
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CourseServices(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task AddCourse(Course course , IdentityUser user)
    {
        course.InstructorId = user?.Id;
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }


    public async Task<Course> GetCourseId(int id)
    {
        return await _context.Courses
            .Include(c => c.Lessons) 
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public async Task UpdateCourse(Course course)
    {
        _context.Courses.Update(course);
        _context.SaveChanges();
    }

}