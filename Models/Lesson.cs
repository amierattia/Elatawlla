using System.ComponentModel.DataAnnotations;

namespace NetWork.Models;

public class Lesson
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } 
    public int CourseId { get; set; } 
    public Course Course { get; set; }
    public List<LessonParagraph> Paragraphs { get; set; } = new();

}