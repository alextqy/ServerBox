using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    public class Base
    {

        [NotMapped]
        [JsonPropertyName("State")]
        public bool State { get; set; }

        [NotMapped]
        [JsonPropertyName("StatusCode")]
        public int StatusCode { get; set; }

        [NotMapped]
        [JsonPropertyName("Memo")]
        public string Memo { get; set; }

        public Base()
        {
            this.State = false;
            this.StatusCode = 200;
            this.Memo = "";
        }
    }
}
