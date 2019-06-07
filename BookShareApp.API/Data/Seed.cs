using System.Collections.Generic;
using System.Security.Cryptography;
using BookShareApp.API.Models;
using Newtonsoft.Json;

namespace BookShareApp.API.Data
{
    public class Seed
    {
        private readonly DataContext context;

        public Seed(DataContext context)
        {
            this.context = context;
        }

        public void SeedUsers()
        {
            string userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in users)
            {
                byte[] passHash, passSalt;
                CreatePasswordHash("password", out passHash, out passSalt);

                user.PasswordHash = passHash;
                user.PasswordSalt = passSalt;
                user.UserName = user.UserName.ToLower();

                context.Users.Add(user);
            }

            context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}