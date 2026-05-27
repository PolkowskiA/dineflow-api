using DineFlow.Domain.Entities.OrderItems;
using DineFlow.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DineFlow.Infrastructure.Persistence.Configurations
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
                .IsRowVersion()
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired(false);

            builder.HasIndex(x => x.CreatedAt);

            builder.HasMany<OrderItem>("_items")
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(static x => x.Items);
        }
    }
}