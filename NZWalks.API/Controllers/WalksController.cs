using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Net;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class WalksController : ControllerBase
  {
    private readonly IMapper mapper;
    private readonly IWalkRepository walkRepository;

    public WalksController(IMapper mapper, IWalkRepository walkRepository)
    {
      this.mapper = mapper;
      this.walkRepository = walkRepository;
    }

        //Create Walks
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create([FromBody] AddWalkRequestDTO addWalkRequestDTO)
    {
      if (ModelState.IsValid)
      {
        //Map DTO to Domain
        var walkDomainModel = mapper.Map<Walk>(addWalkRequestDTO);
        await walkRepository.CreateAsync(walkDomainModel);

        //Map Domain to DTO for response
        var walkDTO = mapper.Map<WalkDTO>(walkDomainModel);

        return Ok(walkDTO);
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpGet] //e.g /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
    public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
                                            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, 
                                            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)

    {
        //Grab from domain
        var walkDomainModel = await walkRepository.GetAllAsync(filterOn, filterQuery,
                                                                sortBy, isAscending ?? true,
                                                                pageNumber, pageSize);

        throw new Exception("This is test exception");

        //Map to DTO
        var walkDTO = mapper.Map<List<WalkDTO>>(walkDomainModel);

        return Ok(walkDTO);
    
    }

    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
      var walkDomainModel = await walkRepository.GetByIdAsync(id);

      if(walkDomainModel == null)
      {
        return NotFound();
      }

      //map domain to DTO
      var walkDTO = mapper.Map<WalkDTO>(walkDomainModel);

      return Ok(walkDTO);
    }

    [HttpPut]
    [ValidateModel]
    [Route("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDTO updateWalkRequestDTO)
    {
     
        //Map DTO to Domain
        var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDTO);

        walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);

        if (walkDomainModel == null)
        {
          return NotFound();
        }

        //Map Domain to DTO
        var walkDTO = mapper.Map<WalkDTO>(walkDomainModel);

        return Ok(walkDTO);
      
    }

    [HttpDelete]
    [Route("{id:Guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      var walkDomainModel = await walkRepository.DeleteAsync(id);

      if (walkDomainModel == null)
      {
        return NotFound();
      }

      var walkDTO = mapper.Map<WalkDTO>(walkDomainModel);
      return Ok(walkDTO);
    }


  }
}
