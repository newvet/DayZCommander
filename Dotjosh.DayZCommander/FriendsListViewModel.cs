using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander
{
	public class FriendsListViewModel : BindableBase, IHandle<PlayersChangedEvent>
	{
		public FriendsListViewModel()
		{
			Friends = new ObservableCollection<Friend>(new [] { new Friend("Travis") });
			App.Events.Subscribe(this);
		}

		public void Handle(PlayersChangedEvent message)
		{
			foreach(var oldPlayer in message.OldPlayers)
			{
				var oldPlayerHash = oldPlayer.Hash;
				var wasRemoved = message
									.NewPlayers
				                  	.None(newPlayer => newPlayer.Hash == oldPlayerHash);

				if(wasRemoved)
					Remove(oldPlayerHash);
			}

			foreach(var newPlayer in message.NewPlayers)
			{
				var newPlayerHash = newPlayer.Hash;
				var isNew = message
								.OldPlayers
								.None(oldPlayer => oldPlayer.Hash == newPlayerHash);
				if(isNew)
					Add(newPlayer);
			}
		}

		private void Add(Player newPlayer)
		{
			Friends.ToList(friend => friend.NewPlayer(newPlayer));
		}

		private void Remove(string oldPlayerHash)
		{
			Friends.ToList(friend => friend.RemovedPlayer(oldPlayerHash));
		}

		public ObservableCollection<Friend> Friends { get; private set; }
	}
}