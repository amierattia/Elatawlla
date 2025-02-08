namespace NetWork.Services;

public class FileService
{
    public async Task<string> UploadFile(IFormFile file, string destinationFolder)
    {
        string uniqueFileName = string.Empty;

        if (file != null && file.Length > 0)
        {
            
            string uploadsFolder = Path.Combine(@"./wwwroot/", destinationFolder);
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }

        return uniqueFileName;
    }
}