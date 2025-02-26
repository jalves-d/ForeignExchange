﻿namespace ForeignExchange.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
    }

}
