
namespace MyPVABot.Models.DirectLine
{
    using Newtonsoft.Json;
    public class DirectLineEndPoint
    {
        public class ChannelUrlsById
        {
            [JsonProperty("directline")]
            public string DirectLine { get; set; }
        }

        public class DirectLineEndPointReturn
        {
            [JsonProperty("channelUrlsById")]
            public ChannelUrlsById ChannelUrlsById { get; set; }

            [JsonProperty("geo")]
            public string Geo { get; set; }
        }
    }
}
