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
using Dotjosh.DayZCommander.Properties;

namespace Dotjosh.DayZCommander.Ui
{
	public class MainWindowViewModel : BindableBase, 
		IHandle<FilterByFriendRequest>,
		IHandle<FilterUpdated>,
		IHandle<ServerUpdatedEvent>,
		IHandle<RepublishFriendsRequest>
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
		private DateTime? _lastLeftMouseDown;
		private readonly LeftPaneViewModel _leftPaneViewModel;

		public MainWindowViewModel(Dispatcher dispatcher)
		{
			App.Events.Subscribe(this);

			_executeOnMainThread = action => dispatcher.BeginInvoke(DispatcherPriority.Send, action);
			UpdateServerList();

			Servers = (ListCollectionView)CollectionViewSource.GetDefaultView(_rawObservableServers);
			Servers.Filter = Filter;
			Servers.Refresh();

			UpdateSettings();

			StartCheckingForUpdates();

			_leftPaneViewModel = new LeftPaneViewModel();

			App.Events.Publish(new PublishFiltersRequest());
		}

		private void UpdateSettings()
		{
		   if (Settings.Default.UpgradeRequired)  
		   {  
			  Settings.Default.Upgrade();  
			  Settings.Default.UpgradeRequired = false;  
			  Settings.Default.Save();  
		   }  
		}

		private void StartCheckingForUpdates()
		{
			var t = new System.Timers.Timer();
			t.Interval = TimeSpan.FromHours(2).TotalMilliseconds;
			t.Elapsed += (sender, args) => CheckForUpdates();
			CheckForUpdates();
			t.Start();
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

		public LeftPaneViewModel LeftPaneViewModel
		{
			get { return _leftPaneViewModel; }
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
			                      		_executeOnMainThread(() => PropertyHasChanged("TotalServerCount", "UnprocessedServersCount", "HasFilteredServers"));
			                      		UpdateAllServers();
			                      	});
		}


		private bool Filter(object o)
		{
		
			var server = (Server)o;

			if(_friendFilter.Count > 0)
				return _friendFilter
							.SelectMany(f => f.Players)
							.Any(x => x.Server.IpAddress == server.IpAddress && x.Server.Port == server.Port);

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
				Assembly asmCurrent = Assembly.GetExecutingAssembly();
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

		public bool HasFilteredServers
		{
			get
			{
				if(_rawServers == null || Servers ==null)
					return false;
				return _rawServers.Count > Servers.Count;
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
				PropertyHasChanged("HasFilteredServers");
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
			PropertyHasChanged("HasFilteredServers");
		}

		public void Handle(FilterUpdated message)
		{
			_filter = message.Filter;
			Servers.Refresh();
			PropertyHasChanged("HasFilteredServers");
		}

		#region Implementation of IHandle<ServerUpdatedEvent>

		public void Handle(ServerUpdatedEvent message)
		{
			if(_lastLeftMouseDown != null && DateTime.Now - _lastLeftMouseDown < TimeSpan.FromMilliseconds(750))
			{
				new Thread(() =>
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(500));
					_executeOnMainThread(() => Handle(message));
				});
			}
			else
			{
				_rawObservableServers.Remove(message.Server);
				_rawObservableServers.Add(message.Server);
				PropertyHasChanged("HasFilteredServers");
			}
		}

		#endregion

		public void LeftMouseDown()
		{
			_lastLeftMouseDown = DateTime.Now;
		}

		#region Implementation of IHandle<RepublishFriendsRequest>

		public void Handle(RepublishFriendsRequest message)
		{
			foreach(var server in _rawServers)
			{
				App.Events.Publish(new PlayersChangedEvent(server.Players, server.Players));
			}
		}

		#endregion
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