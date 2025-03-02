using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForeignExchange.Application.Services
{
    using FluentValidation;
    using ForeignExchange.Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult?> ValidateAsync<T>(T dto)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator is null)
                return new BadRequestObjectResult(new { message = "No validator found for this type." });

            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(new
                {
                    message = "Validation error.",
                    errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            return null;
        }
    }

}
