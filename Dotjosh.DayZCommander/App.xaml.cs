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
		public ObservableCollection<Player> OldPlayers { get; set; }
		public List<Player> NewPlayers { get; set; }

		public PlayersChangedEvent(ObservableCollection<Player> oldPlayers, List<Player> newPlayers)
		{
			OldPlayers = oldPlayers;
			NewPlayers = newPlayers;
		}
	}
}
