using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Drawing;
using System.Collections.Immutable;
using Rabbit.Common.Data.Weather;

namespace Rabbit.Example1.Producer
{
    internal sealed class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("\nEXAMPLE 1 : ONE-WAY MESSAGING : PRODUCER");

            const string ExchangeName = "";
            const string QueueName = "example1_trades_queue";

            var connectionFactory = new ConnectionFactory
            {
                HostName = "172.16.0.207",
                // UserName = "admin",
                // Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            while (true)
            {
                var forecast =  Thermometer.Fake().Report();

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: QueueName,
                    body: forecast.ToBytes()
                );

      Console.WriteLine(forecast.ToJson());

                await Task.Delay(millisecondsDelay: 5000);
            }
        }
    }
}
