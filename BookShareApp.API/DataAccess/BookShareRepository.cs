using System.Collections.Generic;
using System.Threading.Tasks;
using BookShareApp.API.Data;
using BookShareApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShareApp.API.DataAccess
{
    public class BookShareRepository : IBookShareRepository
    {
        private readonly DataContext _context;
        public BookShareRepository(DataContext context) {
            this._context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(o => o.Photos).FirstOrDefaultAsync(o => o.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(o => o.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}