using DatingAppAPI.Data;
using DatingAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController(DataContext dataContext) : ControllerBase
    {
        [Authorize]
        [HttpGet("auth")]        
        public ActionResult GetAuth()
        {
            return Ok("Secret text");
        }
        [HttpGet("not-found")]        
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = dataContext.Users.Find(-1);
            if (thing == null) return NotFound();
            return thing;
        }
        [HttpGet("server-error")]        
        public ActionResult<AppUser> GetServerError()
        {
            var thing = dataContext.Users.Find(-1)?? throw new Exception("A Bad thing has happened");
            return thing;
        }
        [HttpGet("bad-request")]        
        public ActionResult GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}
