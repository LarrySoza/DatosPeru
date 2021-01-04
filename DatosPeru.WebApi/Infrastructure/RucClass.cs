using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DatosPeru.WebApi.Dto;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatosPeru.WebApi.Infrastructure
{
    public class RucClass
    {
        private readonly IConfiguration _config;

        public RucClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<RucDto> Consultar(string ruc)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = @"SELECT 
	                                r.*,
	                                u.departamento,
	                                u.provincia,
	                                u.distrito,
                                    (SELECT max(fecha_hora) FROM sunat_fechas_actualizacion) AS ultima_actualizacion
                                FROM 
	                                sunat_rucs r 
                                LEFT JOIN
	                                vw_ubigeos u
                                ON
	                                r.ubigeo = u.id
                                WHERE 
	                                ruc = @ruc";

                var _parametros = new DynamicParameters();
                _parametros.Add("@ruc", ruc);

                var _rucExistente = await db.QueryFirstOrDefaultAsync<RucDto>(_sql, _parametros);

                return _rucExistente;
            }
        }

        public async Task<ConsultarRucMultipleDto> Consultar(IEnumerable<string> rucs)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _rucsEncontrados = new List<RucDto>();
                var _noEncontrados = new List<string>();

                var _sql = @"SELECT 
	                                r.*,
	                                u.departamento,
	                                u.provincia,
	                                u.distrito,
                                    (SELECT max(fecha_hora) FROM sunat_fechas_actualizacion) AS ultima_actualizacion
                                FROM 
	                                sunat_rucs r 
                                LEFT JOIN
	                                vw_ubigeos u
                                ON
	                                r.ubigeo = u.id
                                WHERE 
	                                ruc = @ruc";

                foreach (string item in rucs)
                {
                    var _parametros = new DynamicParameters();
                    _parametros.Add("@ruc", item);

                    var _rucExistente = await db.QueryFirstOrDefaultAsync<RucDto>(_sql, _parametros);

                    if (_rucExistente != null)
                    {
                        _rucsEncontrados.Add(_rucExistente);
                    }
                    else
                    {
                        _noEncontrados.Add(item);
                    }
                }

                return new ConsultarRucMultipleDto()
                {
                    no_encontrados = _noEncontrados,
                    rucs = _rucsEncontrados
                };
            }
        }
    }
}
