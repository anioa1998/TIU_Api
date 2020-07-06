using Microsoft.EntityFrameworkCore;
using Schronisko_Api.Models;

namespace Schronisko_Api
{
    public class ShelterDbContext : DbContext
    {
        public ShelterDbContext(DbContextOptions<ShelterDbContext> options) : base(options) { }


        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
