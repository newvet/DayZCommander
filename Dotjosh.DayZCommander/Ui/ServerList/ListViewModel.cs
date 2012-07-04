using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Data;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui.ServerList
{
	public class ListViewModel : ViewModelBase,
		IHandle<ServersAdded>,
		IHandle<FilterUpdated>,
		IHandle<ServerUpdated>
	{
		private ListCollectionView _servers;
		private ObservableCollection<Server> _observableServers = new ObservableCollection<Server>();
		private DateTime? _lastLeftMouseDown;
		private Func<Server, bool> _filter;

		public ListViewModel()
		{
			ReplaceServers();
			Title = "servers";
		}

		private void ReplaceServers()
		{
			Servers = (ListCollectionView) CollectionViewSource.GetDefaultView(_observableServers);
			Servers.Filter = Filter;
			Servers.Refresh();
		}

		private bool Filter(object obj)
		{
			var server = (Server) obj;

			if(server.Ping == 0)
				return false;

			if(_filter != null)
				return _filter(server);
			return true;
		}

		public ListCollectionView Servers
		{
			get { return _servers; }
			set
			{
				_servers = value;
				PropertyHasChanged("Servers");
			}
		}

		public void Handle(FilterUpdated message)
		{
			_filter = message.Filter;
			Servers.Refresh();
		}

		public void Handle(ServerUpdated message)
		{
			if(_lastLeftMouseDown != null && DateTime.Now - _lastLeftMouseDown < TimeSpan.FromMilliseconds(750))
			{
				new Thread(() =>
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(500));
					Execute.OnUiThread(() => Handle(message));
				}).Start();
			}
			else
			{
				_observableServers.Remove(message.Server);
				_observableServers.Add(message.Server);
			}
		}

		public void LeftMouseDown()
		{
			_lastLeftMouseDown = DateTime.Now;
		}

		public void Handle(ServersAdded message)
		{
		}
	}
}