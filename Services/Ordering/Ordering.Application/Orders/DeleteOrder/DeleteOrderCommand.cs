namespace Ordering.Application.Orders.DeleteOrder
{
    public record DeleteOrderCommand(int id) : Abstractions.ICommand;
}
