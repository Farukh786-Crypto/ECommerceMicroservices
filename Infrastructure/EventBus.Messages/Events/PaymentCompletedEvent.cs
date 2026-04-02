namespace EventBus.Messages.Events
{
    public class PaymentCompletedEvent : BaseIntegrationEvent
    {
        public int OrderId { get; set; }
        public string? UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
