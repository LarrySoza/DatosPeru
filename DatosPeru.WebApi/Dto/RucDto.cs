using System;
namespace DatosPeru.WebApi.Dto
{
    public class RucDto
    {
        public string ruc { get; set; }
        public string nombre_o_razon_social { get; set; }
        public string estado_del_contribuyente { get; set; }
        public string condicion_de_domicilio { get; set; }
        public string ubigeo { get; set; }
        public string tipo_de_via { get; set; }
        public string nombre_de_via { get; set; }
        public string codigo_de_zona { get; set; }
        public string tipo_de_zona { get; set; }
        public string numero { get; set; }
        public string interior { get; set; }
        public string lote { get; set; }
        public string dpto { get; set; }
        public string manzana { get; set; }
        public string kilometro { get; set; }

        public string departamento { get; set; }

        public string provincia { get; set; }

        public string distrito { get; set; }

        public string direccion
        {
            get
            {
                var _via = string.IsNullOrWhiteSpace($"{tipo_de_via} {nombre_de_via}".Trim('-')) ? null : $"{tipo_de_via} {nombre_de_via}";
                var _zona = string.IsNullOrWhiteSpace($"{codigo_de_zona} {tipo_de_zona}".Trim('-')) ? null : $"{codigo_de_zona} {tipo_de_zona}";
                var _numero = string.IsNullOrWhiteSpace(numero.Trim('-')) ? null : $"NRO. {numero}";
                var _interior = string.IsNullOrWhiteSpace(interior.Trim('-')) ? null : $"INT. {interior}";
                var _lote = string.IsNullOrWhiteSpace(lote.Trim('-')) ? null : $"LT. {lote}";
                var _dpto = string.IsNullOrWhiteSpace(dpto.Trim('-')) ? null : $"DPTO. {dpto}";
                var _manzana = string.IsNullOrWhiteSpace(manzana.Trim('-')) ? null : $"MZ. {manzana}";
                var _kilometro = string.IsNullOrWhiteSpace(kilometro.Trim('-')) ? null : $"KM. {kilometro}";

                //var _direccion = $"{_via} {_numero} {_interior} {_lote} {_dpto} {_manzana} {_kilometro} {_zona}";

                var _direccion = string.Empty;
                _direccion = $"{_via}";
                _direccion = $"{_direccion.Trim()} {_numero}";
                _direccion = $"{_direccion.Trim()} {_interior}";
                _direccion = $"{_direccion.Trim()} {_lote}";
                _direccion = $"{_direccion.Trim()} {_dpto}";
                _direccion = $"{_direccion.Trim()} {_manzana}";
                _direccion = $"{_direccion.Trim()} {_kilometro}";
                _direccion = $"{_direccion.Trim()} {_zona}";

                return _direccion.Trim();
            }
        }

        public string direccion_completa
        {
            get
            {
                var _ubigeoStr = $"{departamento} {provincia} {distrito}";

                return string.IsNullOrWhiteSpace(_ubigeoStr) ? direccion : $"{direccion} - {_ubigeoStr}";
            }
        }

        public DateTime ultima_actualizacion { get; set; }
    }
}
