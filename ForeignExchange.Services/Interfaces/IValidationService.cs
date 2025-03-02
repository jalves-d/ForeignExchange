using Microsoft.AspNetCore.Mvc;

namespace ForeignExchange.Application.Interfaces
{
    public interface IValidationService
    {
        Task<IActionResult?> ValidateAsync<T>(T dto);
    }
}
