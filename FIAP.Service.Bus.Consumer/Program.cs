using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FIAP.Service.Bus.Consumer
{
    public class Program
    {
        private const string QueueConnectionString = "Endpoint=sb://gabrielbatistafiap.servicebus.windows.net/;SharedAccessKeyName=ProductPolicy;SharedAccessKey=UkjV63E0MCwFTMtn5h6zlhVULaJdnRdh9IXEyE0xH9s=";
        private const string QueuePath = "productchanged";
        private static IQueueClient _queueClient;

        private static void Main(string[] args)
        {
            ReceiveMessagesAsync().GetAwaiter().GetResult();
        }

        private static async Task ReceiveMessagesAsync()
        {
            _queueClient = new QueueClient(QueueConnectionString, QueuePath);
            _queueClient.RegisterMessageHandler(MessageHandler,
                new MessageHandlerOptions(ExceptionHandler) { AutoComplete = false });
            Console.ReadLine();
            await _queueClient.CloseAsync();
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionArgs.Exception}.");
            var context = exceptionArgs.ExceptionReceivedContext;
            Console.WriteLine($"Endpoint:{context.Endpoint}, Path:{context.EntityPath}, Action:{context.Action}");
            return Task.CompletedTask;
        }

        private static async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received message: { Encoding.UTF8.GetString(message.Body)}");

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}