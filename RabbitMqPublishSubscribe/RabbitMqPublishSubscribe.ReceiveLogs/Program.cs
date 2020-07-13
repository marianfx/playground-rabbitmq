using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMqPublishSubscribe.ReceiveLogs
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
                    // generate random queue name
                    var queueName = channel.QueueDeclare().QueueName;

                    // bind this random queue to this worker, and add it in the 'logs' exchange
                    // logs will publish to all bound queues
                    channel.QueueBind(queue: queueName,
                                      exchange: "logs",
                                      routingKey: "");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: queueName,
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
