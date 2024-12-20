using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TP2_SBFunction
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("messagequeue", Connection = "Endpoint=sb://tp2servicebus1.servicebus.windows.net/;SharedAccessKeyName=Policy;SharedAccessKey=e3pBeWHlH9YfVboa5PZAGercrlOT7AjjV+ASbHcxe0M=;EntityPath=messagequeue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
