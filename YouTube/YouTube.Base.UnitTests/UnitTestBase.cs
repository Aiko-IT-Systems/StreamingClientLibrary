﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StreamingClient.Base.Util;

namespace YouTube.Base.UnitTests
{
	public class UnitTestBase
	{
		public static string clientID = "884596410562-pcrl1fn8ov0npj7fhjl086ffmud7r5j6.apps.googleusercontent.com";
		public static string clientSecret = "QBkxNmPNIvWatRvOIfRYrXlc";

		public static readonly List<OAuthClientScopeEnum> scopes = new()
		{
			OAuthClientScopeEnum.ManageAccount,
			OAuthClientScopeEnum.ManageData,
			OAuthClientScopeEnum.ManagePartner,
			OAuthClientScopeEnum.ManagePartnerAudit,
			OAuthClientScopeEnum.ManageVideos,
			OAuthClientScopeEnum.ReadOnlyAccount,
			OAuthClientScopeEnum.ViewAnalytics,
			OAuthClientScopeEnum.ViewMonetaryAnalytics
		};

		private static YouTubeConnection connection;

		[AssemblyInitialize]
		public void Initialize()
		{
			Logger.LogOccurred += Logger_LogOccurred;
		}

		public static YouTubeConnection GetYouTubeLiveClient()
		{
			if (UnitTestBase.connection == null)
			{
				UnitTestBase.connection = YouTubeConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes).Result;
			}

			Assert.IsNotNull(UnitTestBase.connection);
			return UnitTestBase.connection;
		}

		protected static void TestWrapper(Func<YouTubeConnection, Task> function)
		{
			if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
			{
				throw new InvalidOperationException("Client ID and/or Client Secret are not set in the UnitTestBase class");
			}

			try
			{
				YouTubeConnection connection = UnitTestBase.GetYouTubeLiveClient();
				function(connection).Wait();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.ToString());
			}
		}

		private void Logger_LogOccurred(object sender, Log log)
		{
			if (log.Level >= LogLevel.Error)
			{
				Assert.Fail(log.Message);
			}
		}
	}
}
