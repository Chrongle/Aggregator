using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "VegetableWeightExchange", type: ExchangeType.Direct);

    var carrotQueue = channel.QueueDeclare().QueueName;
    var potatoQueue = channel.QueueDeclare().QueueName;

    channel.QueueBind(queue: carrotQueue, exchange: "VegetableWeightExchange", routingKey: "carrot");
    channel.QueueBind(queue: potatoQueue, exchange: "VegetableWeightExchange", routingKey: "potato");

    var totalWeight = 0;
    var veggieCounts = new Dictionary<string, int> { { "carrot", 0 }, { "potato", 0 } };
    var veggieWeights = new Dictionary<string, int> { { "carrot", 0 }, { "potato", 0 } };
    var threshold = 50;

    void ProcessMessage(string veggieType, int weight)
    {
        veggieWeights[veggieType] += weight;
        veggieCounts[veggieType]++;
        totalWeight += weight;

        Console.WriteLine($"[Aggregator] Current total weight: {totalWeight}g, {veggieType} count: {veggieCounts[veggieType]}, weight: {veggieWeights[veggieType]}g");

        if (totalWeight >= threshold)
        {
            SendAggregatedData();
            ResetTotals();
        }
    }

    void SendAggregatedData()
    {
        using (var innerChannel = connection.CreateModel())
        {
            innerChannel.QueueDeclare(queue: "AggregatedWeightQueue", durable: false, exclusive: false, autoDelete: false);

            var message = $"carrot:{veggieCounts["carrot"]}:{veggieWeights["carrot"]}|potato:{veggieCounts["potato"]}:{veggieWeights["potato"]}";
            var body = Encoding.UTF8.GetBytes(message);

            innerChannel.BasicPublish(exchange: "", routingKey: "AggregatedWeightQueue", basicProperties: null, body: body);
            Console.WriteLine($"[Aggregator] Sent aggregated data: {message}");
        }
    }

    void ResetTotals()
    {
        totalWeight = 0;
        veggieCounts["carrot"] = 0;
        veggieCounts["potato"] = 0;
        veggieWeights["carrot"] = 0;
        veggieWeights["potato"] = 0;
    }

    var carrotConsumer = new EventingBasicConsumer(channel);
    carrotConsumer.Received += (model, ea) =>
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        var weight = int.Parse(message.Split(':')[1]);
        ProcessMessage("carrot", weight);
    };

    var potatoConsumer = new EventingBasicConsumer(channel);
    potatoConsumer.Received += (model, ea) =>
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        var weight = int.Parse(message.Split(':')[1]);
        ProcessMessage("potato", weight);
    };

    channel.BasicConsume(queue: carrotQueue, autoAck: true, consumer: carrotConsumer);
    channel.BasicConsume(queue: potatoQueue, autoAck: true, consumer: potatoConsumer);

    Console.WriteLine(" [*] Aggregator waiting for weights...");
    Console.ReadLine();
}