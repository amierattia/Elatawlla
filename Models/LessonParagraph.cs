
using System.ComponentModel.DataAnnotations;

namespace NetWork.Models;

public class LessonParagraph
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int LessonId { get; set; }

    [Required, StringLength(255)]
    public string Title { get; set; } 
    [Required]
    public string Content { get; set; } 

    public string? ImagePath { get; set; }
    public string? VedioPath { get; set; }
    public string? ExternalLink { get; set; } 
    public string? AudioPath { get; set; } 

    public Lesson Lesson { get; set; }
}

