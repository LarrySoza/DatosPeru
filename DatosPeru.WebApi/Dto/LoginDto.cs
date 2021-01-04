using System;
using System.ComponentModel.DataAnnotations;

namespace DatosPeru.WebApi.Dto
{
    public class LoginDto
    {
        /// <summary>
        /// Por lo general se usa un correo electronico
        /// </summary>
        [Required]
        public string usuario { get; set; }

        /// <summary>
        /// La clave de usuario sin encriptar
        /// </summary>
        [Required]
        public string clave { get; set; }
    }
}
