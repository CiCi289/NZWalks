using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ImagesController : ControllerBase
  {
    private readonly IImageRepository imageRepository;

    public ImagesController(IImageRepository imageRepository)
    {
      this.imageRepository = imageRepository;
    }

        //POST: api/Images/Upload
        [HttpPost]
    [Route("Upload")]
    public async Task<IActionResult> Upload([FromForm]ImageUploadRequestDTO request)
    {
      //Check if request is correct or not > private action method
      ValidateFileUpload(request);

      if (ModelState.IsValid)
      {
        //if valid,convert DTO to Domain first
        var imageDomainModel = new Image
        {
          File = request.File,
          FileExtension = Path.GetExtension(request.File.FileName),
          FileSizeInBytes = request.File.Length,
          FileName = request.FileName,
          FileDescription = request.FileDescription,
          
        };

        // Upload image + save to database method (from repo)
        await imageRepository.Upload(imageDomainModel);

        var imageDTO = new Image
        {
          Id = imageDomainModel.Id,
          File = imageDomainModel.File,
          FileExtension = imageDomainModel.FileExtension,
          FileSizeInBytes = imageDomainModel.FileSizeInBytes,
          FileName = imageDomainModel.FileName,
          FileDescription = imageDomainModel.FileDescription,
          FilePath = imageDomainModel.FilePath,
        };
        return Ok(imageDTO);

      }
      return BadRequest(ModelState);
    }

    //Check if request is correct or not > private action method
    private void ValidateFileUpload(ImageUploadRequestDTO request)
    {
      //Check File extension
      var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

      if (allowedExtensions.Contains(Path.GetExtension(request.File.FileName)) == false)
      {
        ModelState.AddModelError("file", "Unsupported file extension");
      }

      //Check File size
      if (request.File.Length > 10485760)
      {
        ModelState.AddModelError("file", "File size is more than 10MB. Please upload a smaller size file.");
      }
    }






  }
}
