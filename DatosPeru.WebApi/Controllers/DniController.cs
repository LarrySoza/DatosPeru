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
    public class DniController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;

        public DniController(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Consultar dni individual
        /// </summary>
        /// <param name="dni"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DniDto), (int)HttpStatusCode.OK)]
        [HttpGet("{dni}", Name = "ConsultarDni")]
        public async Task<IActionResult> Consultar(string dni)
        {
            var _dniClass = new DniClass(_config);

            var _dniExistente = await _dniClass.Consultar(dni);

            if (_dniExistente == null)
            {
                _dniExistente = await _dniClass.ConsultarJne(dni);

                if (_dniExistente == null)
                {
                    return NotFound(null);
                }
            }

            return Ok(_dniExistente);
        }

        /// <summary>
        /// Consulta dni multiple (100 max)
        /// </summary>
        /// <param name="dnis"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ConsultarDniMultipleDto), (int)HttpStatusCode.OK)]
        [HttpPost(Name = "ConsultarDniMultiple")]
        public async Task<IActionResult> Consultar([FromBody] List<string> dnis)
        {
            if (dnis == null)
                throw new ApiException("No se envio un objeto valido");

            if (dnis.Count > 100)
                throw new ApiException("Solo se permite maximo 100 dnis a consultar");

            var _dniClass = new DniClass(_config);

            var _dnisInfo = await _dniClass.Consultar(dnis);

            return Ok(_dnisInfo);
        }
    }
}
