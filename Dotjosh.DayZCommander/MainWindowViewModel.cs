using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander
{
	public class MainWindowViewModel : BindableBase
	{
		private readonly Action<Action> _executeOnMainThread;
		private FriendsListViewModel _friendslist;

		public MainWindowViewModel(Dispatcher dispatcher)
		{
			_executeOnMainThread = action => dispatcher.BeginInvoke(DispatcherPriority.Input, action);
			Servers = new ObservableCollection<Server>();
			FriendsList = new FriendsListViewModel();
			GetServerList();
		}

		private void GetServerList()
		{
			Servers.Clear();

			var getAllTask = Task.Factory.StartNew(() => ServerList.GetAll());

			getAllTask.ContinueWith(task =>
			{
				_executeOnMainThread(() =>
				{
					task.Result.ForEach(Servers.Add);
					UpdateServerDetails();
				});
			});
		}

		public FriendsListViewModel FriendsList
		{
			get { return _friendslist; }
			set
			{
				_friendslist = value;
				PropertyHasChanged("FriendsList");
			}
		}

		private void UpdateServerDetails()
		{
			ProcessedServersCount = 0;
			Servers
				.ToList(server =>
				        Task.Factory
				        	.StartNew(() =>
				        	          server.Update(_executeOnMainThread)
				        	)
				        	.ContinueWith(task =>
				        	              _executeOnMainThread(() => ProcessedServersCount++)
				        	)
				);
		}

		public ObservableCollection<Server> Servers { get; private set; }

		private int _processedServersCount;
		public int ProcessedServersCount
		{
			get { return _processedServersCount; }
			set
			{
				_processedServersCount = value;
				PropertyHasChanged("ProcessedServersCount");
			}
		}
	}
}