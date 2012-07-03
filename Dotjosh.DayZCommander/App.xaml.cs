using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static EventAggregator Events = new EventAggregator();
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

	public class ServerUpdatedEvent
	{
		public Server Server { get; set; }

		public ServerUpdatedEvent(Server server)
		{
			Server = server;
		}
	}

	public class RepublishFriendsRequest
	{
	}
}
