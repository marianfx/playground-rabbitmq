using EasyNetQ;
using EasyNetQIntro.Messages;
using System;

namespace EasyNetQIntro.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<TextMessage>("test", message =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Got message {message.Text}.");
                    Console.ResetColor();
                });

                Console.WriteLine("Waiting for messages");
                Console.ReadKey();
            }
        }
    }
}
