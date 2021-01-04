using System;
namespace DatosPeru.WebApi.Entities
{
    public class Usuario
    {
        public string id { get; set; }
        public DateTime fecha_registro { get; set; }
        public string nombre_usuario { get; set; }
        public string clave_hash { get; set; }
        public bool correo_confirmado { get; set; }
    }
}
