using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Identity.Api.Entities;
using User_Identity.Api.Models.DTO.Create;
using User_Identity.Api.Models.DTO.Get;
using FluentValidation;
using FluentValidation.Results;
using User_Identity.Api.Extensions;

namespace User_Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidator<ApplicationUserCreate> _validator;
        
        public UsersController(UserManager<ApplicationUser> userManager, 
                               SignInManager<ApplicationUser> signInManager,
                               IValidator<ApplicationUserCreate> validator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUserGet>> Register([FromBody] ApplicationUserCreate create)
        {
            // Validate input using FluentValidation
            ValidationResult results = _validator.Validate(create);
            if (!results.IsValid)
            {
                results.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            // Cast ApplicationUserCreate to ApplicationUser using the explicit operator
            ApplicationUser user = (ApplicationUser) create;

            var result = await _userManager.CreateAsync(user, create.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok((ApplicationUserGet)user);
        }
    }
}
