using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Dotjosh.DayZCommander.Core
{
	public class Server : BindableBase
	{
		private readonly string _ipAddress;
		private readonly int _port;
		private readonly ServerQueryClient _queryClient;
		private long _ping;
		private ObservableCollection<Player> _players;
		private SortedDictionary<string, string> _settings;
		public string LastException = null;

		public Server(string ipAddress, int port)
		{
			_ipAddress = ipAddress;
			_port = port;
			_queryClient = new ServerQueryClient(this, ipAddress, port);
			Settings = new SortedDictionary<string, string>();
			Players = new ObservableCollection<Player>();
			Info = new ServerInfo(null, null);
		}

		public string Name
		{
			get
			{
				if(LastException != null)
				{
					return "Server did not respond.";
				}
				return GetSettingOrDefault("hostname") ?? "Loading...";
			}
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
		private ServerInfo _info;

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
				Info = new ServerInfo((ServerDifficulty?) Difficulty, Name);
				PropertyHasChanged("Settings");
				PropertyHasChanged("Name");
				PropertyHasChanged("CurrentPlayers");
				PropertyHasChanged("MaxPlayers");
				PropertyHasChanged("ServerTime");
				PropertyHasChanged("Difficulty");
			}
		}

		public ServerInfo Info
		{
			get { return _info; }
			private set
			{
				_info = value;
				PropertyHasChanged("Info");
			}
		}

		public long Ping
		{
			get
			{
				if(LastException != null)
				{
					return 10 * 1000;
				}
				return _ping;
			}
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

		public int? Difficulty
		{
			get { return GetSettingOrDefault("difficulty").TryIntNullable(); }
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

		public int Port
		{
			get { return _port; }
		}

		public bool? IsNight
		{
			get
			{
				var serverTime = ServerTime;
				if(serverTime == null)
					return null;

				return serverTime.Value.Hour < 5 || serverTime.Value.Hour > 19;
			}
		}

		public bool Passworded
		{
			get { return GetSettingOrDefault("password").TryInt() > 0; }
		}

		private string GetSettingOrDefault(string settingName)
		{
			if (Settings.ContainsKey(settingName))
			{
				return Settings[settingName];
			}
			return null;
		}

		public void Update()
		{
			try
			{
				var serverResult = _queryClient.Execute();
				Execute.OnUiThread(() =>
				                    	{
											App.Events.Publish(new PlayersChangedEvent(Players, serverResult.Players));
				                    		Players = new ObservableCollection<Player>(serverResult.Players.OrderBy(x => x.Name));
				                    		LastException = null;
				                    		Settings = serverResult.Settings;
				                    		Ping = serverResult.Ping;
											App.Events.Publish(new ServerUpdated(this));
				                    	});
			}
			catch (Exception ex)
			{
				Execute.OnUiThread(() =>
				                    	{
											LastException = ex.Message;
											PropertyHasChanged("Name", "Ping");
											App.Events.Publish(new ServerUpdated(this));
				                    	});
				
			}
		}
	}
}