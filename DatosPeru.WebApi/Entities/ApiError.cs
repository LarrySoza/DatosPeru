using System;
using System.Text.Json.Serialization;

namespace DatosPeru.WebApi.Entities
{
    public class ApiError
    {
        public string code { get; private set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string message { get; private set; }

        public ApiError(string code, string message = null)
        {
            this.code = code;
            this.message = message;
        }
    }
}
