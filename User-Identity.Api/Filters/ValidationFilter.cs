using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using User_Identity.Api.Extensions;

namespace User_Identity.Api.Filters;

public class ValidationFilter<T> : IAsyncActionFilter where T : class
{
    private readonly IValidator<T> _validator;
    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dto = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
        if (dto is not null)
        {
            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                result.AddToModelState(context.ModelState);
                var details = new ValidationProblemDetails(context.ModelState)
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path
                };
                context.Result = new BadRequestObjectResult(details);
                return;
            }
        }
        await next();
    }
}

