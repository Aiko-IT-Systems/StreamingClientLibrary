﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trovo.Base.UnitTests
{
	public abstract class UnitTestBase
	{
		public const string clientID = "8FMjuk785AX4FMyrwPTU3B8vYvgHWN33";

		public static readonly List<OAuthClientScopeEnum> scopes = new()
		{
			OAuthClientScopeEnum.chat_connect,
			OAuthClientScopeEnum.chat_send_self,
			OAuthClientScopeEnum.send_to_my_channel,
			OAuthClientScopeEnum.manage_messages,

			OAuthClientScopeEnum.channel_details_self,
			OAuthClientScopeEnum.channel_update_self,
			OAuthClientScopeEnum.channel_subscriptions,

			OAuthClientScopeEnum.user_details_self,
		};

		private static TrovoConnection connection;

		public static TrovoConnection GetTrovoClient()
		{
			if (UnitTestBase.connection == null)
			{
				UnitTestBase.connection = TrovoConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes).Result;
			}

			Assert.IsNotNull(UnitTestBase.connection);
			return UnitTestBase.connection;
		}

		protected static void TestWrapper(Func<TrovoConnection, Task> function)
		{
			try
			{
				TrovoConnection connection = UnitTestBase.GetTrovoClient();
				function(connection).Wait();
			}
			catch (AggregateException aex)
			{
				Assert.Fail(aex.InnerException.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.ToString());
			}
		}
	}
}
