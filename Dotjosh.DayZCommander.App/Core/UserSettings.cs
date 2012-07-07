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
		private const string SETTINGS_FILE_NAME = "settings.xml";
		private static UserSettings _current;

		public UserSettings()
		{
			Initialize();
		}

		[DataMember]
		public List<string> Friends { get; set; }

		[DataMember]
		public Filter Filter { get; set; }

		[DataMember]
		public WindowSettings WindowSettings { get; set; }

		public void Save()
		{
			using(var fs = GetSettingsFileStream(FileMode.Create))
			{
				var serializer = new DataContractSerializer(GetType());
				serializer.WriteObject(fs, this);
				fs.Flush(true);
			}
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext ctx)
		{
			Initialize();
		}

		private void Initialize()
		{
			Friends = new List<string>();
			Filter = new Filter();
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