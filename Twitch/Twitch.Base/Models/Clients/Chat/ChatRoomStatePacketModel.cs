﻿namespace Twitch.Base.Models.Clients.Chat
{
	/// <summary>
	/// Information about a chat room state packet.
	/// </summary>
	public class ChatRoomStatePacketModel : ChatPacketModelBase
	{
		/// <summary>
		/// The ID of the command for a chat user join.
		/// </summary>
		public const string CommandID = "ROOMSTATE";

		/// <summary>
		/// Indicates whether the chat is in emote-only mode.
		/// </summary>
		public bool EmoteOnly { get; set; }

		/// <summary>
		/// Indicates whether the chat is in followers-only mode.
		/// 
		/// Any non-negative number indicates the chat is follower-only and any positive number indicates how many minutes a user must be following for to chat.
		/// </summary>
		public int FollowersOnly { get; set; }

		/// <summary>
		/// Indicates whether the chat is in r9k-only mode.
		/// </summary>
		public bool R9K { get; set; }

		/// <summary>
		/// he number of seconds chatters without moderator privileges must wait between sending messages.
		/// </summary>
		public int Slow { get; set; }

		/// <summary>
		/// Indicates whether the chat is in subs-only mode.
		/// </summary>
		public bool SubsOnly { get; set; }

		/// <summary>
		/// Creates a new instance of the ChatRoomStatePacketModel class.
		/// </summary>
		/// <param name="packet">The Chat packet</param>
		public ChatRoomStatePacketModel(ChatRawPacketModel packet)
			: base(packet)
		{
			EmoteOnly = packet.GetTagBool("emote-only");
			FollowersOnly = packet.GetTagInt("followers-only");
			R9K = packet.GetTagBool("r9k");
			Slow = packet.GetTagInt("slow");
			SubsOnly = packet.GetTagBool("subs-only");
		}
	}
}
