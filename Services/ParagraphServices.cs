using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using NetWork.Models;

namespace NetWork.Services;

public class ParagraphServices
{
    
   
    
    // application 
    private readonly FileService _fileService;
    private readonly ApplicationDbContext _context;

    public ParagraphServices(FileService fileService, ApplicationDbContext dbContext)
    {
        _fileService = fileService;
        _context = dbContext;
    }
    
    public async Task AddParagraphs(int lessonId, List<string> titles, List<string> contents, List<IFormFile> images, List<IFormFile> videos, List<IFormFile> audios, List<string> externalLinks)
    {
        var paragraphs = new List<LessonParagraph>();

        for (int i = 0; i < titles.Count; i++)
        {
            var paragraph = new LessonParagraph
            {
                LessonId = lessonId,
                Title = titles[i],
                Content = contents[i],
                ImagePath = images.Count > i && images[i] != null ? await _fileService.UploadFile(images[i], Pathes.Image) : null,
                VedioPath = videos.Count > i && videos[i] != null ? await SaveVideo(videos[i]) : null,
                AudioPath = audios.Count > i && audios[i] != null ? await SaveAudio(audios[i]) : null,
                ExternalLink = externalLinks.Count > i ? externalLinks[i] : null
            };
            paragraphs.Add(paragraph);
        }

        await _context.LessonParagraphs.AddRangeAsync(paragraphs);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLesson(int? lessonId)
    {
        var lesson = await FindLesson(lessonId); 

        if (lesson != null) 
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> UpdateParagraphAsync(LessonParagraph updatedParagraph)
    {
        var paragraph = await _context.LessonParagraphs.FindAsync(updatedParagraph.Id);
        if (paragraph == null) return false;

        paragraph.Content = updatedParagraph.Content;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteParagraphAsync(int id)
    {
        var paragraph = await _context.LessonParagraphs.FindAsync(id);
        if (paragraph == null) return false;

        _context.LessonParagraphs.Remove(paragraph);
        await _context.SaveChangesAsync();
        return true;
    }
    
    
    public async Task<List<LessonParagraph>> GetLessonParagraphsAsync(int lessonId)
    {
        return await _context.LessonParagraphs
            .Where(lp => lp.LessonId == lessonId)
            .Include(lp => lp.Lesson)
            .ToListAsync();
    }

    public async Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Include(l => l.Course)
            .Include(l => l.Paragraphs)
            .ToListAsync();

    }
    
  

    public async Task UpdateLessonParagraphAsync(LessonParagraph lessonParagraph)
    {
        _context.LessonParagraphs.Update(lessonParagraph);
        await _context.SaveChangesAsync();
    }


    // hepler functions 
    public static class Pathes
    {
        public const string Image = "images/paragraphs/";
        public const string videos = "videos/paragraphs/";
        public const string audios = "audios/paragraphs/";
    }
    
    private async Task<string> SaveVideo(IFormFile video)
    {
        return await _fileService.UploadFile(video, Pathes.videos);
    }

    private async Task<string> SaveAudio(IFormFile audio)
    {
        return await _fileService.UploadFile(audio, Pathes.audios);
    }

    public async Task<Lesson?> FindLesson(int? lessonId)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
    }
}