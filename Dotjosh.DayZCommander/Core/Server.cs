using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dotjosh.DayZCommander.Core
{
	public class Server : BindableBase
	{
		private readonly ServerQueryClient _queryClient;

		public Server(string ipAddress, int port)
		{
			_queryClient = new ServerQueryClient(ipAddress, port);
			Settings = new SortedDictionary<string, string>();
			Players = new ObservableCollection<Player>();
		}

		public string Name
		{
			get { return GetSettingOrDefault("hostname") ?? "Loading..."; }
		}

		private string GetSettingOrDefault(string settingName)
		{
			if(Settings.ContainsKey(settingName))
			{
				return Settings[settingName];
			}
			return null;
		}

		private SortedDictionary<string, string> _settings;
		public SortedDictionary<string, string> Settings
		{
			get { return _settings; }
			private set
			{
				_settings = value;
				PropertyHasChanged("Settings");
				PropertyHasChanged("Name");
			}
		}

		private ObservableCollection<Player> _players;
		private long _ping;

		public ObservableCollection<Player> Players
		{
			get { return _players; }
			private set
			{
				_players = value;
				PropertyHasChanged("Players");
			}
		}

		public void Update(Action<Action> executeOnMainThread)
		{
			try
			{
				var serverResult = _queryClient.Execute();
					Players = new ObservableCollection<Player>(serverResult.Players);
					Settings = serverResult.Settings;
					Ping = serverResult.Ping;
			}
			catch(Exception ex)
			{
					
			}
		}

		public long Ping
		{
			get {
				return _ping;
			}
			set {
				_ping = value;
				PropertyHasChanged("Ping");
			}
		}
	}
}