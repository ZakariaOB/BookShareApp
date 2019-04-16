using BookShareApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShareApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Value> Values { get; set; }
   }
}