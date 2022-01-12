using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace storage_proiect.Ambrosia.Models
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