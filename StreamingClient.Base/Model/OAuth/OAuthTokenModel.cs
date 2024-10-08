using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace StreamingClient.Base.Model.OAuth
{
    /// <summary>
    /// A token received from an OAuth authentication service.
    /// </summary>
    [DataContract]
    public class OAuthTokenModel
    {
        /// <summary>
        /// The ID of the client service.
        /// </summary>
        [DataMember]
        public string clientID { get; set; }

        /// <summary>
        /// The secret of the client service.
        /// </summary>
        [DataMember]
        public string clientSecret { get; set; }

        /// <summary>
        /// The authorization code sent when authenticating against the OAuth service.
        /// </summary>
        [DataMember]
        public string authorizationCode { get; set; }

        /// <summary>
        /// The token used for refreshing the authentication.
        /// </summary>
        [JsonProperty("refresh_token"), DataMember]
        public string refreshToken { get; set; }

        /// <summary>
        /// The token used for accessing the OAuth service.
        /// </summary>
        [JsonProperty("access_token"), DataMember]
        public string accessToken { get; set; }

        /// <summary>
        /// The expiration time of the token in seconds from when it was obtained.
        /// </summary>
        [JsonProperty("expires_in"), DataMember]
        public long expiresIn { get; set; }

        /// <summary>
        /// The timestamp of the expiration, if supported by the service, in seconds from Unix Epoch
        /// </summary>
        [JsonIgnore]
        public long expiresTimeStamp { get; set; }

        /// <summary>
        /// The redirect URL used as part of the token.
        /// </summary>
        [DataMember]
        public string redirectUrl { get; set; }

        /// <summary>
        /// The time when the token was obtained.
        /// </summary>
        [DataMember]
        public DateTimeOffset AcquiredDateTime { get; set; }

        /// <summary>
        /// The expiration time of the token.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset ExpirationDateTime => (expiresTimeStamp > 0) ? DateTimeOffset.FromUnixTimeSeconds(expiresTimeStamp) : AcquiredDateTime.AddSeconds(expiresIn);

        /// <summary>
        /// Creates a new instance of an OAuth token.
        /// </summary>
        public OAuthTokenModel() => AcquiredDateTime = DateTimeOffset.Now;
    }
}
