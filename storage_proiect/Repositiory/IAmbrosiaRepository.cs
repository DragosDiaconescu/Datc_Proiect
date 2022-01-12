using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using storage_proiect.Ambrosia.Models;

namespace storage_proiect.Ambrosia.Repository
{
    public interface IAmbrosiaRepository
    {
        Task<List<AmbrosiaEntity>> GetAllAmbrosia();

        Task InsertNewAmbrosia(AmbrosiaEntity ambrosia);

        Task EditAmbrosia(JObject ambrosia);

        Task DeleteAmbrosia(JObject ambrosia);
    }
}