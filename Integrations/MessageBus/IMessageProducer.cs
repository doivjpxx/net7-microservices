namespace MessageBus;

public interface IMessageProducer
{
    Task PublishMessage(object message, string topicName);
}