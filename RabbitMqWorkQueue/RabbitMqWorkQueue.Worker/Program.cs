using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMqWorkQueue.Worker
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
                    // must be the same channel as declared in the publisher
                    channel.QueueDeclare(queue: QUEUE_NAME,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    // set prefetch to 1, so it assigns 1 job per busy worker
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: QUEUE_NAME,
                                        autoAck: false,
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

            // sleep how many seconds how many dots
            int dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 1000);

            Console.WriteLine(" [x] Finished processing");
            var channel = ((EventingBasicConsumer)sender).Model;
            channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }
    }
}
