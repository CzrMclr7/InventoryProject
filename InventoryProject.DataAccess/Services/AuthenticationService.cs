using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models.Authentication;
using InventoryProject.DataAccess.Persistence.Repositories.UserRepo;
using InventoryProject.DataAccess.Services.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InventoryProject.DataAccess.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;

        public AuthenticationService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<User> Authenticate(AuthRequest authRequest)
        {
            // Find the user by the provided username
            var user = await _userRepo.GetByUserNameAsync(authRequest.Username);

            if (user == null)
               throw new Exception("Invalid Username/Password");

            // Verify the password by comparing the hashed password
            if (!VerifyPassword(authRequest.Password, user.PasswordSalt, user.Password))
                throw new Exception("Invalid Username/Password");

            return user;
        }
        private bool VerifyPassword(string password, string passwordSalt, string hashedPassword)
        {
            // Hashed Mode
            byte[] salt = Encoding.ASCII.GetBytes(passwordSalt);
            string hashedInput = GenerateHashedPassword(password, salt);
            return hashedInput == hashedPassword;
        }

        private static string GenerateHashedPassword(string password, byte[] salt)
        {
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }
    }
}
