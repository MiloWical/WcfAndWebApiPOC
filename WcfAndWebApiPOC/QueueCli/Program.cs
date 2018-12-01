using System;

namespace QueueCli
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Configuration;

    class Program
    {
        private static QueueClient _queueClient;

        static void Main(string[] args)
        {
            SetupQueueClient();

            PrintHeader();

            var operation = Menu();

            while (operation != Operation.Quit)
            {
                Console.WriteLine();

                string payload = null;

                switch (operation)
                {
                    case Operation.AbsoluteValue:
                        payload = ReadSingleValue();
                        break;
                    case Operation.Sum:
                    case Operation.Product:
                        payload = ReadMultipleValues();
                        break;
                    case Operation.Quit:
                        break;
                    default:
                        Console.WriteLine("Illegal entry.");
                        break;
                }

                if(!string.IsNullOrWhiteSpace(payload))
                    SendMessage(operation, payload);

                operation = Menu();
            }
        }

        static void SetupQueueClient()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appconfig.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var connectionString = configuration["ConnectionStrings:ServiceBus"];
            var queueName = configuration["Queue"];

            _queueClient = new QueueClient(connectionString, queueName);
        }

        static void PrintHeader()
        {
            Console.WriteLine("Number Processing Service Queue Interface" + Environment.NewLine + "-----------------------------------------" + Environment.NewLine);
        }

        static Operation Menu()
        {
            Console.WriteLine();

            foreach (int value in Enum.GetValues(typeof(Operation)))
            {
                Console.WriteLine($"{value}) {Enum.GetName(typeof(Operation), value)}");
            }

            Console.WriteLine();
            Console.Write("> ");

            Operation operation;

            try
            {
                operation = Enum.Parse<Operation>(Console.ReadLine());
            }
            catch (ArgumentException)
            {
                operation = 0;
            }

            return operation;
        }

        static void SendMessage(Operation operation, string payload)
        {
            var message = new Message
            {
                UserProperties = {["Operation"] = operation.ToString()},
                Body = Encoding.UTF8.GetBytes(payload)
            };

            _queueClient.SendAsync(message).Wait();
        }

        static string ReadSingleValue()
        {
            Console.Write("Type a number (enter to return): ");
            return Console.ReadLine();
        }

        static string ReadMultipleValues()
        {
            var numberList = new List<string>();

            string number;

            do
            {
                number = ReadSingleValue();

                if(!string.IsNullOrWhiteSpace(number))
                    numberList.Add(number);

            } while (!string.IsNullOrWhiteSpace(number));

            var builder = new StringBuilder();
            builder.AppendJoin(',', numberList);

            return builder.ToString();
        }

        enum Operation
        {
            AbsoluteValue = 1,
            Sum = 2,
            Product = 3,
            Quit = 4
        }
    }
}
