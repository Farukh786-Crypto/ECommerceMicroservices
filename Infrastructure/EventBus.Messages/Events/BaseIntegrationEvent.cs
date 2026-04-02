namespace EventBus.Messages.Events
{
    public class BaseIntegrationEvent
    {
        public Guid CorrelationId { get; set; } // You can trace entire flow of event
        public DateTime CreationDate { get; set; } // when event is created
        public BaseIntegrationEvent()
        {
            CorrelationId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public BaseIntegrationEvent(Guid correlationId,DateTime creationDate)
        {
            CorrelationId=correlationId;
            CreationDate = creationDate;
        }
    }
}
