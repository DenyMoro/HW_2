using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

[ApiController]
[Route("api/photos")]
public class PhotoController : ControllerBase
{
    [HttpPost("upload")]
    public IActionResult UploadPhoto([FromBody] PhotoModel model)
    {
        try
        {
            string fileName = $"{Guid.NewGuid()}.jpg";
            if (model.Photo.Contains(','))
                model.Photo = model.Photo.Split(',')[1];
            byte[] byteArray = Convert.FromBase64String(model.Photo);

            using Image image = Image.Load(byteArray);

            int maxWidth = 200, maxHeight = 200;
            var resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            };
            image.Mutate(x => x.Resize(resizeOptions));

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(folderPath, fileName);

            image.Save(filePath);

            return Ok(new { imageUrl = $"/images/{fileName}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class PhotoModel
{
    public required string Photo { get; set; } 
}
