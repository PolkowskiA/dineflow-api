using DineFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DineFlow.Infrastructure.Persistance.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.SubmittedAt);

            builder.Property(x => x.PaidAt);

            builder.Property(x => x.ClosedAt);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.CreatedAt);

            builder.HasMany<OrderItem>("_items")
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(x => x.Items);
        }
    }
}