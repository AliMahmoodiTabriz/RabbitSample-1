using System;
using System.Collections.Immutable;
using Rabbit.Common.Data.Weather;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit.Example1.Consumer
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("\nEXAMPLE 1 : ONE-WAY MESSAGING : CONSUMER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "172.16.0.207",
                // UserName = "admin",
                // Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(
                queue: "example1_trades_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var messageBody = eventArgs.Body.ToArray();
                var forecast = Forecast.FromBytes(messageBody);
                Console.WriteLine(forecast.ToJson());
                
                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(
                queue: queue.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.ReadLine();
        }
    }
}
