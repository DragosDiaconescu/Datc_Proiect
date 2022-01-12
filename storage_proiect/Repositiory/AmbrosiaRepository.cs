using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using storage_proiect.Ambrosia.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Queues;

namespace storage_proiect.Ambrosia.Repository
{
    public class AmbrosiaRepository : IAmbrosiaRepository
    {
        private string _connectionString;

        private CloudTableClient _tableClient;

        private CloudTable _ambrosiaTable;

        public AmbrosiaRepository(IConfiguration configuration)
        {
            Task.Run(async () => { await InitializeTable(configuration); }).GetAwaiter().GetResult();

        }
        public async Task InsertNewAmbrosia(AmbrosiaEntity ambrosia)
        {
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-insert-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String); 
        }
        
        public async Task InitializeTable(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "AzureStorageConnectionString").ToString();
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _ambrosiaTable = _tableClient.GetTableReference("Ambrosia");
            await _ambrosiaTable.CreateIfNotExistsAsync();
        }

        public async Task<List<AmbrosiaEntity>> GetAllAmbrosia()
        {
            var ambrosias = new List<AmbrosiaEntity>();
            TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<AmbrosiaEntity> resultSegment = await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                ambrosias.AddRange(resultSegment);
            } while (token != null);
            return ambrosias;
        }
        public async Task EditAmbrosia(JObject ambrosia)
        {
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);

            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String); 
        }
        public async Task DeleteAmbrosia(JObject ambrosia)
        {
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);

            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-delete-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String);
        }
    }
}