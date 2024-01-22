using System.Text.Json.Serialization;

namespace PortalCore.Application.Dtos
{
    public class Token
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = null!;
    }
}
