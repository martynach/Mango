using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer: IAzureServiceBusConsumer
{
    private readonly IConfiguration _configuration;

    private readonly EmailService _emailService;

    // private readonly AppDbContext _dbContext;
    private readonly string _connectionString;
    private readonly string _emailCartQueue;
    private readonly string _registerUserQueue;
    private readonly ServiceBusProcessor _cartEmailProcessor;
    private readonly ServiceBusProcessor _registeredUserProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
        // _dbContext = dbContext;
        _emailCartQueue = _configuration.GetValue<string>("QueueAndTopicNames:ShoppingCartEmailQueue");
        _registerUserQueue = _configuration.GetValue<string>("QueueAndTopicNames:RegisteredUsersQueue");
        _connectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

        var client = new ServiceBusClient(_connectionString);
        _cartEmailProcessor = client.CreateProcessor(_emailCartQueue);
        _registeredUserProcessor = client.CreateProcessor(_registerUserQueue);
    }

    public async Task Start()
    {
        _cartEmailProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _cartEmailProcessor.ProcessErrorAsync += ErrorHandler;
        await _cartEmailProcessor.StartProcessingAsync();
        
        _registeredUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
        _registeredUserProcessor.ProcessErrorAsync += ErrorHandler;
        await _registeredUserProcessor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine("  - - - -    ");
        Console.WriteLine("Error when receiving message from the queue: ");
        Console.WriteLine("   - - -  " + args.Exception + " - - -     ");
        Console.WriteLine("  - - - -    ");
        return Task.CompletedTask;
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
    {
        // this is where we receive a message
        Console.WriteLine(" - - - - - - - - - - -  OnEmailCartRequestReceived - - - - - - - - - - - ");
        var message = args.Message;
        var stringMessage = Encoding.UTF8.GetString(message.Body);
        Console.WriteLine($"   message: {stringMessage}");


        var cartDto = JsonConvert.DeserializeObject<CartDto>(stringMessage);

        try
        {
            await _emailService.EmailCartAndLog(cartDto);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - - - - - - - - - - -  OnEmailCartRequestReceived exception: {ex}");
            throw;
        }
    }
    
    private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
    {
        // this is where we receive a message
        Console.WriteLine(" - - - - - - - - - - -  OnEmailRegisteredUserRequestReceived - - - - - - - - - - - ");
        var message = args.Message;
        var stringMessage = Encoding.UTF8.GetString(message.Body);
        var email = JsonConvert.DeserializeObject<string>(stringMessage);
        Console.WriteLine($"   stringMessage: {stringMessage}; email: {email}");

        try
        {
            await _emailService.RegisterUserEmailAndLog(stringMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - - - - - - - - - - -  OnEmailRegisteredUserRequestReceived exception: {ex}");
            throw;
        }
    }

    public async Task Stop()
    {
        // the commented is my implementation
        // _serviceBusProcessor.ProcessMessageAsync -= OnEmailCartRequestReceived;
        // _serviceBusProcessor.ProcessErrorAsync -= ErrorHandler;
        await _cartEmailProcessor.StopProcessingAsync();
        await _cartEmailProcessor.DisposeAsync();
        
        await _registeredUserProcessor.StopProcessingAsync();
        await _registeredUserProcessor.DisposeAsync();
    }
}