using System;
using System.Linq;
using User_Identity.Api.Entities;

namespace User_Identity.Api.Models.DTO.Create;

public class ApplicationUserCreate
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public static explicit operator ApplicationUser(ApplicationUserCreate create)
    {
        if (create is null)
        {
            throw new ArgumentNullException(nameof(create));
        }

        return new ApplicationUser
        {
            Email = create.Email,
            FirstName = create.FirstName,
            LastName = create.LastName,
            UserName = create.Email
        };
    }      
}