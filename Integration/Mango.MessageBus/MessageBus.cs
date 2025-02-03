using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Mango.MessageBus;

public class MessageBus: IMessageBus
{
    private string connectionString = "todo";
    public async Task PublishMessage(object message, string topicOrQueueName)
    {
        Console.WriteLine("PublishMessage method: start");
        
        await using var client = new ServiceBusClient(connectionString);
        // var client = new ServiceBusClient(connectionString);
        Console.WriteLine("PublishMessage method: successfully created ServiceBusClient");
        
        var stringMessage = JsonConvert.SerializeObject(message);
        // var serviceBusMessage = new ServiceBusMessage(stringMessage);
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(stringMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };
        Console.WriteLine("PublishMessage method: successfully created ServiceBusMessage");

        var sender = client.CreateSender(topicOrQueueName);
        Console.WriteLine("PublishMessage method: successfully created ServiceBusSender");

        await sender.SendMessageAsync(serviceBusMessage);
        Console.WriteLine($"SendEmail method: message successfully sent to service bus queue: {topicOrQueueName}");

        // await client.DisposeAsync();

    }
}