using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalCore.WebApi.Filters
{
    [Serializable]
    public class ClientRole
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;

        [JsonPropertyName("clientRoleClaims")]
        public List<ClientRoleClaim>? ClientRoleClaims { get; set; }
    }

    [Serializable]
    public class ClientRoleClaim
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        [JsonPropertyName("claimType")]
        public string? ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        [JsonPropertyName("claimValue")]
        public string? ClaimValue { get; set; }
    }
}
