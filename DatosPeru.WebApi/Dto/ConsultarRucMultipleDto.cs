using System;
using System.Collections.Generic;

namespace DatosPeru.WebApi.Dto
{
    public class ConsultarRucMultipleDto
    {
        public IEnumerable<string> no_encontrados { get; set; }

        public IEnumerable<RucDto> rucs { get; set; }
    }
}
