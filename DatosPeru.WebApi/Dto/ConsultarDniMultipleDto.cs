using System;
using System.Collections.Generic;

namespace DatosPeru.WebApi.Dto
{
    public class ConsultarDniMultipleDto
    {
        public IEnumerable<string> no_encontrados { get; set; }

        public IEnumerable<DniDto> dnis { get; set; }
    }
}
