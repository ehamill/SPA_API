using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MAWcore6.Data;
using MAWcore6.Models;
using System.Security.Claims;
//using Microsoft.AspNet.Identity;

namespace MAWcore6.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        //public CityController()
        //{
        //}
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CityController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public class City
        {
            public int CityId { get; set; }
            public string ServerId { get; set; }
            [Required]
            public string UserId { get; set; }
            public string CityName { get; set; }

        }

        
        [HttpGet]
        public async Task<JsonResult> Get()
        { 
            string ServerId = User.Identity!.Name!;
            
            var UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            City UserCity = new City()
            {
                CityId = 1,
                CityName = "FirstCityName",
                //ServerId = 1,
                UserId = UserID,// userManager.Id
            };

            return new JsonResult(new { city = UserCity });
        }

        

    }
}
