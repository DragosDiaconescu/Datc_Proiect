using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Company.Function
{
    public static class QueueTrigger_insert
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _ambrosiaTable;
        private static string _connectionString;
        private static async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _ambrosiaTable = _tableClient.GetTableReference("Ambrosia");
            await _ambrosiaTable.CreateIfNotExistsAsync();
        }

        [Function("QueueTrigger_insert")]
        public static void Run([QueueTrigger("ambrosia-insert-queue", Connection = "andreicristedatc6_STORAGE")] string myQueueItem,
            FunctionContext context)
        {
            TableQuery<AmbrosiaEntity> query;
            TableContinuationToken token;
            TableQuerySegment<AmbrosiaEntity> resultSegment;

            _connectionString = "DefaultEndpointsProtocol=https;AccountName=andreicristedatc6;AccountKey=uyiP8TPQVdP4D7lg5nFrusUVKjsTcwsDIuc22JgegGHJJT+ITp34+3AAkdPa+uS5F65CBylCYr8Bd1tMq0B9bg==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();
            var logger = context.GetLogger("QueueTrigger_insert");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var ambrosia = JsonConvert.DeserializeObject<AmbrosiaEntity>(myQueueItem);

            query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ambrosia.RowKey));
                                token = null;
                                resultSegment = Task.Run(async () =>{ return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
                                token = resultSegment.ContinuationToken;
                                if(resultSegment.Results.Count > 0)
                                    ambrosia.PartitionKey = (resultSegment.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString();
                                else
                                    ambrosia.PartitionKey = "0";
                                var insertOperation = TableOperation.Insert(ambrosia);
                                Task.Run(async () =>{  await _ambrosiaTable.ExecuteAsync(insertOperation);}).GetAwaiter().GetResult();
        }
    }
}
