using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastucture.Data;

namespace Ordering.Infrastucture.Repositories
{
    public class OrderRepository : RepositoryBase<Order> , IOrderRepository
    {
        public OrderRepository(OrderContext orderContext) : base(orderContext) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            var orderList = await _orderContext.Orders.AsNoTracking().Where(o=>o.UserName == userName).ToListAsync();
            return orderList;
        }
        // order get message save in outbox
        public async Task AddOutboxMessageAsync(OutboxMessage outboxMessage)
        {
            await _orderContext.OutboxMessage.AddAsync(outboxMessage);
            await _orderContext.SaveChangesAsync();
        }
    }
    
}
