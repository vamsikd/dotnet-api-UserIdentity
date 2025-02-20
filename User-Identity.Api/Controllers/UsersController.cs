using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Identity.Api.Entities;
using User_Identity.Api.Models.DTO.Create;
using User_Identity.Api.Models.DTO.Get;

namespace User_Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUserGet>> Register([FromBody] ApplicationUserCreate create)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cast ApplicationUserCreate to ApplicationUser using the explicit operator
            ApplicationUser user = (ApplicationUser) create;

            var result = await _userManager.CreateAsync(user, create.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok((ApplicationUserGet)user);
        }
    }
}
