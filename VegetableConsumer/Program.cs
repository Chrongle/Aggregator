using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "AggregatedWeightQueue", durable: false, exclusive: false, autoDelete: false);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        var data = message.Split('|');

        foreach (var entry in data)
        {
            var parts = entry.Split(':');
            var veggieType = parts[0];
            var count = int.Parse(parts[1]);
            var totalWeight = int.Parse(parts[2]);

            Console.WriteLine($"[Consumer] Total for {veggieType}: {count} pieces, total weight: {totalWeight}g");
        }
    };

    channel.BasicConsume(queue: "AggregatedWeightQueue", autoAck: true, consumer: consumer);

    Console.WriteLine(" [*] Waiting for aggregated weights...");
    Console.ReadLine();
}