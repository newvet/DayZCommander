using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dotjosh.DayZCommander.Core
{
	public class Friend : BindableBase
	{
		public string Name { get; set; }

		public Friend(string name)
		{
			Name = name;
			Players = new ObservableCollection<Player>();
		}

		public ObservableCollection<Player> Players { get; set; }

		public bool IsPlaying
		{
			get { return Players.Count > 0; }
		}

		public void NewPlayer(Player newPlayer)
		{
			if(!string.Equals(newPlayer.Name, Name, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if(Players.Any(x => x.Hash == newPlayer.Hash))
				return;

			Players.Add(newPlayer);
			PropertyHasChanged("IsPlaying");
		}

		public void RemovedPlayer(string oldPlayerHash)
		{
			var player = Players.FirstOrDefault(p => p.Hash == oldPlayerHash);
			if(player == null)
				return;

			Players.Remove(player);
			PropertyHasChanged("IsPlaying");
		}
	}
}