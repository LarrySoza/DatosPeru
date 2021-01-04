using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatosPeru.WebApi.Dto;
using DatosPeru.WebApi.Infrastructure;
using DatosPeru.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatosPeru.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _config;

        public TokenController(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Obtener el token JWT
        /// </summary>
        /// <param name="user">Datos de inicio de sesión</param>
        [ProducesResponseType(typeof(TokenDto), (int)HttpStatusCode.OK)]
        [HttpPost(Name = "ObtenerToken")]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null)
            {
                return BadRequest("Solicitud incorrecta.");
            }

            var _loginClass = new LoginClass(_config);

            var _claimsUser = await _loginClass.GetIdentityAsync(user);


            if (_claimsUser != null)
            {
                var _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("TokenAuthentication:SecretKey").Value));
                var _signinCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);

                var _now = DateTime.UtcNow;
                var _lifetimeSeconds = Convert.ToInt32(_config.GetSection("TokenAuthentication:LifetimeSeconds").Value);
                var _expires = TimeSpan.FromSeconds(_lifetimeSeconds);
                var _iat = new DateTimeOffset(_now).ToUniversalTime().ToUnixTimeSeconds();

                //Claims estandar segun la norma rfc7519
                var _claims = new List<Claim>()
                {
                    //Identifica el objeto o usuario en nombre del cual fue emitido el JWT
                    new Claim(JwtRegisteredClaimNames.Sub, user.usuario),

                    //Identificador único del token incluso entre diferente proveedores de servicio.
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    //Identifica la marca temporal en qué el JWT fue emitido.
                    new Claim(JwtRegisteredClaimNames.Iat, _iat.ToString(), ClaimValueTypes.Integer64) 
                };

                foreach (var item in _claimsUser.Claims)
                {
                    _claims.Add(item);
                }
                               
                var tokeOptions = new JwtSecurityToken(
                    issuer: _config.GetSection("TokenAuthentication:Issuer").Value,
                    audience: _config.GetSection("TokenAuthentication:Audience").Value,
                    claims: _claims,
                    expires: _now.Add(_expires),
                    signingCredentials: _signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new TokenDto()
                {
                    access_token = tokenString
                });
            }
            else
            {
                return Unauthorized(null);
            }
        }
    }
}
