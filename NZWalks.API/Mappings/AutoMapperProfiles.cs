using AutoMapper;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Mappings
{
  public class AutoMapperProfiles : Profile
  {

    public AutoMapperProfiles() 
    {
      //Region
      CreateMap<Region, RegionDTO>().ReverseMap();
      CreateMap<AddRegionRequestDTO, RegionDTO>().ReverseMap();
      CreateMap<AddRegionRequestDTO, Region>().ReverseMap();
      CreateMap<UpdateRegionRequestDTO, Region>().ReverseMap();

      //Walk
      CreateMap<AddWalkRequestDTO, Walk>().ReverseMap();
      CreateMap<Walk, WalkDTO>().ReverseMap();
      CreateMap<UpdateWalkRequestDTO, Walk>().ReverseMap();



      //Difficulty
      CreateMap<Difficulty, DifficultyDTO>().ReverseMap();

    }

    //Example
      //  public AutoMapperProfiles()
      //  {
      //CreateMap<UserDTO, UserDomain>()
      //  .ForMember(x => x.Name, opt => opt.MapFrom(x => x.FullName))
      //  .ReverseMap();
      //  }
  }

  //Example
  //public class UserDTO
  //{
  //  public string FullName { get; set; }
  //}
  //public class UserDomain
  //{
  //  public string Name { get; set; }
  //}
}
