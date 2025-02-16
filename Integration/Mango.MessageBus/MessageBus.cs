using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Mango.MessageBus;

public class MessageBus: IMessageBus
{
    private string connectionString = "todo";
    public async Task PublishMessage(object message, string topicOrQueueName)
    {
        // var client = new ServiceBusClient(connectionString);
        await using var client = new ServiceBusClient(connectionString);
        Console.WriteLine("PublishMessage method: client created successfully");

        var stringMessage = JsonConvert.SerializeObject(message);
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(stringMessage));
        Console.WriteLine("PublishMessage method: serviceBusMessage created successfully");

        var sender = client.CreateSender(topicOrQueueName);
        Console.WriteLine("PublishMessage method: sender created successfully");

        await sender.SendMessageAsync(serviceBusMessage);
        Console.WriteLine("PublishMessage method: message sent successfully");

        // await client.DisposeAsync();
    }
}