using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
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
			FriendsList = new FriendsListViewModel();
			GetServerList();
			_maxPing = 150;
		}

		private void GetServerList()
		{
			var getAllTask = Task.Factory.StartNew(() => ServerList.GetAll());

			getAllTask.ContinueWith(task =>
			{
				_executeOnMainThread(() =>
				                     	{
				                     		_rawServers = task.Result;
											PropertyHasChanged("TotalServerCount");
				                     		_rawObservableServers = new ObservableCollection<Server>();
				                     		Servers = CollectionViewSource.GetDefaultView(_rawObservableServers) as ListCollectionView;
											Servers.SortDescriptions.Add(new SortDescription("Ping", ListSortDirection.Ascending));
											Servers.Filter = Filter;
				                     		UpdateServerDetails();
				});
			});
		}

		private bool Filter(object o)
		{
			var server = o as Server;
			if(server.Ping > _maxPing)
				return false;
			return true;
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

		public long MaxPing
		{
			get { return _maxPing; }
			set
			{
				_maxPing = value;
				PropertyHasChanged("MaxPing");
				Servers.Refresh();
			}
		}

		private void UpdateServerDetails()
		{
			ProcessedServersCount = 0;
			_rawServers
				.ToList(server =>
				        Task.Factory
				        	.StartNew(() =>
				        	          server.Update(_executeOnMainThread)
				        	)
				        	.ContinueWith(task =>
				        	              _executeOnMainThread(() =>
				        	                                   	{
				        	                                   		ProcessedServersCount++;
				        	                                   		_rawObservableServers.Add(server);
				        	                                   	})
				        	)
				);
		}

		public int TotalServerCount
		{
			get { 
				return _rawServers != null 
				? _rawServers.Count
				:0; 
			}
		}


		private ListCollectionView _servers;
		public ListCollectionView Servers
		{
			get { return _servers; }
			private set
			{
				_servers = value;
				PropertyHasChanged("Servers");
			}
		}

		private int _processedServersCount;
		private List<Server> _rawServers;
		private long _maxPing;
		private ObservableCollection<Server> _rawObservableServers;

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