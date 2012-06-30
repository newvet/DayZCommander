using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dotjosh.DayZCommander.Core
{
	public class Server : BindableBase
	{
		private readonly string _ipAddress;
		private readonly ServerQueryClient _queryClient;
		private long _ping;
		private ObservableCollection<Player> _players;
		private SortedDictionary<string, string> _settings;
		private string LastException = null;

		public Server(string ipAddress, int port)
		{
			_ipAddress = ipAddress;
			_queryClient = new ServerQueryClient(this, ipAddress, port);
			Settings = new SortedDictionary<string, string>();
			Players = new ObservableCollection<Player>();
		}

		public string Name
		{
			get { return LastException ?? GetSettingOrDefault("hostname") ?? "Loading..."; }
		}

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

		public long Ping
		{
			get { return _ping; }
			set
			{
				_ping = value;
				PropertyHasChanged("Ping");
			}
		}

		public ObservableCollection<Player> Players
		{
			get { return _players; }
			private set
			{
				_players = value;
				PropertyHasChanged("Players");
			}
		}

		public string IpAddress
		{
			get { return _ipAddress; }
		}

		private string GetSettingOrDefault(string settingName)
		{
			if (Settings.ContainsKey(settingName))
			{
				return Settings[settingName];
			}
			return null;
		}

		public void Update(Action<Action> executeOnMainThread)
		{
			try
			{
				var serverResult = _queryClient.Execute();
				executeOnMainThread(() =>
				                    	{
											App.Events.Publish(new PlayersChangedEvent(Players, serverResult.Players));
				                    		Players = new ObservableCollection<Player>(serverResult.Players);
				                    		LastException = null;
				                    		Settings = serverResult.Settings;
				                    		Ping = serverResult.Ping;
				                    	});
			}
			catch (Exception ex)
			{
				executeOnMainThread(() =>
				                    	{
											LastException = ex.Message;
											PropertyHasChanged("Name");
				                    	});
				
			}
		}
	}

	
}