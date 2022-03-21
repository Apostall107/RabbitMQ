using RabbitMQ.Client;
using System.Text;

Task.Run(CreateTask(3333, "error"));
Task.Run(CreateTask(2888, "info"));
Task.Run(CreateTask(2222, "warning"));
Task.Run(CreateTask(1666, "help"));

Console.ReadKey();


static Func<Task> CreateTask(int timeToSleepTo, string routingKey)
{
    return () =>
    {
        int counter = 0;
        do
        {
            int timeToSleep = new Random().Next(timeToSleepTo/2, timeToSleepTo);
            Thread.Sleep(timeToSleep);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                string message = $"Message type [{routingKey}] from publisher: No{counter++}";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"Message type [{routingKey}] has been sent to the Direct Exchange: [No:{counter}]");
            }

        } while (true);
    };
}

