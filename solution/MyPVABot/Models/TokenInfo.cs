using Newtonsoft.Json;

namespace MyPVABot.Models.DirectLine
{
    public class TokenInfo
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("Error")]
        public TokenError Error { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ErrorCode")]
        public int ErrorCode { get; set; }
    }
}
