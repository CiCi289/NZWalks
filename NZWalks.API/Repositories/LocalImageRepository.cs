using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
  public class LocalImageRepository : IImageRepository
  {
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly NZWalksDbContext dbContext;

    public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            NZWalksDbContext dbContext)
    {
      this.webHostEnvironment = webHostEnvironment;
      this.httpContextAccessor = httpContextAccessor;
      this.dbContext = dbContext;
    }


    public async Task<Image> Upload(Image image)
    {
      //point url to that local file path for images
      var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",
                                    $"{image.FileName}{image.FileExtension}");
      //To upload image to local path
      //Create the file there
      using var stream = new FileStream(localFilePath, FileMode.Create);
      //Copy to localpath
      await image.File.CopyToAsync(stream);

      //Inject HttpContext to retrieve 
      //https://localhost:1234/images/image.jpg

      //create url path to upload to the table
      var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

      image.FilePath = urlFilePath;

      //save to database (add image to images table)
      await dbContext.Images.AddAsync(image);
      await dbContext.SaveChangesAsync();

      return image;
    }
  }
}
