using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Collections.Generic;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  
  public class RegionsController : ControllerBase
  {
    //private readonly NZWalksDbContext dbContext;

    private readonly IRegionRepository regionRepository;
    private readonly IMapper mapper;
    private readonly ILogger<RegionsController> logger;

    public RegionsController(IRegionRepository regionRepository, 
                            IMapper mapper,
                            ILogger<RegionsController> logger)
    {
      this.regionRepository = regionRepository;
      this.mapper = mapper;
      this.logger = logger;
    }

    [HttpGet] //Get all Regions
    //[Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetAll()
    {

      try
      {

        //logger.LogInformation("GetAllRegions action method was invoked");
        //logger.LogWarning("This is test warning log.");
        //logger.LogError("This is test error log");
        //throw new Exception("Custom text exception");
        //Get Data from DB
        var regionDomain = await regionRepository.GetAllAsync();

        ////Map Domain to DTOs
        //var regionsDTO = new List<RegionDTO>();
        //foreach(var region in regions)
        //{
        //  regionsDTO.Add(new RegionDTO()
        //  {
        //    Id = region.Id,
        //    Code = region.Code,
        //    Name = region.Name,
        //    RegionImageUrl = region.RegionImageUrl,
        //  });
        //}

        //Automapper instead of Manual ^
        var regionsDTO = mapper.Map<List<RegionDTO>>(regionDomain);

        logger.LogInformation($"finished request wih data: {JsonSerializer.Serialize(regionsDTO)}");

        //Return the DTO
        return Ok(regionsDTO);
      }
      catch (Exception ex) //to handle exceptions
      {
        logger.LogError(ex, ex.Message);
        throw;
      }
      
    }

    //Get single region by id
    [HttpGet]
    [Route("{id:Guid}")]
    //[Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
      //var region = dbContext.Regions.Find(id);
      var regionDomain = await regionRepository.GetByIdAsync(id);

      if (regionDomain == null)
      {
        return NotFound();
      }

      ////Map Domain to DTO
      //var regionsDTO = new RegionDTO
      //{
      //  Id = region.Id,
      //  Code = region.Code,
      //  Name = region.Name,
      //  RegionImageUrl = region.RegionImageUrl,
      //};
      var regionsDTO = mapper.Map<RegionDTO>(regionDomain);

      return Ok(regionsDTO);
    }

    //Post/ Create region
    [HttpPost]
    [ValidateModel]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDTO addRegionRequestDTO)
    {

        ////Map DTO to Domain Model
        //var regionDomainModel = new Region
        //{
        //  Code = addRegionRequestDTO.Code,
        //  Name = addRegionRequestDTO.Name,
        //  RegionImageUrl = addRegionRequestDTO.RegionImageUrl
        //};


        var regionDomainModel = mapper.Map<Region>(addRegionRequestDTO);

        //Use Domain Model to Create region
        regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

        ////Map Domain Model back to DTO (as we cant pass Domain Model to user when success)
        //var regionDTO = new RegionDTO
        //{
        //  Id = regionDomainModel.Id,
        //  Code = regionDomainModel.Code,
        //  Name = regionDomainModel.Name,
        //  RegionImageUrl = regionDomainModel.RegionImageUrl,
        //};
        var regionDTO = mapper.Map<RegionDTO>(regionDomainModel);

        return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
 
    }

    //Update region according to id
    [HttpPut]
    [ValidateModel]
    [Route("{id:Guid}")]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDTO updateRegionRequestDTO)
    {
    
        ////convert dto to domain model for repo
        //var regionDomainModel = new Region
        //{
        //  Code = updateRegionRequestDTO.Code,
        //  Name = updateRegionRequestDTO.Name,
        //  RegionImageUrl = updateRegionRequestDTO.RegionImageUrl,
        //};
        var regionDomainModel = mapper.Map<Region>(updateRegionRequestDTO);

        regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

        if (regionDomainModel == null)
        {
          return NotFound();
        }

        ////Convert Domain to DTO for response
        //var regionDTO = new RegionDTO
        //{
        //  Id = regionDomainModel.Id,
        //  Code = regionDomainModel.Code,
        //  Name = regionDomainModel.Name,
        //  RegionImageUrl = regionDomainModel.RegionImageUrl,
        //};

        var regionDTO = mapper.Map<RegionDTO>(regionDomainModel);
        return Ok(regionDTO);

    }

    //Delete a region by Id
    [HttpDelete]
    [Route("{id:Guid}")]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Delete([FromRoute] Guid id) 
    {
      var regionDomainModel = await regionRepository.DeleteAsync(id);

      if (regionDomainModel == null)
      {
        return NotFound();
      }

      ////response
      ////(optional: return deleted region)
      ////(map domain to DTO)
      //var regionDTO = new RegionDTO
      //{
      //  Id = regionDomainModel.Id,
      //  Code = regionDomainModel.Code,
      //  Name = regionDomainModel.Name,
      //  RegionImageUrl = regionDomainModel.RegionImageUrl,
      //};
      var regionDTO = mapper.Map<RegionDTO>(regionDomainModel);

      return Ok(regionDTO);
    }

  }
}
