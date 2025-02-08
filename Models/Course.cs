using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NetWork.Models;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Display(Name = "Course Name")]
    public string Title { get; set; }

    [Required, MaxLength(50)]
    [Display(Name = "Course Description")]
    public string? Description { get; set; }

    public List<Lesson> Lessons { get; set; }

    public string? InstructorId { get; set; } 
    [ForeignKey("InstructorId")]
    public IdentityUser? Instructor { get; set; }
}
