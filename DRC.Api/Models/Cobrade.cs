using System.Text.Json.Serialization;

namespace DRC.Api.Models
{
    public class Cobrade
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("cobrade")]
        public long CobradeId { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}
