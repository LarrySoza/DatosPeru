using System;
namespace DatosPeru.WebApi.Dto
{
    /// <summary>
    /// Actualmente es la extructura que devuelve SUNAT al hacer POST a
    /// https://e-consulta.sunat.gob.pe/cl-at-ittipcam/tcS01Alias/listarTipoCambio
    /// </summary>
    public class SunatTipoCambioDto
    {
        public string fecPublica { get; set; }
        public string valTipo { get; set; }
        public string codTipo { get; set; }
    }
}
