using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;
using Dotjosh.DayZCommander.Ui.Friends;
using Dotjosh.DayZCommander.Ui.ServerList;

namespace Dotjosh.DayZCommander.Ui
{
	public class MainWindowViewModel : ViewModelBase, 
		IHandle<RepublishFriendsRequest>
	{
		private Core.ServerList _serverList;
		private ViewModelBase _currentTab;
		private ObservableCollection<ViewModelBase> _tabs;

		public MainWindowViewModel()
		{
			Tabs = new ObservableCollection<ViewModelBase>(new ViewModelBase[]
			                                               	{
			                                               		ServerListViewModel = new ServerListViewModel(),
																FriendsViewModel = new FriendsViewModel()
			                                               	});
			CurrentTab = Tabs.First();

			Updater = new DayZCommanderUpdater();
			Updater.StartCheckingForUpdates();

			ServerList = new Core.ServerList();
			ServerList.GetAndUpdateAll();
		}

		public DayZCommanderUpdater Updater { get; private set; }
		public ServerListViewModel ServerListViewModel { get; set; }
		public FriendsViewModel FriendsViewModel { get; set; }

		public Core.ServerList ServerList
		{
			get { return _serverList; }
			set
			{
				_serverList = value;
				PropertyHasChanged("ServerList");
			}
		}

		public bool IsServerListSelected
		{
			get { return CurrentTab == ServerListViewModel; }
		}

		public bool IsFriendsSelected
		{
			get { return CurrentTab == FriendsViewModel; }
		}

		public ViewModelBase CurrentTab
		{
			get { return _currentTab; }
			set
			{
				if(_currentTab != null)
					_currentTab.IsSelected = false;
				_currentTab = value;
				if(_currentTab != null)
					_currentTab.IsSelected = true;
				PropertyHasChanged("CurrentTab", "IsFriendsSelected", "IsServerListSelected");
			}
		}

		public ObservableCollection<ViewModelBase> Tabs
		{
			get { return _tabs; }
			set
			{
				_tabs = value;
				PropertyHasChanged("Tabs");
			}
		}

		public void Handle(RepublishFriendsRequest message)
		{
			foreach(var server in ServerList.Items)
			{
				App.Events.Publish(new PlayersChangedEvent(server.Players, server.Players));
			}
		}
	}
}