using DineFlow.Domain.Entities.OrderItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DineFlow.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.InPreparationQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.ReadyQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.ServedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CancelledQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired(false);

            builder.Ignore(x => x.Status);

            builder.Ignore(x => x.ActiveQuantity);

            builder.Ignore(x => x.PendingQuantity);

            builder.HasIndex("OrderId");
        }
    }
}