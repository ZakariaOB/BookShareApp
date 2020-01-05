using System.Collections.Generic;
using System.Threading.Tasks;
using BookShareApp.API.Data;
using BookShareApp.API.Framework;
using BookShareApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShareApp.API.DataAccess
{
    public class BookShareRepository : IBookShareRepository
    {
        private readonly DataContext _context;
        public BookShareRepository(DataContext context)
        {
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

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(o => o.Photos).FirstOrDefaultAsync(o => o.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(o => o.Photos);
            return await PagedList<User>.CreateAsync(
                users, 
                userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Photo> GetMainPhotoForUser(int userId)
        {
            return _context.Photos.FirstOrDefaultAsync(u => u.UserId == userId && u.IsMain);
        }
    }
}