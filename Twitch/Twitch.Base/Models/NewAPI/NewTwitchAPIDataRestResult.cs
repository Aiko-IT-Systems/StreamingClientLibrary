﻿using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.NewAPI
{
	/// <summary>
	/// A wrapper result used for the New Twitch APIs
	/// </summary>
	/// <typeparam name="T">The type that the result contains</typeparam>
	public class NewTwitchAPIDataRestResult<T>
	{
		/// <summary>
		/// The data of the result.
		/// </summary>
		public List<T> data { get; set; } = new List<T>();

		/// <summary>
		/// The total number of results.
		/// </summary>
		public long total { get; set; }

		/// <summary>
		/// Pagination information.
		/// </summary>
		public JObject pagination { get; set; }

		/// <summary>
		/// The pagination cursor.
		/// </summary>
		public string Cursor { get { return (this.pagination != null && this.pagination.ContainsKey("cursor")) ? this.pagination["cursor"].ToString() : null; } }
	}
}
