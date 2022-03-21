using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

double _totalHold = 0;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "sms-queue",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    channel.QueueBind(queue: "sms-queue",
                      exchange: "notifier",
                      routingKey: string.Empty);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (sender, e) =>
    {
        var body = e.Body;
        var message = Encoding.UTF8.GetString(body.ToArray());

        var payment = GetPayment(message);
        _totalHold += (payment - (payment * 0.1));

        Console.WriteLine($"Payment received for the amount of {payment}");
        Console.WriteLine($"${_totalHold} total hold after tax");
    };

    channel.BasicConsume(queue: "sms-queue",
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine($"Subscribed to the queue `sms-queue`");
    Console.WriteLine("Listening . . .");

    Console.ReadLine();
}


static int GetPayment(string message)
{
    var messageWords = message.Split(' ');

    return int.Parse(messageWords[^1]);
}