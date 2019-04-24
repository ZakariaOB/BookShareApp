using System;
using System.Threading.Tasks;
using BookShareApp.API.Models;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using BookShareApp.API.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookShareApp.API.DataAccess
{
    public class AuthRepository : IAuthRepository
    {
        private DataContext _context;
        public AuthRepository(DataContext context) => this._context = context;
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(i => i.UserName == username);
            if (user == null)
            {
                return null;
            }

            var passwordHash = user.PasswordHash;
            var passwordSalt = user.PasswordSalt;

            bool isPassOk = VerifyPassword(password, passwordHash, passwordSalt);

            if (!isPassOk) {
                return null;
            }

            return null;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordHash))
            {
                var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Enumerable.Range(0, computed.Length - 1).All(i => computed[i] == passwordHash[i]);
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username) => await _context.Users.AnyAsync(t => t.UserName == username);
    }
}