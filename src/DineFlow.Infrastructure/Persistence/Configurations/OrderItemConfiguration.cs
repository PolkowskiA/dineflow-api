using DineFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Infrastructure.Persistance.Configurations
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
                .IsRequired();

            builder.Property(x => x.InPreparationQuantity)
                .IsRequired();

            builder.Property(x => x.ReadyQuantity)
                .IsRequired();

            builder.Property(x => x.ServedQuantity)
                .IsRequired();

            builder.Property(x => x.CancelledQuantity)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Ignore(x => x.Status);

            builder.Ignore(x => x.ActiveQuantity);

            builder.Ignore(x => x.PendingQuantity);

            builder.HasIndex("OrderId");

            builder.HasIndex(x => x.ProductId);
        }
    }
}