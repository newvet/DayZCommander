using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
			if (Settings.Default.Friends == null)
			{
				Settings.Default.Friends = new StringCollection();
				Settings.Default.Save();
			}

			foreach (string friendName in Settings.Default.Friends)
			{
				Friends.Add(new Friend(friendName));
			}
			App.Events.Subscribe(this);
			Title = "friends";
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

		private ObservableCollection<Friend> _friends;
		public ObservableCollection<Friend> Friends
		{
			get { return _friends; }
			private set
			{
				_friends = value;
				PropertyHasChanged("Friends");
			}
		}

		#region IHandle<PlayersChangedEvent> Members

		public void Handle(PlayersChangedEvent message)
		{
			foreach (Player oldPlayer in message.OldPlayers)
			{
				string oldPlayerHash = oldPlayer.Hash;
				bool wasRemoved = message
					.NewPlayers
					.None(newPlayer => newPlayer.Hash == oldPlayerHash);

				if (wasRemoved)
					Remove(oldPlayerHash);
			}

			foreach (Player newPlayer in message.NewPlayers)
			{
				Add(newPlayer);
			}
		}

		#endregion

		private void Add(Player newPlayer)
		{
			Friends.ToList(friend => friend.NewPlayer(newPlayer));
			UpdateTitle();
		}

		private void Remove(string oldPlayerHash)
		{
			Friends.ToList(friend => friend.RemovedPlayer(oldPlayerHash));
			UpdateTitle();
		}

		private void UpdateTitle()
		{
			var count = Friends.Count(f => f.IsPlaying);
			if(count == 0)
				Title = "friends";
			else
				Title = string.Format("friends({0})", count);
		}

		public void NewFriend()
		{
			IsAdding = true;
		}

		public void CreateFriend()
		{
			if (!string.IsNullOrWhiteSpace(NewFriendName))
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
			Settings.Default.Upgrade();
			Settings.Default.Friends.Clear();
			foreach (Friend friend in Friends)
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
		public FilterByFriendRequest(Friend friend, bool isFiltered)
		{
			Friend = friend;
			IsFiltered = isFiltered;
		}

		public Friend Friend { get; set; }
		public bool IsFiltered { get; set; }
	}
}