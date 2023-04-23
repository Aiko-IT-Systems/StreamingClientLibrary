﻿using System;
using System.Text;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;

namespace Glimesh.Base.Models.Clients
{
	/// <summary>
	/// Base model for client-based packets.
	/// </summary>
	public abstract class ClientPacketModelBase
	{
		/// <summary>
		/// Default topic used for most packets.
		/// </summary>
		public const string AbsintheControlTopicName = "__absinthe__:control";

		/// <summary>
		/// Event sent when connecting to specific channel's chat.
		/// </summary>
		public const string DocEventName = "doc";

		/// <summary>
		/// Reply event received when for most response packets.
		/// </summary>
		public const string ReplyEventName = "phx_reply";

		/// <summary>
		/// The join reference for multi-connection.
		/// </summary>
		public string JoinRef { get; set; } = "1";

		/// <summary>
		/// The normal reference for multi-connection.
		/// </summary>
		public string NormalRef { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		/// The topic triggered.
		/// </summary>
		public string Topic { get; set; }

		/// <summary>
		/// The event that occurred.
		/// </summary>
		public string Event { get; set; }

		/// <summary>
		/// The payload of the packet.
		/// </summary>
		public JObject Payload { get; set; } = new JObject();

		/// <summary>
		/// Creates a new instance of the ClientPacketModelBase class.
		/// </summary>
		protected ClientPacketModelBase() { }

		/// <summary>
		/// Creates a new instance of the ClientPacketModelBase class.
		/// </summary>
		/// <param name="serializedChatPacketArray">The serialized packet array</param>
		protected ClientPacketModelBase(string serializedChatPacketArray)
			: this()
		{
			JArray array = JSONSerializerHelper.DeserializeFromString<JArray>(serializedChatPacketArray);
			if (array.Count > 0)
			{ this.JoinRef = array[0]?.ToString(); }
			if (array.Count > 1)
			{ this.NormalRef = array[1]?.ToString(); }
			if (array.Count > 2)
			{ this.Topic = array[2]?.ToString(); }
			if (array.Count > 3)
			{ this.Event = array[3]?.ToString(); }
			if (array.Count > 4 && array[4] != null)
			{ this.Payload = (JObject)array[4]; }
		}

		/// <summary>
		/// Whether the packet is a reply event.
		/// </summary>
		public bool IsReplyEvent { get { return string.Equals(this.Event, ReplyEventName, StringComparison.OrdinalIgnoreCase); } }

		/// <summary>
		/// Whether the payload contains the Status property and it's set to "ok".
		/// </summary>
		public bool IsPayloadStatusOk
		{
			get
			{
				if (this.Payload != null && this.Payload.ContainsKey("status"))
				{
					return string.Equals(this.Payload["status"]?.ToString(), "ok", StringComparison.OrdinalIgnoreCase);
				}
				return false;
			}
		}

		/// <summary>
		/// Generates the serialized packet array for sending over the web socket connection.
		/// </summary>
		/// <returns>The serialized packet array</returns>
		public string ToSerializedPacketArray()
		{
			return JSONSerializerHelper.SerializeToString(new JArray() { this.JoinRef, this.NormalRef, this.Topic, this.Event, this.Payload });
		}

		/// <summary>
		/// Encodes the specified text for sending in a GraphQL packet.
		/// </summary>
		/// <param name="text">The text to encode</param>
		/// <returns>The encoded text</returns>
		protected string EncodeText(string text)
		{
			StringBuilder str = new StringBuilder();
			foreach (char c in text)
			{
				if (c == '\\')
				{
					str.Append("\\\\");
				}
				else if (c == '\"')
				{
					str.Append("\\\"");
				}
				else
				{
					str.Append(c);
				}
			}
			return str.ToString();
		}
	}
}
