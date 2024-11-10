using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test_ngo.Models;

namespace test_ngo.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<test_ngo.Models.test> test { get; set; } = default!;
        public DbSet<test_ngo.Models.Donate> Donate { get; set; } = default!;
        public DbSet<test_ngo.Models.Volunteer> Volunteer { get; set; } = default!;
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Event> Events { get; set; } 
        public DbSet<PastEvent> PastEvents { get; set; }

    }
}
