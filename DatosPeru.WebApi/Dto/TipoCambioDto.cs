using System;
namespace DatosPeru.WebApi.Dto
{
    public class TipoCambioDto
    {
        public int anio { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }
        public decimal venta { get; set; }
        public decimal compra { get; set; }
    }
}
