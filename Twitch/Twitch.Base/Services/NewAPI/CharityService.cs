using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StreamingClient.Base.Util;

using Twitch.Base.Models.NewAPI.Charity;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Charity-based services.
	/// </summary>
	public class CharityService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the CharityService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public CharityService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Gets information about the charity campaign that a broadcaster is running. For example, the campaign’s fundraising goal and the current amount of donations.
		/// 
		/// To receive events when progress is made towards the campaign’s goal or the broadcaster changes the fundraising goal, subscribe to the channel.charity_campaign.progress subscription type.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get the charity campaign for</param>
		/// <returns>The broadcaster's current charity</returns>
		public async Task<CharityCampaignModel> GetCharityCampaignAsync(UserModel broadcaster)
		{
			Validator.ValidateVariable(broadcaster, nameof(broadcaster));

			IEnumerable<CharityCampaignModel> campaigns = await GetDataResultAsync<CharityCampaignModel>("charity/campaigns?broadcaster_id=" + broadcaster.id);
			return campaigns?.FirstOrDefault();
		}

		/// <summary>
		/// Gets the list of donations that users have made to the broadcaster’s active charity campaign.
		/// 
		/// To receive events as donations occur, subscribe to the channel.charity_campaign.donate subscription type.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get the charity campaign donations for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The broadcaster's current charity donation</returns>
		public async Task<IEnumerable<CharityCampaignDonationModel>> GetCharityCampaignDonationsAsync(UserModel broadcaster, int maxResults = 1)
		{
			Validator.ValidateVariable(broadcaster, nameof(broadcaster));

			return await GetPagedDataResultAsync<CharityCampaignDonationModel>("charity/campaigns?broadcaster_id=" + broadcaster.id, maxResults);
		}
	}
}
