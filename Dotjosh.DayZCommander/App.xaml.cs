using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;
using NLog;

namespace Dotjosh.DayZCommander
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static EventAggregator Events = new EventAggregator();
		private bool _isUncaughtUiThreadException;

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += UncaughtThreadException;
			DispatcherUnhandledException += UncaughtUiThreadException;
			//LocalMachineInfo.Touch();

			base.OnStartup(e);
		}

		private void UncaughtUiThreadException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			_isUncaughtUiThreadException = true;
			_logger.Fatal(e.Exception);
			var messageBoxResult = MessageBox.Show(
				"It wasn't your fault, but something went really wrong!\r\nWould you like me to try and restart DayZ Commander for you?",
				"Oh noes",
				MessageBoxButton.YesNo);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				e.Handled = true;
				System.Windows.Forms.Application.Restart();
				Current.Shutdown();
			}
		}

		private void UncaughtThreadException(object sender, UnhandledExceptionEventArgs e)
		{
			if(_isUncaughtUiThreadException)
				return;
			var exception = e.ExceptionObject as Exception;
			_logger.Fatal(exception);
		}

	}

	public class PlayersChangedEvent
	{
		public IEnumerable<Player> OldPlayers { get; set; }
		public IEnumerable<Player> NewPlayers { get; set; }

		public PlayersChangedEvent(IEnumerable<Player> oldPlayers, IEnumerable<Player> newPlayers)
		{
			OldPlayers = oldPlayers;
			NewPlayers = newPlayers;
		}
	}

	public class ServerUpdated
	{
		public Server Server { get; set; }

		public ServerUpdated(Server server)
		{
			Server = server;
		}
	}

	public class RepublishFriendsRequest
	{
	}
}
