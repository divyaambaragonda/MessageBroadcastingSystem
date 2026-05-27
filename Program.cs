using MessageBroadcastingSystem.Application.Interfaces;
using MessageBroadcastingSystem.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IMessageBroker,
            MessageBroker>();

        services.AddSingleton<IMessageProducer,
            MessageProducer>();

        services.AddSingleton<IMessageConsumer>(
            sp => new MessageConsumer(
                "Consumer-1",
                sp.GetRequiredService<
                    ILogger<MessageConsumer>>()));

        services.AddSingleton<IMessageConsumer>(
            sp => new MessageConsumer(
                "Consumer-2",
                sp.GetRequiredService<
                    ILogger<MessageConsumer>>()));

        services.AddSingleton<IMessageConsumer>(
            sp => new MessageConsumer(
                "Consumer-3",
                sp.GetRequiredService<
                    ILogger<MessageConsumer>>()));

        services.AddSingleton<IMessageConsumer>(
            sp => new MessageConsumer(
                "Consumer-4",
                sp.GetRequiredService<
                    ILogger<MessageConsumer>>()));

        services.AddSingleton<IMessageConsumer>(
            sp => new MessageConsumer(
                "Consumer-5",
                sp.GetRequiredService<
                    ILogger<MessageConsumer>>()));

        services.AddSingleton<ISystemController,
            SystemController>();
    });

var host = builder.Build();

var broker =
    host.Services.GetRequiredService<IMessageBroker>();

var consumers =
    host.Services.GetServices<IMessageConsumer>();



var controller =
    host.Services.GetRequiredService<
        ISystemController>();

var producer =
    host.Services.GetRequiredService<
        IMessageProducer>();

Console.WriteLine("=== Message Broadcasting System ===");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Start Producer");
    Console.WriteLine("2. Send Manual Message");
    Console.WriteLine("3. Disconnect All");
    Console.WriteLine("4. Reconnect All");
    Console.WriteLine("5. Reset System");
    Console.WriteLine("6. Stop System");
    Console.WriteLine("7. Exit");
    Console.WriteLine("8. Subscribe All");

    Console.Write("Select: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await controller.StartAsync();
            break;

        case "2":
            Console.Write("Enter message: ");

            var msg = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await producer.SendManualMessageAsync(
                    msg,
                    CancellationToken.None);

                await controller.ResetAsync();
            }

            break;

        case "3":
            controller.DisconnectAll();
            break;

        case "4":
            controller.ReconnectAll();
            break;

        case "5":
            await controller.ResetAsync();
            break;

        case "6":
            await controller.StopAsync();
            break;

        case "7":
            await controller.StopAsync();
            return;

        case "8":
            foreach (var consumer in consumers)
            {
                broker.Subscribe(consumer);
            }
            break;
    }
}