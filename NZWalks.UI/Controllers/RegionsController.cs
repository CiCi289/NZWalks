using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
  public class RegionsController : Controller
  {
    private readonly IHttpClientFactory httpClientFactory;

    public RegionsController(IHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory;
    }

    //Display list all
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      List<RegionDTO> response = new List<RegionDTO>();
      try
      {
        //Get All regions from Web API
        var client = httpClientFactory.CreateClient();

        var httpResponseMessage = await client.GetAsync("https://localhost:7251/api/regions");

        httpResponseMessage.EnsureSuccessStatusCode(); //ensures success, if not throws exception

        //if success, deserialize the JSON to DTO obj , Add to response, Pass to view to display
       
        response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDTO>>());
        
        return View(response);

      }
      catch (Exception ex)
      {
        //log exception

        throw;
      }
    }

    //Add new
    [HttpGet]
    public IActionResult Add()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddRegionViewModel model)
    {
      var client = httpClientFactory.CreateClient();

      var httpRequestMessage = new HttpRequestMessage()
      {
        Method = HttpMethod.Post,
        RequestUri = new Uri("https://localhost:7251/api/regions"),
        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
      };

      var httpResponseMessage =  await client.SendAsync(httpRequestMessage);
      httpResponseMessage.EnsureSuccessStatusCode();

      var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDTO>(); //RegionDTO

      if (response != null)
      {
        return RedirectToAction("Index", "Regions");
      }
      return View();
    }

    //Better way i guess for Add
    //[HttpPost]
    //public async Task<IActionResult> Add(AddRegionViewModel model)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return View(model);
    //  }

    //  var client = httpClientFactory.CreateClient();
    //  var httpRequestMessage = new HttpRequestMessage()
    //  {
    //    Method = HttpMethod.Post,
    //    RequestUri = new Uri("https://localhost:7251/api/regions"),
    //    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
    //  };

    //  var httpResponseMessage = await client.SendAsync(httpRequestMessage);

    //  if (httpResponseMessage.IsSuccessStatusCode)
    //  {
    //    return RedirectToAction("Index", "Regions");
    //  }
    //  else
    //  {
    //    // Handle the error case
    //    ModelState.AddModelError("", "An error occurred while adding the region.");
    //    return View(model);
    //  }
    //}


    //Edit one region
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
      var client = httpClientFactory.CreateClient();

      var response = await client.GetFromJsonAsync<RegionDTO>($"https://localhost:7251/api/regions/{id.ToString()}");
      //no need to ensure here bcoz we are getting from json async method directly

      if (response != null)
      {
        return View(response);
      }
      return View(null);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(RegionDTO request) // coz we binded RegionDTO in Edit.cshtml, why RegionDTO ?
      // how about create new (EditRegionViewModel editRegionViewModel)? 
    {
      var client = httpClientFactory.CreateClient();

      var httpRequestMessage = new HttpRequestMessage()
      {
        Method = HttpMethod.Put,
        RequestUri = new Uri($"https://localhost:7251/api/regions/{request.Id}"),
        Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
      };


      var httpResponseMessage = await client.SendAsync(httpRequestMessage);
      httpResponseMessage.EnsureSuccessStatusCode();

      var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDTO>(); //RegionDTO

      if (response != null)
      {
        return RedirectToAction("Index", "Regions");
      }
      return View();
    }

    //Delete a region
    [HttpPost]
    public async Task<IActionResult> Delete(RegionDTO request) //coz binded with cshtml, so accept them as para
    {
      try
      {
        var client = httpClientFactory.CreateClient();

        var httpResponseMessage = await client.DeleteAsync($"https://localhost:7251/api/regions/{request.Id}");
        httpResponseMessage.EnsureSuccessStatusCode();

        return RedirectToAction("Index", "Regions");

      }
      catch (Exception ex)
      {

        throw;
      }

      return View("Edit");
    }

  }
}
