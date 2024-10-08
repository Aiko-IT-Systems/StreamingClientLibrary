using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;

using Twitch.Base.Models.NewAPI.Bits;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The period of time for a Bits leaderboard.
	/// </summary>
	public enum BitsLeaderboardPeriodEnum
	{
		/// <summary>
		/// Day
		/// </summary>
		Day,
		/// <summary>
		/// Week
		/// </summary>
		Week,
		/// <summary>
		/// Month
		/// </summary>
		Month,
		/// <summary>
		/// Year
		/// </summary>
		Year,
		/// <summary>
		/// All
		/// </summary>
		All
	}

	/// <summary>
	/// The APIs for Bits-based services.
	/// </summary>
	public class BitsService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the BitsService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public BitsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Retrieves the list of available Cheermotes, animated emotes to which viewers can assign Bits, to cheer in chat. Cheermotes returned are available throughout Twitch, in all Bits-enabled channels.
        /// </summary>
        /// <param name="channel">Optional channel to include specialized cheermotes for if they exist</param>
        /// <returns>The list of available cheermotes</returns>
        public async Task<IEnumerable<BitsCheermoteModel>> GetCheermotesAsync(UserModel channel = null) => await GetDataResultAsync<BitsCheermoteModel>("bits/cheermotes" + ((channel != null) ? "?broadcaster_id=" + channel.id : ""));

        /// <summary>
        /// Gets the Bits leaderboard for the current channel.
        /// </summary>
        /// <param name="startedAt">The date when the leaderboard should start</param>
        /// <param name="period">The period to get the leaderboard for</param>
        /// <param name="userID">An optional user to get bits leaderboard data specifically for</param>
        /// <param name="count">The total amount of users to include</param>
        /// <returns>The Bits leaderboard</returns>
        public async Task<BitsLeaderboardModel> GetBitsLeaderboardAsync(DateTimeOffset? startedAt = null, BitsLeaderboardPeriodEnum period = BitsLeaderboardPeriodEnum.All, string userID = null, int count = 10)
		{
			Dictionary<string, string> parameters = new();
			if (startedAt != null)
			{
				parameters.Add("started_at", startedAt.GetValueOrDefault().ToRFC3339String());
			}
			if (userID != null)
			{
				parameters.Add("user_id", userID);
			}
			parameters.Add("period", period.ToString().ToLower());
			parameters.Add("count", count.ToString());

			string parameterString = string.Join("&", parameters.Select(kvp => kvp.Key + "=" + kvp.Value));
			JObject jobj = await GetJObjectAsync("bits/leaderboard?" + parameterString);
			if (jobj != null)
			{
				BitsLeaderboardModel result = new()
                {
					users = ((JArray)jobj["data"]).ToTypedArray<BitsLeaderboardUserModel>(),
					started_at = jobj["date_range"]["started_at"].ToString(),
					ended_at = jobj["date_range"]["ended_at"].ToString()
				};
				return result;
			}
			return null;
		}
	}
}
