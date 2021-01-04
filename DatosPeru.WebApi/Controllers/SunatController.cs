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
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("admin/[controller]")]
    public class SunatController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;

        public SunatController(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Registrar o actualizar el tipo de cambio Consultando directamente a SUNAT(Solo Administradores)
        /// </summary>
        /// <param name="bodyRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<SunatTipoCambioDto>), (int)HttpStatusCode.OK)]
        [HttpPost("consultar-tipo-cambio", Name = "ConsultarTipoCambioSunat")]
        public async Task<IActionResult> Registrar(SunatTipoCambioRequestDto bodyRequest)
        {
            var _sunatTipoCambioClass = new SunatTipoCambioClass(_config);

            var _listTipoCambioSunat = await _sunatTipoCambioClass.Consultar(bodyRequest);

            return Ok(_listTipoCambioSunat);
        }

        ///// <summary>
        ///// Registrar o actualizar el tipo de cambio SUNAT(Solo Administradores)
        ///// </summary>
        ///// <param name="listTipoCambioSunat"></param>
        ///// <returns></returns>
        //[HttpPost("registrar-tipo-cambio", Name ="RegistrarTipoCambioSunat")]
        //public async Task<IActionResult> Registrar([FromBody] List<SunatTipoCambioDto> listTipoCambioSunat)
        //{
        //    var _sunatTipoCambioClass = new SunatTipoCambioClass(_config);

        //    await _sunatTipoCambioClass.Registrar(listTipoCambioSunat);

        //    return Ok();
        //}
    }
}
