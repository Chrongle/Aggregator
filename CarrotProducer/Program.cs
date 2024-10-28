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

    for (int i = 0; i < 20; i++)
    {
        var weight = random.Next(1, 10);
        var message = $"carrot:{weight}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "VegetableWeightExchange", routingKey: "carrot", basicProperties: null, body: body);
        Console.WriteLine($"[CarrotProducer] Sent weight data: {weight}g");

        Thread.Sleep(1000);
    }
}