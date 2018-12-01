namespace NumberService.WebAPI.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using Components;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Configuration;

    public class QueueNumberController
    {
        private INumberProcessor _processor;

        public QueueNumberController(IConfiguration config, INumberProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));

            var queueClient = new QueueClient(config["ConnectionString"], "test_queue", ReceiveMode.PeekLock,
                RetryPolicy.Default);

            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                Debug.WriteLine(Encoding.UTF8.GetString(message.Body));

                ProcessMessage(message);
                
                if(!message.SystemProperties.IsReceived)
                    await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            },
            async (exceptionArgs) =>
            {
                Debug.WriteLine(exceptionArgs.Exception.Message);
                await Task.CompletedTask;
            });
        }

        private void ProcessMessage(Message message)
        {
            var values = ByteArrayToIntArray(message.Body);
            int result;

            var operation = message.UserProperties["Operation"];

            switch (operation)
            {
                case "AbsoluteValue":
                    result = _processor.AbsoluteValue(values[0]);
                    break;
                case "Sum":
                    result = _processor.Sum(values);
                    break;
                case "Product":
                    result = _processor.Product(values);
                    break;
                default:
                    throw new NotSupportedException($"The supplied operation '{operation}' cannot be processed.");
            }

            Debug.WriteLine(result);
        }

        private static int[] ByteArrayToIntArray(byte[] data)
        {
            var intStrings = Encoding.UTF8.GetString(data).Split(',');

            var intArray = new int[intStrings.Length];

            for (var i = 0; i < intStrings.Length; i++)
                intArray[i] = int.Parse(intStrings[i]);

            return intArray;
        }
    }
}
