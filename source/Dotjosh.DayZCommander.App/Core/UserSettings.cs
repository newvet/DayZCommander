using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class UserSettings
	{
		private static UserSettings _current;

		[DataMember] private List<string> _friends = new List<string>();
		[DataMember] private Filter _filter = new Filter();
		[DataMember] private WindowSettings _windowSettings = new WindowSettings();
		[DataMember] private GameOptions _gameOptions = new GameOptions();

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
			if(_windowSettings == null)
				_windowSettings = new WindowSettings();
			if(_gameOptions == null)
				_gameOptions = new GameOptions();
		}

		public void Save()
		{
			using(var fs = GetSettingsFileStream(FileMode.Create))
			{
				var serializer = new DataContractSerializer(GetType());
				serializer.WriteObject(fs, this);
				fs.Flush(true);
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
	}
}