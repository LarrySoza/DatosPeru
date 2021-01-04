using System;
namespace DatosPeru.WebApi.Dto
{
    public class SunatTipoCambioRequestDto
    {
        public int anio { get; set; }
        public int mes { get; set; }
        public string token { get; set; }
    }
}
