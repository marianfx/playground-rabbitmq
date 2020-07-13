using EasyNetQ;
using EasyNetQIntro.Messages;
using System;

namespace EasyNetQIntro.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                while(true)
                {
                    var message = GetConsoleMessage();
                    if (message == "Quit")
                        return;

                    bus.Publish(new TextMessage() { Text = message });
                }
            }
        }

        private static string GetConsoleMessage()
        {
            var input = "";
            Console.WriteLine("Enter a message. [Enter] to send. [Quit] to quit.");
            return Console.ReadLine();
        }
    }
}
