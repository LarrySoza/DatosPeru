using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using DatosPeru.WebApi.Dto;
using DatosPeru.WebApi.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatosPeru.WebApi.Infrastructure
{
    public class LoginClass
    {
        private readonly IConfiguration _config;

        public LoginClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(LoginDto login)
        {
            login.usuario = login.usuario.ToLower();
            login.clave = GlobalClass.Sha1Enconde(login.clave);

            using (var db = new NpgsqlConnection(_config.GetConnectionString("DatosPeruDb")))
            {
                var _sql = "SELECT * FROM usuarios WHERE nombre_usuario=@nombre_usuario AND clave_hash=@clave_hash";

                var _parametros = new DynamicParameters();
                _parametros.Add("@nombre_usuario", login.usuario);
                _parametros.Add("@clave_hash", login.clave);

                var _usuario = await db.QueryFirstOrDefaultAsync<Usuario>(_sql, _parametros);

                if (_usuario != null)
                {
                    _sql = @"SELECT 
	                            r.*
                            FROM 
	                            usuarios_roles u
                            INNER JOIN
	                            roles r
                            ON
	                            u.rol_id = r.id
                            WHERE
	                            u.usuario_id= @usuario_id";

                    _parametros = new DynamicParameters();
                    _parametros.Add("@usuario_id", _usuario.id);

                    var _roles = await db.QueryAsync<Rol>(_sql, _parametros);

                    var _claims = new List<Claim>();
                    _claims.Add(new Claim("usuarioId", _usuario.id));

                    foreach (var rol in _roles)
                    {
                        _claims.Add(new Claim(ClaimTypes.Role, rol.nombre));
                    }

                    return new ClaimsIdentity(_claims.ToArray());
                }

                return null;
            }
        }
    }
}
