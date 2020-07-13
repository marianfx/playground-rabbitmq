using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqWorkQueue.NewTask
{
    class Program
    {
        private static string QUEUE_NAME = "task_queue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // declare the queue (idempotent -> same action always; if it exists, it uses it)
                    channel.QueueDeclare(queue: QUEUE_NAME,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    // we can send only bytes
                    var message = GetMessageFromArgs(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    // assign properties (header)
                    //var properties = channel.CreateBasicProperties();
                    //properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                        routingKey: QUEUE_NAME,
                                        basicProperties: properties,
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
