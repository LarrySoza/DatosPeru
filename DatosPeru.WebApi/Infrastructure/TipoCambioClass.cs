using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DatosPeru.WebApi.Dto;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatosPeru.WebApi.Infrastructure
{
    public class TipoCambioClass
    {
        private readonly IConfiguration _config;

        public TipoCambioClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<TipoCambioDto> Consultar(int anio, int mes, int dia)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = @"SELECT * FROM sunat_tipo_cambio WHERE anio=@anio AND mes=@mes AND dia=@dia";

                var _parametros = new DynamicParameters();
                _parametros.Add("@anio", anio);
                _parametros.Add("@mes", mes);
                _parametros.Add("@dia", dia);

                var _tipoCambioExistente = await db.QueryFirstOrDefaultAsync<TipoCambioDto>(_sql, _parametros);

                return _tipoCambioExistente;
            }
        }

        public async Task<IEnumerable<TipoCambioDto>> Consultar(int anio, int mes)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = @"SELECT * FROM sunat_tipo_cambio WHERE anio=@anio AND mes=@mes ORDER BY dia";

                var _parametros = new DynamicParameters();
                _parametros.Add("@anio", anio);
                _parametros.Add("@mes", mes);

                var _tipoCambioExistente = await db.QueryAsync<TipoCambioDto>(_sql, _parametros);

                return _tipoCambioExistente;
            }
        }
    }
}
