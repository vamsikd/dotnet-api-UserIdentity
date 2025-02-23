using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using User_Identity.Api.Extensions;
using Microsoft.Extensions.Logging;

namespace User_Identity.Api.Filters;

public class ValidationFilter<T> : IAsyncActionFilter where T : class
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<ValidationFilter<T>> _logger;

    public ValidationFilter(IValidator<T> validator, ILogger<ValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
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
                    Title = "Validation Errors.",
                    Detail = "One or more validation errors occurred. Please refer to the 'errors' property.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path
                };
                details.Extensions.Add("RequestId", context.HttpContext.TraceIdentifier);
                details.Extensions.Add("TraceId", Activity.Current?.Id);
                
                 _logger.LogWarning("Validation errors occurred. {@details}", details);

                context.Result = new BadRequestObjectResult(details);
                return;
            }
        }
        await next();
    }
}

