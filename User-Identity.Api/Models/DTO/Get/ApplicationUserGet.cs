using System;
using User_Identity.Api.Entities;

namespace User_Identity.Api.Models.DTO.Get;

public class ApplicationUserGet
{
    public Guid Id { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public static explicit operator ApplicationUserGet(ApplicationUser user)
    {
        return new ApplicationUserGet
        {
            Id = Guid.Parse(user.Id), 
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }    
}
