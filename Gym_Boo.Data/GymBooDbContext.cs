using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data;

public class GymBooDbContext : DbContext
{
    public GymBooDbContext(DbContextOptions options) : base(options)
    {
    }
    
    
}