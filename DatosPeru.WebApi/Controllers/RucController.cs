using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DatosPeru.WebApi.Dto;
using DatosPeru.WebApi.Entities;
using DatosPeru.WebApi.Infrastructure;
using DatosPeru.WebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DatosPeru.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("datos-peru/[controller]")]
    public class RucController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;

        public RucController(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Consultar ruc individual
        /// </summary>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(RucDto), (int)HttpStatusCode.OK)]
        [HttpGet("{ruc}", Name = "ConsultarRuc")]
        public async Task<IActionResult> Consultar(string ruc)
        {
            var _rucClass = new RucClass(_config);

            var _rucExistente = await _rucClass.Consultar(ruc);

            if (_rucExistente == null) return NotFound(null);

            return Ok(_rucExistente);
        }

        /// <summary>
        /// Consulta ruc multiple (100 max)
        /// </summary>
        /// <param name="rucs"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ConsultarRucMultipleDto), (int)HttpStatusCode.OK)]
        [HttpPost(Name = "ConsultarRucMultiple")]
        public async Task<IActionResult> Consultar([FromBody] List<string> rucs)
        {
            if (rucs == null)
                throw new ApiException("No se envio un objeto valido");

            if (rucs.Count > 100)
                throw new ApiException("Solo se permite maximo 100 rucs a consultar");

            var _rucClass = new RucClass(_config);

            var _rucsInfo = await _rucClass.Consultar(rucs);

            return Ok(_rucsInfo);
        }
    }
}
