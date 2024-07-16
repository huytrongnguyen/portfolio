using Microsoft.EntityFrameworkCore;

namespace Portfolio.Core;

public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options) {
  public DbSet<DailyUserMetric> DailyUserMetric { get; set; }
}