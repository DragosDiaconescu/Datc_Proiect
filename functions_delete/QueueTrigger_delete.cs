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
    public static class QueueTrigger_delete
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

        [Function("QueueTrigger_delete")]
        public static void Run([QueueTrigger("ambrosia-delete-queue", Connection = "andreicristedatc6_STORAGE")] string myQueueItem,
            FunctionContext context)
        {
            TableQuery<AmbrosiaEntity> query;
            TableContinuationToken token;
            TableQuerySegment<AmbrosiaEntity> resultSegment;

            _connectionString = "DefaultEndpointsProtocol=https;AccountName=andreicristedatc6;AccountKey=uyiP8TPQVdP4D7lg5nFrusUVKjsTcwsDIuc22JgegGHJJT+ITp34+3AAkdPa+uS5F65CBylCYr8Bd1tMq0B9bg==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();
            var logger = context.GetLogger("QueueTrigger_delete");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var ambrosia = JsonConvert.DeserializeObject<JObject>(myQueueItem);
            query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ambrosia["RowKey"].ToString()), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ambrosia["PartitionKey"].ToString())));
                                token = null;
                                resultSegment = Task.Run(async() => { return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token); }).GetAwaiter().GetResult();
                                token = resultSegment.ContinuationToken;
                                var deleteOperation = TableOperation.Delete(resultSegment.Results[0]);
                                Task.Run(async () => {await _ambrosiaTable.ExecuteAsync(deleteOperation);}).GetAwaiter().GetResult();
        }
    }
}
