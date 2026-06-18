using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using DineFlow.Domain.Entities.OrderItems;
using DineFlow.Domain.Entities.Orders;
using DineFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DineFlow.Tests.Infrastructure
{
    public class TestAppDbContext : AppDbContext
    {
        private static readonly RandomNumberGenerator RowVersionGenerator = RandomNumberGenerator.Create();

        public TestAppDbContext(DbContextOptions<TestAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (string.Equals(Database.ProviderName, "Microsoft.EntityFrameworkCore.Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                modelBuilder.Entity<Order>().Property(x => x.RowVersion).ValueGeneratedNever();
                modelBuilder.Entity<OrderItem>().Property(x => x.RowVersion).ValueGeneratedNever();
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplySqliteRowVersionTokens();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySqliteRowVersionTokens();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplySqliteRowVersionTokens();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplySqliteRowVersionTokens()
        {
            if (!string.Equals(Database.ProviderName, "Microsoft.EntityFrameworkCore.Sqlite", StringComparison.OrdinalIgnoreCase))
                return;

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified))
            {
                var property = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "RowVersion" && p.Metadata.ClrType == typeof(byte[]));
                if (property is null)
                    continue;

                property.CurrentValue = GenerateRowVersion();
                property.IsModified = true;
            }
        }

        private static byte[] GenerateRowVersion()
        {
            var rowVersion = new byte[8];
            RowVersionGenerator.GetBytes(rowVersion);
            return rowVersion;
        }
    }
}