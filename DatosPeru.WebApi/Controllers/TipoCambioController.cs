using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DatosPeru.WebApi.Dto;
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
    public class TipoCambioController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;

        public TipoCambioController(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Consultar tipo de cambio individual
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <param name="dia"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(TipoCambioDto), (int)HttpStatusCode.OK)]
        [HttpGet("{anio}/{mes}/{dia}", Name = "ConsultarTipoCambio")]
        public async Task<IActionResult> Consultar(int anio, int mes,int dia)
        {
            var _tipoCambioClass = new TipoCambioClass(_config);

            var _tipoCambioExistente = await _tipoCambioClass.Consultar(anio, mes, dia);

            if (_tipoCambioExistente == null) return NotFound(null);

            return Ok(_tipoCambioExistente);
        }

        /// <summary>
        /// Consultar tipo de cambio por mes
        /// </summary>
        /// <param name="anio"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<TipoCambioDto>), (int)HttpStatusCode.OK)]
        [HttpGet("{anio}/{mes}", Name = "ListarTipoCambioMes")]
        public async Task<IActionResult> Consultar(int anio, int mes)
        {
            var _tipoCambioClass = new TipoCambioClass(_config);

            var _tipoCambioExistente = await _tipoCambioClass.Consultar(anio, mes);

            return Ok(_tipoCambioExistente);
        }
    }
}
