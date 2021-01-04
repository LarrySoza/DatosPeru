using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DatosPeru.WebApi.Dto;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RestSharp;

namespace DatosPeru.WebApi.Infrastructure
{
    public class SunatTipoCambioClass
    {
        private readonly IConfiguration _config;

        public SunatTipoCambioClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<SunatTipoCambioDto>> Consultar(SunatTipoCambioRequestDto bodyRequest)
        {
            var _client = new RestClient("https://e-consulta.sunat.gob.pe");

            var _request = new RestRequest("cl-at-ittipcam/tcS01Alias/listarTipoCambio")
                .AddJsonBody(bodyRequest);

            var _listTipoCambioSunat = await _client.PostAsync<IEnumerable<SunatTipoCambioDto>>(_request);

            if (_listTipoCambioSunat.Count() > 0)
            {
                await Registrar(_listTipoCambioSunat);
            };

            return _listTipoCambioSunat;
        }

        public async Task Registrar(IEnumerable<SunatTipoCambioDto> listTipoCambioSunat)
        {
            //Primero convertimos a una lista de objetos TipoCambioDto
            //No creo que sea la manera mas eficiente pero asumiendo que son unas 60-64 interacciones no importa
            var _listTipoCambio = new List<TipoCambioDto>();

            foreach (var itemSunat in listTipoCambioSunat)
            {
                var _split = itemSunat.fecPublica.Split('/');

                var _fecha = new DateTime(Convert.ToInt32(_split[2]), Convert.ToInt32(_split[1]), Convert.ToInt32(_split[0]));

                var _anio = _fecha.Year;
                var _mes = _fecha.Month;
                var _dia = _fecha.Day;

                bool _existe = false;

                foreach (var itemExistente in _listTipoCambio)
                {
                    if (itemExistente.anio == _anio &&
                        itemExistente.mes == _mes &&
                        itemExistente.dia == _dia)
                    {
                        if (itemSunat.codTipo == "C")
                        {
                            itemExistente.compra = Convert.ToDecimal(itemSunat.valTipo);
                        }

                        if (itemSunat.codTipo == "V")
                        {
                            itemExistente.venta = Convert.ToDecimal(itemSunat.valTipo);
                        }

                        _existe = true;
                    }
                }

                if (!_existe)
                {
                    var _tipoCambio = new TipoCambioDto()
                    {
                        anio = _anio,
                        mes = _mes,
                        dia = _dia
                    };

                    if (itemSunat.codTipo == "C")
                    {
                        _tipoCambio.compra = Convert.ToDecimal(itemSunat.valTipo);
                    }

                    if (itemSunat.codTipo == "V")
                    {
                        _tipoCambio.venta = Convert.ToDecimal(itemSunat.valTipo);
                    }

                    _listTipoCambio.Add(_tipoCambio);
                }
            }           

            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sqlSelect = @"SELECT * FROM sunat_tipo_cambio WHERE anio=@anio AND mes=@mes AND dia=@dia";
                var _sqlInsert = @"INSERT INTO sunat_tipo_cambio(anio,mes,dia,venta,compra) VALUES(@anio,@mes,@dia,@venta,@compra)";
                var _sqlUpdate = @"UPDATE sunat_tipo_cambio SET venta=@venta,compra=@compra WHERE anio=@anio AND mes=@mes AND dia=@dia";


                foreach (var item in _listTipoCambio)
                {
                    var _parametros = new DynamicParameters();
                    _parametros.Add("@anio", item.anio);
                    _parametros.Add("@mes", item.mes);
                    _parametros.Add("@dia", item.dia);

                    var _tipoCambioExistente = await db.QueryFirstOrDefaultAsync<TipoCambioDto>(_sqlSelect, _parametros);

                    _parametros.Add("@venta", item.venta);
                    _parametros.Add("@compra", item.compra);

                    if (_tipoCambioExistente == null)
                    {
                        await db.ExecuteAsync(new CommandDefinition(_sqlInsert, _parametros));
                    }
                    else
                    {
                        await db.ExecuteAsync(new CommandDefinition(_sqlUpdate, _parametros));
                    }
                }
            }
        }
    }
}
