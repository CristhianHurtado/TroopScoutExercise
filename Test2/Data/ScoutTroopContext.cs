using Microsoft.EntityFrameworkCore;
using Test2.Models;
using Test2.ViewModels;

namespace Test2.Data
{
    public class ScoutTroopContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public ScoutTroopContext(DbContextOptions<ScoutTroopContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

        public DbSet<Troop> Troops { get; set; }
        public DbSet<Scout> Scouts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ScoutPhoto> scoutPhotos { get; set; }
        public DbSet<ScoutThumbnail> scoutThumbnails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Unique number for troop
            modelBuilder.Entity<Troop>()
            .HasIndex(p => p.TroopNumber)
            .IsUnique();

            //Unique First Name Last Name DOB combination
            modelBuilder.Entity<Scout>()
            .HasIndex(p => new { p.FirstName, p.LastName, p.DOB })
            .IsUnique();

            //One to many troop scout  
            modelBuilder.Entity<Troop>()
                .HasMany<Scout>(p => p.Scouts)
                .WithOne(c => c.Troop)
                .HasForeignKey(c => c.TroopID)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }

        public DbSet<Test2.ViewModels.AchievementSummaryVM> AchievementSummaryVM { get; set; }
    }
}
