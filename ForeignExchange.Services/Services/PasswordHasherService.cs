using ForeignExchange.Application.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace ForeignExchange.Application.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly IConfiguration _configuration;
        private readonly string _pepper;
        private readonly int _saltSize;
        private readonly int _iterationCount;
        private readonly int _keySize;

        public PasswordHasherService(IConfiguration configuration)
        {
            _configuration = configuration;

            var encryptHashingSection = _configuration.GetSection("EncryptHashing");

            _pepper = encryptHashingSection["Pepper"] ?? throw new ArgumentNullException("EncryptHashing:Pepper");
            _saltSize = encryptHashingSection.GetValue<int>("SaltSize");
            _iterationCount = encryptHashingSection.GetValue<int>("IterationCount");
            _keySize = encryptHashingSection.GetValue<int>("KeySize");

            if (_saltSize <= 0) throw new ArgumentOutOfRangeException("EncryptHashing:SaltSize");
            if (_iterationCount <= 0) throw new ArgumentOutOfRangeException("EncryptHashing:IterationCount");
            if (_keySize <= 0) throw new ArgumentOutOfRangeException("EncryptHashing:KeySize");

        }

        public string HashPassword(string password)
        {
            byte[] salt = GenerateSalt(_saltSize);
            string saltedPassword = password + _pepper;

            byte[] hash = KeyDerivation.Pbkdf2(
                password: saltedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _iterationCount,
                numBytesRequested: _keySize
            );

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedPasswordHash = Convert.FromBase64String(parts[1]);

            string saltedPassword = password + _pepper;

            byte[] computedHash = KeyDerivation.Pbkdf2(
                password: saltedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _iterationCount,
                numBytesRequested: _keySize
            );

            return storedPasswordHash.SequenceEqual(computedHash);
        }

        private static byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }

}
