using DineFlow.Domain;
using DineFlow.Domain.Entities;
using DineFlow.Domain.Entities.OrderItems;
using DineFlow.Domain.Entities.Orders;
using DineFlow.Infrastructure.Persistence;
using DineFlow.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DineFlow.Tests.Infrastructure
{
    public class OrderItemConcurrencyTests
    {
        [Fact]
        public async Task SaveChanges_Should_Throw_DbUpdateConcurrencyException_When_OrderItem_RowVersion_Is_Stale()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<TestAppDbContext>()
                .UseSqlite(connection)
                .Options;

            Guid orderId;
            Guid itemId;

            // create schema and seed
            using (var init = new TestAppDbContext(options))
            {
                await init.Database.EnsureCreatedAsync();

                var repo = new OrderRepository(init);
                var order = new Order(OrderType.Takeaway, null);
                var item = new OrderItem(Guid.NewGuid(), "Pizza", 8m, 1);
                itemId = item.Id;
                order.AddItem(item);
                orderId = order.Id;

                await repo.AddAsync(order);
                await repo.SaveChangesAsync();
            }

            using var c1 = new TestAppDbContext(options);
            using var c2 = new TestAppDbContext(options);

            var r1 = new OrderRepository(c1);
            var r2 = new OrderRepository(c2);

            var o1 = await r1.GetByIdAsync(orderId);
            var o2 = await r2.GetByIdAsync(orderId);

            o1.Should().NotBeNull();
            o2.Should().NotBeNull();

            // submit before kitchen work (domain invariant)
            o1!.Submit();
            o2!.Submit();

            // first context updates item and saves
            o1.StartItemPreparation(itemId, 1);
            await r1.SaveChangesAsync();

            // second context makes a conflicting update with stale rowversion
            o2.StartItemPreparation(itemId, 1);

            Func<Task> act = async () => await r2.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }
    }
}