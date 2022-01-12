using Microsoft.Azure.Cosmos.Table;

namespace webJob_proiect
{
    public class MetricEntity : TableEntity
    {
        public MetricEntity(string metric)
        {
            this.RowKey = metric;
        }

        public MetricEntity() { }

        public int zones{get;set;}
        
    }
}