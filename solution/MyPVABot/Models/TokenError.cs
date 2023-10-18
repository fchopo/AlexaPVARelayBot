using Newtonsoft.Json;

namespace MyPVABot.Models.DirectLine
{
    public class TokenError
    {
        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}
