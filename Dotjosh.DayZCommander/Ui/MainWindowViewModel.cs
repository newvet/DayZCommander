using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;
using Dotjosh.DayZCommander.Ui.Friends;
using Dotjosh.DayZCommander.Ui.ServerList;
using Microsoft.Win32;

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

			ServerList = new Core.ServerList();
			ServerList.GetAndUpdateAll();

			CurrentVersion = App.CurrentVersion;
		}

		public Core.ServerList ServerList
		{
			get { return _serverList; }
			set
			{
				_serverList = value;
				PropertyHasChanged("ServerList");
			}
		}


		public static string CurrentVersion { get; private set; }

		public bool IsServerListSelected
		{
			get { return CurrentTab == ServerListViewModel; }
		}
		public ServerListViewModel ServerListViewModel { get; set; }
		public bool IsFriendsSelected
		{
			get { return CurrentTab == FriendsViewModel; }
		}
		public FriendsViewModel FriendsViewModel { get; set; }

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