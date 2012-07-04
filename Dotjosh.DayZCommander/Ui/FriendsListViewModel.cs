using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;
using Dotjosh.DayZCommander.Properties;

namespace Dotjosh.DayZCommander.Ui
{
	public class FriendsListViewModel : ViewModelBase, 
		IHandle<PlayersChangedEvent>
	{
		private bool _isAdding;
		private string _newFriendName;

		public FriendsListViewModel()
		{			
			Friends = new ObservableCollection<Friend>();
			if(Settings.Default.Friends == null)
			{
				Settings.Default.Friends = new StringCollection();
				Settings.Default.Save();
			}

			foreach(var friendName in Settings.Default.Friends)
			{
				Friends.Add(new Friend(friendName));
			}
			App.Events.Subscribe(this);
			Title = "Friends";
		}

		public bool IsAdding
		{
			get { return _isAdding; }
			set
			{
				_isAdding = value;
				PropertyHasChanged("IsAdding");
			}
		}

		public string NewFriendName
		{
			get { return _newFriendName; }
			set
			{
				_newFriendName = value;
				PropertyHasChanged("NewFriendName");
			}
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

		public void NewFriend()
		{
			IsAdding = true;
		}

		public void CreateFriend()
		{
			if(!string.IsNullOrWhiteSpace(NewFriendName))
			{
				Friends.Add(new Friend(NewFriendName));
				SaveFriends();
			}
			IsAdding = false;
			NewFriendName = "";
			App.Events.Publish(new RepublishFriendsRequest());
		}

		private void SaveFriends()
		{
			Settings.Default.Friends.Clear();
			foreach(var friend in Friends)
			{
				Settings.Default.Friends.Add(friend.Name);
			}
		}

		public void CancelNewFriend()
		{
			IsAdding = false;
		}
	}

	public class FilterByFriendRequest
	{
		public Friend Friend { get; set; }
		public bool IsFiltered { get; set; }

		public FilterByFriendRequest(Friend friend, bool isFiltered)
		{
			Friend = friend;
			IsFiltered = isFiltered;
		}
	}
}