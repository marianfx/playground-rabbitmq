using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMqIntro.Receive
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
                    // must be the same channel as declared in the publisher
                    channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: "hello",
                                        autoAck: true,
                                        consumer: consumer);

                    // note: block the thread so the context is intact (wait)
                    Console.WriteLine("This one will keep listening. Press [enter] to exit.");
                    Console.ReadLine();
                }
            }

        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body); // get as string; you could deserialize with json eventually
            Console.WriteLine($"Received message: {message}.");
        }
    }
}
