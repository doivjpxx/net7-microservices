using System.Text;
using MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

public class MessageProducer : IMessageProducer
{
    public async Task PublishMessage(object message, string topicName)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(topicName, false, false, false, null);

        var messageJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(messageJson);

        channel.BasicPublish("", topicName, null, body);

        Console.WriteLine($"Message published to topic {topicName}");

        connection.Dispose();
    }
}