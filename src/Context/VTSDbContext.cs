using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.MenuItemler.Entity;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.CurrentUser.Service;
using AIInstructor.src.Shared.RDBMS.Entity;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.Context
{
    public class VTSDbContext : DbContext
    {
        private readonly ICurrentUserService currentUserService;

        public VTSDbContext(DbContextOptions<VTSDbContext> options, ICurrentUserService _currentUserService)
            : base(options)
        {
            currentUserService = _currentUserService;
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                var user = currentUserService.GetCurrentUsername();

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = user;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = user;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; // soft delete
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = user;
                        break;
                }
            }
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        // Genel entity'ler
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<KullaniciGrup> KullaniciGruplar { get; set; }
        public DbSet<KullaniciKullaniciGrup> KullaniciKullaniciGruplar { get; set; }
        public DbSet<KullaniciGrupRol> KullaniciGrupRoller { get; set; }
        public DbSet<MenuItem> MenuItemler { get; set; }
        public DbSet<MenuItemRol> MenuItemRoller { get; set; }
        public DbSet<TrainingScenario> TrainingScenarios { get; set; }
        public DbSet<ScenarioSession> ScenarioSessions { get; set; }
        public DbSet<ScenarioMessage> ScenarioMessages { get; set; }
        public DbSet<ScenarioEvaluation> ScenarioEvaluations { get; set; }
        public DbSet<StudentGamificationProfile> StudentGamificationProfiles { get; set; }
        public DbSet<EarnedBadge> EarnedBadges { get; set; }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            // Örnek ayarlar
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.Property(e => e.Domain).HasDefaultValue(" ");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.HasIndex(e => e.KullaniciAdi).IsUnique();
                entity.Property(e => e.TCNO).HasMaxLength(11);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<KullaniciGrup>(entity =>
            {
                entity.HasIndex(e => e.Ad).IsUnique();
            });

            modelBuilder.Entity<TrainingScenario>(entity =>
            {
                entity.Property(e => e.Language).HasMaxLength(50).HasDefaultValue("tr-TR");
            });

            modelBuilder.Entity<ScenarioSession>(entity =>
            {
                entity.HasOne(session => session.Scenario)
                    .WithMany(scenario => scenario.Sessions)
                    .HasForeignKey(session => session.ScenarioId);
            });

            modelBuilder.Entity<ScenarioMessage>(entity =>
            {
                entity.HasOne(message => message.Session)
                    .WithMany(session => session.Messages)
                    .HasForeignKey(message => message.SessionId);
            });

            modelBuilder.Entity<ScenarioEvaluation>(entity =>
            {
                entity.HasOne(evaluation => evaluation.Session)
                    .WithOne(session => session.Evaluation)
                    .HasForeignKey<ScenarioEvaluation>(evaluation => evaluation.SessionId);
            });

            modelBuilder.Entity<StudentGamificationProfile>(entity =>
            {
                entity.HasIndex(profile => profile.StudentId).IsUnique();
            });

            modelBuilder.Entity<EarnedBadge>(entity =>
            {
                entity.HasOne(badge => badge.GamificationProfile)
                    .WithMany(profile => profile.Badges)
                    .HasForeignKey(badge => badge.GamificationProfileId);
            });

            

          

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var body = Expression.Equal(prop, Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
