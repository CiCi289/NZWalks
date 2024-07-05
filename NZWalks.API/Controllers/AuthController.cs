using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly ITokenRepository tokenRepository;

    public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
    {
      this.userManager = userManager;
      this.tokenRepository = tokenRepository;
    }



    //Post: api/Auth/Register
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
    {
      var identityUser = new IdentityUser()
      {
        UserName = registerRequestDTO.Username,
        Email = registerRequestDTO.Username
      };

      //Create new user
      var identityResult = await userManager.CreateAsync(identityUser, registerRequestDTO.Password);

      if (identityResult.Succeeded)
      {
        //when successfully created, assign role for this newly created user
        if (registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
        {
          identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDTO.Roles);

          if (identityResult.Succeeded)
          {
            return Ok("New User is registered! Please Log in.");
          }
        }
      }
      return BadRequest("Something went wrong!");
    }

    //Post: api/Auth/Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
    {
      //get user for auth and token
      var user = await userManager.FindByEmailAsync(loginRequestDTO.Username);

      if(user != null)
      {
        var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (checkPasswordResult == true)
        {
          //Get Roles for token creation step
          var roles = await userManager.GetRolesAsync(user);

          if (roles != null) 
          {
            //Create Token from Repo
            var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

            //To also add other infos for response, created DTO
            var response = new LoginResponseDTO
            {
              JwtToken = jwtToken
            }; 

            //return Ok result with Token
            return Ok(response);
          }
        }
      }
      return BadRequest("Incorrect Username or Password!");
    }

  }
}
