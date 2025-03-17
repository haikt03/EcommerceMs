namespace EventBus.Message
{
    public interface IIntegrationEvent
    {
        DateTime CreationDate { get; }
        Guid Id { get; set; }
    }
}
