using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

namespace TP2_FUNCTION_APP
{
    public class Function1
    {
        private readonly string connectionString = "Endpoint=sb://tp2servicebus1.servicebus.windows.net/;SharedAccessKeyName=Policy;SharedAccessKey=e3pBeWHlH9YfVboa5PZAGercrlOT7AjjV+ASbHcxe0M=;EntityPath=messagequeue";
        private readonly string queueName = "messagequeue";

        [FunctionName("Function1")]
        public async Task Run([BlobTrigger("blob/{name}", Connection = "ConnectionString")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            // Envoi d'un message à la file d'attente Service Bus
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);

            ServiceBusMessage message = new ServiceBusMessage(name);
            await sender.SendMessageAsync(message);

            log.LogInformation($"Message sent to Service Bus queue: {queueName}");
        }
    }
}
