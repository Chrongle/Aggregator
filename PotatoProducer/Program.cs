using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "VegetableWeightExchange", type: ExchangeType.Direct);

    var random = new Random();

    for (int i = 0; i < 15; i++)
    {
        var weight = random.Next(5, 15);
        var message = $"potato:{weight}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "VegetableWeightExchange", routingKey: "potato", basicProperties: null, body: body);
        Console.WriteLine($"[PotatoProducer] Sent weight data: {weight}g");

        Thread.Sleep(1500);
    }
}