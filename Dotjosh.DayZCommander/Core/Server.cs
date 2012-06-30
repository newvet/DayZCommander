using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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

		public int? CurrentPlayers
		{
			get { return GetSettingOrDefault("numplayers").TryIntNullable(); }
		}

		public int? MaxPlayers
		{
			get { return GetSettingOrDefault("maxplayers").TryIntNullable(); }
		}

		public static Regex ServerTimeRegex = new Regex(@"((GmT|Utc)[\s]*(?<Offset>([+]|[-])[\s]?[\d]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public DateTime? ServerTime
		{
			get
			{
				var name = GetSettingOrDefault("hostname");
				if(string.IsNullOrWhiteSpace(name))
					return null;

				var match = ServerTimeRegex.Match(name);
				if(!match.Success)
					return null;

				var offset = match.Groups["Offset"].Value.Replace(" ", "");
				var offsetInt = int.Parse(offset);

				return DateTime.UtcNow
							.AddHours(offsetInt);
			}
		}

		public SortedDictionary<string, string> Settings
		{
			get { return _settings; }
			private set
			{
				_settings = value;
				PropertyHasChanged("Settings");
				PropertyHasChanged("Name");
				PropertyHasChanged("CurrentPlayers");
				PropertyHasChanged("MaxPlayers");
				PropertyHasChanged("ServerTime");
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

		public int FreeSlots
		{
			get 
			{ 
				if(MaxPlayers != null && CurrentPlayers != null)
				{
					return (int) (MaxPlayers - CurrentPlayers);
				}
				return 0;
			}
		}

		public bool IsEmpty
		{
			get { return CurrentPlayers == null || CurrentPlayers == 0; }
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