using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace webJob_proiect
{
    class Program
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _metricsTable;
        private static string _connectionString;
        private static async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _metricsTable = _tableClient.GetTableReference("Metrics");
            await _metricsTable.CreateIfNotExistsAsync();
        }
        static void Main(string[] args)
        {
            TableQuery<MetricEntity> query;
            TableContinuationToken token;
            TableQuerySegment<MetricEntity> resultSegment;
            RestClient client;
            RestRequest request;

            IRestResponse response;
            request = new RestRequest(Method.GET);
            List<AmbrosiaEntity> ambrosias;
            client = new RestClient("https://smhdproiectdatc.azurewebsites.net/Ambrosia");
            client.Timeout = -1;

            response = client.Execute(request);
            ambrosias = JsonConvert.DeserializeObject<List<AmbrosiaEntity>>(response.Content);
           

            _connectionString = "DefaultEndpointsProtocol=https;AccountName=andreicristedatc6;AccountKey=uyiP8TPQVdP4D7lg5nFrusUVKjsTcwsDIuc22JgegGHJJT+ITp34+3AAkdPa+uS5F65CBylCYr8Bd1tMq0B9bg==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();

            MetricEntity metric = new MetricEntity("metric");

            query = new TableQuery<MetricEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "metric"));
            token = null;
            resultSegment = Task.Run(async () =>{ return await _metricsTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
            token = resultSegment.ContinuationToken;
            if(resultSegment.Results.Count > 0)
                metric.PartitionKey = (resultSegment.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString();
            else
                metric.PartitionKey = "0";
            metric.zones = ambrosias.Count();
            var insertOperation = TableOperation.Insert(metric);
            Task.Run(async () =>{  await _metricsTable.ExecuteAsync(insertOperation);}).GetAwaiter().GetResult();

            
        }
    }
}
