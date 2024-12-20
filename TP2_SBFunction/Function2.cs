using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TP2_SBFunction
{
    public class Function2
    {
        [FunctionName("Function2")]
        public void Run([ServiceBusTrigger("messagequeue", Connection = "ConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

        }
    }
}
