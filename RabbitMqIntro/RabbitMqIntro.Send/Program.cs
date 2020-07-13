using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqIntro.Send
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
                    // declare the queue (idempotent -> same action always; if it exists, it uses it)
                    channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    // we can send only bytes
                    var message = "Hello haters!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                        routingKey: "hello",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine($"[x] Sent {message}.");
                }
            }
            // here, the publisher does not keep the connection anymore, everything is disposed (but the broker is there)
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
