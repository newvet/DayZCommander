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
	public class MainWindowViewModel : BindableBase, 
		IHandle<RepublishFriendsRequest>
	{
		private DayZCommanderUpdater _updater;
		private Core.ServerList _serverList;
		private ViewModelBase _currentTab;
		private ObservableCollection<ViewModelBase> _tabs;

		public MainWindowViewModel()
		{
			Updater = new DayZCommanderUpdater();
			Updater.StartCheckingForUpdates();

			
			Tabs = new ObservableCollection<ViewModelBase>(new ViewModelBase[]
			                                               	{
			                                               		ServerListViewModel = new ServerListViewModel(),
																FriendsViewModel = new FriendsViewModel()
			                                               	});
			CurrentTab = Tabs.First();

			ServerList = new Core.ServerList();
			ServerList.GetAndUpdateAll();
		}

		public DayZCommanderUpdater Updater
		{
			get { return _updater; }
			set
			{
				_updater = value;
				PropertyHasChanged("Updater");
			}
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

		public string CurrentVersion
		{
			get
			{
				var xmlDoc = new XmlDocument();
				var asmCurrent = Assembly.GetExecutingAssembly();
				string executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

				xmlDoc.Load(executePath + ".manifest");
				string retval = string.Empty;
				if (xmlDoc.HasChildNodes)
				{
					retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value;
				}
				return new Version(retval).ToString();
			
			}
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