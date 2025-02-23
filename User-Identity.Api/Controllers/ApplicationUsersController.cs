using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Identity.Api.Entities;
using User_Identity.Api.Models.DTO.Create;
using User_Identity.Api.Models.DTO.Get;
using FluentValidation;
using User_Identity.Api.Extensions;
using User_Identity.Api.Filters;

namespace User_Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public UsersController(UserManager<ApplicationUser> userManager, 
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [ServiceFilter(typeof(ValidationFilter<ApplicationUserCreate>))]
        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUserGet>> Register([FromBody] ApplicationUserCreate create)
        {
            ApplicationUser user = (ApplicationUser) create;
            var result = await _userManager.CreateAsync(user, create.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok((ApplicationUserGet)user);
        }
    }
}
