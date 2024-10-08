using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI.EventSub;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for EventSub-based services.
	/// </summary>
	public class EventSubService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// The header for the message id.
		/// </summary>
		public const string TwitchEventSubMessageIdHeader = "Twitch-Eventsub-Message-Id";
		/// <summary>
		/// The header for the message timestamp.
		/// </summary>
		public const string TwitchEventSubMessageTimestampHeader = "Twitch-Eventsub-Message-Timestamp";
		/// <summary>
		/// The header for the message signature.
		/// </summary>
		public const string TwitchEventSubMessageSignatureHeader = "Twitch-Eventsub-Message-Signature";

		/// <summary>
		/// Creates an instance of the EventSubService
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public EventSubService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Gets the subscriptions of the app identified by a Bearer token.  Requires App Token.
		/// </summary>
		/// <param name="maxResults">Maximum results to return. Must be a value from 10 to 100.</param>
		/// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
		public async Task<IEnumerable<EventSubSubscriptionModel>> GetSubscriptionsAsync(int maxResults = 100)
		{
			Validator.Validate(maxResults >= 10, "maxResults must be greater than or equal to 10.");
			Validator.Validate(maxResults <= 100, "maxResults must be less than or equal to 100.");
			return await GetPagedDataResultAsync<EventSubSubscriptionModel>("eventsub/subscriptions", maxResults);
		}

        /// <summary>
        /// Deletes a subscription of the app identified by a Bearer token.  Requires App Token.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to delete</param>
        /// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
        public async Task DeleteSubscriptionAsync(string subscriptionId) => await DeleteAsync($"eventsub/subscriptions?id={subscriptionId}");

        /// <summary>
        /// Creates a subscription for the app identified by a Bearer token.  Requires App Token.
        /// </summary>
        /// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
        [Obsolete("Not used anymore. Use CreateSubscriptionAsync instead.")]
        public Task<EventSubSubscriptionModel> CreateSubscription(string type, string conditionName, string conditionValue, string callback, string secret, string version = null) => CreateSubscriptionAsync(type, "webhook", new Dictionary<string, string> { { conditionName, conditionValue } }, secret, callback, version);

        /// <summary>
        /// Creates a subscription for the app identified by a Bearer token.  Requires App Token for webhook and user token for websocket.
        /// </summary>
        /// <returns>The subscribed event for the app identified by a Bearer token in the Twitch connection</returns>
        public async Task<EventSubSubscriptionModel> CreateSubscriptionAsync(string type, string transportMethod, IReadOnlyDictionary<string, string> conditions, string secretOrSessionId, string webhookCallback = null, string version = null)
		{
			JObject jobj = new()
            {
				["type"] = type
			};

			jobj["version"] = string.IsNullOrEmpty(version) ? (JToken)"1" : (JToken)version;

            jobj["condition"] = new JObject();
			foreach (KeyValuePair<string, string> kvp in conditions)
			{
				jobj["condition"][kvp.Key] = kvp.Value;
			}

			jobj["transport"] = new JObject
			{
				["method"] = transportMethod
			};
			if (transportMethod == "webhook")
			{
				jobj["transport"]["callback"] = webhookCallback;
				jobj["transport"]["secret"] = secretOrSessionId;
			}
			else
			{
				jobj["transport"]["session_id"] = secretOrSessionId;
			}

            // TODO: Consider getting other top level fields
            //      "total": 1,
            //      "total_cost": 1,
            //      "max_total_cost": 10000,
            //      "limit": 10000
            EventSubSubscriptionModel[] subs = await PostDataResultAsync<EventSubSubscriptionModel>("eventsub/subscriptions", AdvancedHttpClient.CreateContentFromObject(jobj));
			return subs?.FirstOrDefault();
		}

		/// <summary>
		/// Used to verify the signature from a twitch event sub webhook call.
		/// </summary>
		/// <param name="twitchEventSubMessageId">This comes from the Twitch-Eventsub-Message-Id header.</param>
		/// <param name="twitchEventSubMessageTimestamp">This comes from the Twitch-Eventsub-Message-Timestamp header.</param>
		/// <param name="body">This is the body of the message.</param>
		/// <param name="twitchEventSubMessageSignature">This comes from the Twitch-Eventsub-Message-Signature header.</param>
		/// <param name="secret">This is the secret you provided when you created the webhook.</param>
		/// <returns></returns>
		public static bool VerifySignature(string twitchEventSubMessageId, string twitchEventSubMessageTimestamp, string body, string twitchEventSubMessageSignature, string secret)
		{
			if (string.IsNullOrEmpty(twitchEventSubMessageId) ||
				string.IsNullOrEmpty(twitchEventSubMessageTimestamp) ||
				string.IsNullOrEmpty(body) ||
				string.IsNullOrEmpty(twitchEventSubMessageSignature))
			{
				return false;
			}

            HMACSHA256 sha256 = new(Encoding.UTF8.GetBytes(secret));

            string data = twitchEventSubMessageId + twitchEventSubMessageTimestamp + body;
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            string stringHash = BitConverter.ToString(hash).Replace("-", "");

            string calculatedSignature = $"sha256={stringHash}";
			return string.Equals(calculatedSignature, twitchEventSubMessageSignature, StringComparison.OrdinalIgnoreCase);
		}
	}
}
