using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
using Service.Commons.Notifications;

namespace Service.Api.Utils;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validationErrors = new List<ValidationFailure>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            if (argument is IEnumerable enumerableArgument && argument is not string)
            {
                ValidateEnumerableArgument(context, enumerableArgument, validationErrors);
            }
            else
            {
                await ValidateSingleArgumentAsync(context, argument, validationErrors);
            }
        }

        if (validationErrors.Count != 0)
        {
            SetValidationResponse(context, validationErrors);
            return;
        }

        await next();
    }

    private async Task ValidateSingleArgumentAsync(ActionExecutingContext context, object argument, List<ValidationFailure> errors)
    {
        var validator = GetValidatorForArgument(context, argument);
        if (validator == null) return;

        var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument));
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors);
        }
    }

    private void ValidateEnumerableArgument(ActionExecutingContext context, IEnumerable enumerableArgument, List<ValidationFailure> errors)
    {
        var index = 0;
        bool isEmpty = true;

        foreach (var item in enumerableArgument)
        {
            isEmpty = false;
            if (item == null) continue;

            var validator = GetValidatorForArgument(context, item);
            if (validator == null) continue;

            var validationResult = validator.Validate(new ValidationContext<object>(item));
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    error.PropertyName = $"{error.PropertyName}[{index}]";
                    errors.Add(error);
                }
            }
            index++;
        }

        if (isEmpty)
        {
            errors.Add(new ValidationFailure("", NotificationMessage.Common.RequestListRequired));
        }
    }

    private void SetValidationResponse(ActionExecutingContext context, List<ValidationFailure> validationErrors)
    {
        var path = context.HttpContext.Request.Path;
        var result = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = NotificationTitle.BadRequest,
            Detail = NotificationMessage.Common.ValidationError,
            Instance = path,
            Extensions = { { "traceId", Activity.Current?.Id } }
        };

        foreach (var failure in validationErrors)
        {
            if (result.Errors.ContainsKey(failure.PropertyName))
            {
                result.Errors[failure.PropertyName] = result.Errors[failure.PropertyName].Concat(new[] { failure.ErrorMessage }).ToArray();
            }
            else
            {
                result.Errors.Add(failure.PropertyName, new[] { failure.ErrorMessage });
            }
        }

        context.Result = new BadRequestObjectResult(result);
    }

    private static IValidator? GetValidatorForArgument(ActionExecutingContext context, object argument)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
        return (IValidator?)context.HttpContext.RequestServices.GetService(validatorType);
    }
}
