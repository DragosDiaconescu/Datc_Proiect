using Microsoft.Azure.Cosmos.Table;
using System;

namespace Company.Function
{
    public class AmbrosiaEntity : TableEntity
    {
        public AmbrosiaEntity(string phoneNumber)
        {
            this.RowKey = phoneNumber;
        }

        public AmbrosiaEntity() { }

        public double latitude { get; set; }

        public double longitude { get; set; }

    }
}