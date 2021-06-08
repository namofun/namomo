using Microsoft.EntityFrameworkCore;

namespace SatelliteSite
{
    public class PdsContext : DbContext
    {
        public PdsContext(DbContextOptions<PdsContext> options)
            : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }
    }
}
