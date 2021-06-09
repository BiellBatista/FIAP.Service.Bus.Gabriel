using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.Service.Bus.Producer
{
    public class Program
    {
        private const string QueueConnectionString = "Endpoint=sb://gabrielbatistafiap.servicebus.windows.net/;SharedAccessKeyName=ProductPolicy;SharedAccessKey=UkjV63E0MCwFTMtn5h6zlhVULaJdnRdh9IXEyE0xH9s=";
        private const string QueuePath = "productchanged";
        private static IQueueClient _queueClient;

        private static void Main(string[] args)
        {
            SendMessagesAsync().GetAwaiter().GetResult();
            Console.WriteLine("messages were sent");
            Console.ReadLine();
        }

        private static async Task SendMessagesAsync()
        {
            _queueClient = new QueueClient(QueueConnectionString, QueuePath);

            var messages = "Hi,Hello,Hey,How are you,Be Welcome"
                .Split(",")
                .Select(msg =>
                {
                    Console.WriteLine($"Will send message: {msg}");
                    return new Message(Encoding.UTF8.GetBytes(msg));
                })
                .ToList();

            var sendTask = _queueClient.SendAsync(messages);

            await sendTask;
            CheckCommunicationExceptions(sendTask);

            var closeTask = _queueClient.CloseAsync();

            await closeTask;
            CheckCommunicationExceptions(closeTask);
        }

        private static bool CheckCommunicationExceptions(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList()
                .ForEach(innerException =>
                {
                    Console.WriteLine($"Error in SendAsync task: { innerException.Message}. Details: { innerException.StackTrace}");

                    if (innerException is ServiceBusCommunicationException)
                        Console.WriteLine("Connection Problem with Host");
                });

            return false;
        }
    }
}