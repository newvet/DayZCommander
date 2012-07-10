using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class UserSettings
	{
		private static UserSettings _current;

		[DataMember] private List<string> _friends = new List<string>();
		[DataMember] private Filter _filter = new Filter();
		[DataMember] private WindowSettings _windowSettings = null; //This is null on purpose so the MainWindow view can set defaults if needed
		[DataMember] private GameOptions _gameOptions = new GameOptions();
		[DataMember] private List<FavoriteServer> _favorites;

		public List<string> Friends
		{
			get { return _friends; }
			set { _friends = value; }
		}

		public Filter Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		public WindowSettings WindowSettings
		{
			get { return _windowSettings; }
			set { _windowSettings = value; }
		}

		public GameOptions GameOptions
		{
			get { return _gameOptions; }
			set { _gameOptions = value; }
		}

		public List<FavoriteServer> Favorites
		{
			get { return _favorites; }
			set { _favorites = value; }
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext ctx)
		{
			Initialize();
		}

		private void Initialize()
		{
			if(_friends == null)
				_friends = new List<string>();
			if(_filter == null)
				_filter = new Filter();
			if(_gameOptions == null)
				_gameOptions = new GameOptions();
			if(_favorites == null)
				_favorites = new List<FavoriteServer>();
		}

		public void Save()
		{
			try
			{
				using(var fs = GetSettingsFileStream(FileMode.Create))
				{
					var serializer = new DataContractSerializer(GetType());
					serializer.WriteObject(fs, this);
					fs.Flush(true);
				}
			}
			catch(Exception)
			{
				
			}
		}

		private static UserSettings Load()
		{
			try
			{
				using(var fs = GetSettingsFileStream(FileMode.Open))
				{
					using(var reader = new StreamReader(fs))
					{
						var rawXml = reader.ReadToEnd();
						if(string.IsNullOrWhiteSpace(rawXml))
						{
							return new UserSettings();
						}
						else
						{
							return LoadFromXml(XDocument.Parse(rawXml));
						}
					}
				}
			}
			catch(FileNotFoundException)
			{
				return new UserSettings();
			}
		}

		private static FileStream GetSettingsFileStream(FileMode fileMode)
		{
			return new FileStream(SettingsPath, fileMode);		
		}

		public static UserSettings Current
		{
			get
			{
				if(_current == null)
				{
					try
					{
						_current = Load();
					}
					catch(Exception ex)
					{
						_current = new UserSettings();
					}
				}
				return _current;
			}
		}

		private static string SettingsPath
		{
			get
			{
				var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				var dayzCommanderAppDataDirectory = new DirectoryInfo( Path.Combine(appDataFolder, "DayZCommander") );
				if(!dayzCommanderAppDataDirectory.Exists)
					dayzCommanderAppDataDirectory.Create();
				return Path.Combine(dayzCommanderAppDataDirectory.FullName, "settings.xml");
			}
		}

		private static UserSettings LoadFromXml(XDocument xDocument)
		{
			var serializer = new DataContractSerializer(typeof(UserSettings));
			return (UserSettings)serializer.ReadObject(xDocument.CreateReader());
		}

		public bool IsFavorite(Server server)
		{
			return _favorites.Any(f => f.Matches(server));
		}

		public void AddFavorite(Server server)
		{
			if(_favorites.Any(f => f.Matches(server)))
				return;
			_favorites.Add(new FavoriteServer(server));
			App.Events.Publish(new FavoritesUpdated(server));
			Save();
		}

		public void RemoveFavorite(Server server)
		{
			var favorite = _favorites.FirstOrDefault(f => f.Matches(server));
			if(favorite == null)
				return;
			_favorites.Remove(favorite);
			App.Events.Publish(new FavoritesUpdated(server));
			Save();
		}
	}

	public class FavoritesUpdated
	{
		public Server Server { get; set; }

		public FavoritesUpdated(Server server)
		{
			Server = server;
		}
	}
}