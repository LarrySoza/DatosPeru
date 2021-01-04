using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DatosPeru.WebApi.Dto;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RestSharp;

namespace DatosPeru.WebApi.Infrastructure
{
    public class DniClass
    {
        private readonly IConfiguration _config;

        public DniClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<DniDto> ConsultarJne(string dni)
        {
            var _client = new RestClient("https://aplicaciones007.jne.gob.pe/srop_publico/Consulta/api/AfiliadoApi/GetNombresCiudadano");

            var _bodyRequest = new
            {
                CODDNI = dni
            };

            var _request = new RestRequest()
                .AddJsonBody(_bodyRequest);

            _request.AddHeader("RequestVerificationToken", "Dmfiv1Unnsv8I9EoXEzbyQExSD8Q1UY7viyyf_347vRCfO-1xGFvDddaxDAlvm0cZ8XgAKTaWclVFnnsGgoy4aLlBGB5m-E8rGw_ymEcCig1:eq4At-H2zqgXPrPnoiDGFZH0Fdx5a-1UiyVaR4nQlCvYZzAhzmvWxLwkUk6-yORYrBBxEnoG5sm-Hkiyc91so6-nHHxIeLee5p700KE47Cw1");

            var _dnijne = await _client.PostAsync<DniJneDto>(_request);

            var _nombreSplit = _dnijne.data.Split('|');

            if (!string.IsNullOrWhiteSpace(_nombreSplit[0]))
            {
                var _dni = new DniDto()
                {
                    dni = dni,
                    apellido_paterno = _nombreSplit[0]?.Trim(),
                    apellido_materno = _nombreSplit[1]?.Trim(),
                    nombres = _nombreSplit[2]?.Trim()
                };

                Actualizar(_dni);

                return _dni;
            }

            return null;
        }

        /// <summary>
        /// En la base de datos ya existen todos los registros del 00000000 al 99999999
        /// </summary>
        /// <param name="dni"></param>
        public async void Actualizar(DniDto dni)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = @"UPDATE reniec_dnis SET
                                    apellido_paterno = @apellido_paterno,
                                    apellido_materno = @apellido_materno,
                                    nombres = @nombres
                             WHERE
                                    dni=@dni";

                var _parametros = new DynamicParameters();
                _parametros.Add("@apellido_paterno", dni.apellido_paterno);
                _parametros.Add("@apellido_materno", dni.apellido_materno);
                _parametros.Add("@nombres", dni.nombres);
                _parametros.Add("@dni", dni.dni);

                var _dniExistente = await db.ExecuteAsync(_sql, _parametros);
            }
        }

        public async Task<DniDto> Consultar(string dni)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = @"SELECT * FROM reniec_dnis WHERE dni=@dni";

                var _parametros = new DynamicParameters();
                _parametros.Add("@dni", dni);

                var _dniExistente = await db.QueryFirstOrDefaultAsync<DniDto>(_sql, _parametros);

                if (_dniExistente != null && _dniExistente.nombres != "-")
                {
                    return _dniExistente;
                }

                return null;
            }
        }

        public async Task<ConsultarDniMultipleDto> Consultar(IEnumerable<string> dnis)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _dnisEncontrados = new List<DniDto>();
                var _noEncontrados = new List<string>();

                var _sql = @"SELECT * FROM reniec_dnis WHERE dni=@dni";

                foreach (string item in dnis)
                {
                    var _parametros = new DynamicParameters();
                    _parametros.Add("@dni", item);

                    var _dniExistente = await db.QueryFirstOrDefaultAsync<DniDto>(_sql, _parametros);

                    if (_dniExistente != null && _dniExistente.nombres != "-")
                    {
                        _dnisEncontrados.Add(_dniExistente);
                    }
                    else
                    {
                        _noEncontrados.Add(item);
                    }
                }

                return new ConsultarDniMultipleDto()
                {
                    no_encontrados = _noEncontrados,
                    dnis = _dnisEncontrados
                };
            }
        }
    }
}
