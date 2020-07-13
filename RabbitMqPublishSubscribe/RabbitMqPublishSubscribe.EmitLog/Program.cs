using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqPublishSubscribe.EmitLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // declare the exchange (fanout = send to all queues for exchange)
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    // we can send only bytes
                    var message = GetMessageFromArgs(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "logs",
                                        routingKey: "",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine($"[x] Sent {message}.");
                }
            }
            // here, the publisher does not keep the connection anymore, everything is disposed (but the broker is there)
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessageFromArgs(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello haters!");
        }
    }
}
