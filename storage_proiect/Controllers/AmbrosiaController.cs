using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using storage_proiect.Ambrosia.Models;
using storage_proiect.Ambrosia.Repository;
using Newtonsoft.Json.Linq;
using System;

namespace storage_proiect.Ambrosia.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AmbrosiaController : ControllerBase
    {
        private IAmbrosiaRepository _ambrosiasRepository;

        public AmbrosiaController(IAmbrosiaRepository ambrosiasRepository)
        {
            _ambrosiasRepository = ambrosiasRepository;
        }


        [HttpGet]
        public async Task<List<AmbrosiaEntity>> Get()
        {
            return await _ambrosiasRepository.GetAllAmbrosia();
        }

        [HttpPost]
        public async Task Post([FromBody] AmbrosiaEntity ambrosia)
        {
            await _ambrosiasRepository.InsertNewAmbrosia(ambrosia);
        }

        [HttpPatch]
        public async Task Patch([FromBody] Object ambrosia)
        {
            await _ambrosiasRepository.EditAmbrosia(JObject.Parse(ambrosia.ToString()));
        }
        [HttpDelete]
        public async Task Delete([FromBody] Object ambrosia)
        {
            await _ambrosiasRepository.DeleteAmbrosia(JObject.Parse(ambrosia.ToString()));
        }
    }
}
