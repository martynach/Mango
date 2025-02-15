using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer _serviceBusConsumer;
    public static WebApplicationBuilder UseAzureServiceBusConsumer(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        _serviceBusConsumer = app.ApplicationServices.GetRequiredService<IAzureServiceBusConsumer>();
        var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        hostApplicationLifetime.ApplicationStarted.Register(OnStart);
        hostApplicationLifetime.ApplicationStopping.Register(OnStop);
        
        return app;
    }

    private static void OnStart()
    {
        _serviceBusConsumer.Start();
    }
    
    private static void OnStop()
    {
        _serviceBusConsumer.Stop();
    }
}