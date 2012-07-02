using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui
{
	public class MainWindowViewModel : BindableBase, 
		IHandle<FilterByFriendRequest>,
		IHandle<FilterUpdated>
	{
		private readonly Action<Action> _executeOnMainThread;
		private ListCollectionView _servers;
		private int _processedServersCount;
		private List<Server> _rawServers;
		private Server _selectedServer;
		private List<Friend> _friendFilter = new List<Friend>();
		private ObservableCollection<Server> _rawObservableServers = new ObservableCollection<Server>();
		private Func<Server, bool> _filter;
		private bool _restartToApplyUpdate;

		public MainWindowViewModel(Dispatcher dispatcher)
		{
			App.Events.Subscribe(this);

			_executeOnMainThread = action => dispatcher.BeginInvoke(DispatcherPriority.Send, action);
			UpdateServerList();

			Servers = (ListCollectionView)CollectionViewSource.GetDefaultView(_rawObservableServers);
			Servers.Filter = Filter;
			SortByPing = true;

			CheckForUpdates();
		}

		private void CheckForUpdates()
		{
			 if (ApplicationDeployment.IsNetworkDeployed)
			 {
			 	ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += (sender, args) =>
			 	{
					if(args.UpdateAvailable)
					{
						ApplicationDeployment.CurrentDeployment.UpdateCompleted += (o, eventArgs) =>
						{
							RestartToApplyUpdate = true;
						};
						ApplicationDeployment.CurrentDeployment.UpdateAsync();
					}
			 	};
				ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
			 }
		}

		public bool RestartToApplyUpdate
		{
			get { return _restartToApplyUpdate; }
			set
			{
				_restartToApplyUpdate = value;
				PropertyHasChanged("RestartToApplyUpdate");
			}
		}

		public void UpdateServerList()
		{
			_rawObservableServers.Clear();
			PropertyHasChanged("TotalServerCount", "UnprocessedServersCount");
			Task.Factory.StartNew(() =>
			                      	{
			                      		_rawServers = ServerList.GetAll();
			                      		_executeOnMainThread(() => PropertyHasChanged("TotalServerCount", "UnprocessedServersCount"));
			                      		UpdateAllServers();
			                      	});
		}

		public bool SortByPing
		{
			get { return Servers != null && Servers.SortDescriptions.All(x => x.PropertyName == "Ping"); }
			set
			{
				Servers.SortDescriptions.Clear();
				if(value)
					Servers.SortDescriptions.Add(new SortDescription("Ping", ListSortDirection.Ascending));		

				PropertyHasChanged("SortByPing");
				PropertyHasChanged("SortByMostPlayers");
			}
		}

		public bool SortByMostPlayers
		{
			get { 
					return Servers != null 
						&& Servers.SortDescriptions.Any(x => x.PropertyName == "CurrentPlayers")
						&& Servers.SortDescriptions.Any(x => x.PropertyName == "Ping"); 
			}
			set
			{
				Servers.SortDescriptions.Clear();
				if(value)
				{
					Servers.SortDescriptions.Add(new SortDescription("CurrentPlayers", ListSortDirection.Descending));
					Servers.SortDescriptions.Add(new SortDescription("Ping", ListSortDirection.Ascending));
				}

				PropertyHasChanged("SortByPing");
				PropertyHasChanged("SortByMostPlayers");
			}
		}

		private bool Filter(object o)
		{
		
			var server = (Server)o;

			if(_friendFilter.Count > 0)
				return _friendFilter
							.SelectMany(f => f.Players)
							.Any(x => x.Server.IpAddress == server.IpAddress && x.Server.Port == server.Port);

			if(server.Ping == 0)
				return false;

			if(_filter != null)
				return _filter(server);

			return true;
		}

		public void UpdateAllServers()
		{
			ProcessedServersCount = 0;
			new Thread(() =>
			{
				for (int index = 0; index < _rawServers.Count; index++)
				{
					var server = _rawServers[index];
				
					new Thread(() =>
				                      		{
				                      			server.Update(_executeOnMainThread);
				                      			_executeOnMainThread(() =>
				                      		                     		{
				                      		                     			ProcessedServersCount++;
				                      		                     			_rawObservableServers.Add(server);
				                      		                     		});
				                      		}).Start();

					if(index % 70 == 0)
						Thread.Sleep(300);
				}
			}).Start();
		}

		public string CurrentVersion
		{
			get
			{

				XmlDocument xmlDoc = new XmlDocument();
				Assembly asmCurrent = System.Reflection.Assembly.GetExecutingAssembly();
				string executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

				xmlDoc.Load(executePath + ".manifest");
				string retval = string.Empty;
				if (xmlDoc.HasChildNodes)
				{
					retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value.ToString();
				}
				return new Version(retval).ToString();
			
			}
		}

		public int TotalServerCount
		{
			get { 
				return _rawServers != null 
				? _rawServers.Count
				:0; 
			}
		}

		public int UnprocessedServersCount
		{
			get { return TotalServerCount - ProcessedServersCount; }
		}


		public ListCollectionView Servers
		{
			get { return _servers; }
			private set
			{
				_servers = value;
				PropertyHasChanged("Servers");
				PropertyHasChanged("SortByPing");
			}
		}

		public Server SelectedServer
		{
			get { return _selectedServer; }
			set
			{
				_selectedServer = value;
				PropertyHasChanged("SelectedServer");
			}
		}

		public int ProcessedServersCount
		{
			get { return _processedServersCount; }
			set
			{
				_processedServersCount = value;
				PropertyHasChanged("ProcessedServersCount", "UnprocessedServersCount");
			}
		}

		public void Handle(FilterByFriendRequest message)
		{
			if(message.IsFiltered)
			{
				if(!_friendFilter.Any(f => f.Name == message.Friend.Name))
				{
					_friendFilter.Add(message.Friend);
				}
			}
			else
			{
				_friendFilter.RemoveAll(f => f.Name == message.Friend.Name);
			}
			Servers.Refresh();
		}

		public void Handle(FilterUpdated message)
		{
			_filter = message.Filter;
			Servers.Refresh();
		}

		public void UpdateSelectedServer()
		{
			if(SelectedServer != null)
			{
				Task.Factory
					.StartNew(() => SelectedServer.Update(_executeOnMainThread));
			}
		}
	}

	public class FilterUpdated
	{
		public Func<Server, bool> Filter { get; set; }

		public FilterUpdated(Func<Server, bool> filter)
		{
			Filter = filter;
		}
	}
}