using System.Runtime.Serialization;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class GameOptions : BindableBase
	{
		[DataMember] private string _additionalStartupParameters;
		[DataMember] private bool _launchUsingSteam;
		[DataMember] private bool _windowedMode;
		[DataMember] private bool _multiGpu;
        [DataMember] private bool _CloseDayZCommander;
		[DataMember] private string _arma2DirectoryOverride;
		[DataMember] private string _arma2OaDirectoryOverride;
        [DataMember] private string _DayZDirectoryOverride;

		public string AdditionalStartupParameters
		{
			get { return _additionalStartupParameters; }
			set
			{
				_additionalStartupParameters = value;
				PropertyHasChanged("AdditionalStartupParameters");
				UserSettings.Current.Save();
			}
		}
		
		public bool LaunchUsingSteam
		{
			get { return _launchUsingSteam; }
			set
			{
				_launchUsingSteam = value;
				PropertyHasChanged("LaunchUsingSteam");
				UserSettings.Current.Save();
			}
		}

		public bool WindowedMode
		{
			get { return _windowedMode; }
			set
			{
				_windowedMode = value;
				PropertyHasChanged("WindowedMode");
				UserSettings.Current.Save();
			}
		}

		public bool MultiGpu
		{
			get { return _multiGpu; }
			set
			{
				_multiGpu = value;
				PropertyHasChanged("MultiGpu");
				UserSettings.Current.Save();
			}
		}

        public bool CloseDayZCommander
        {
            get { return _CloseDayZCommander; }
            set
            {
                _CloseDayZCommander = value;
                PropertyHasChanged("CloseDayZCommander");
                UserSettings.Current.Save();
            }
        }

		public string Arma2DirectoryOverride
		{
			get { return _arma2DirectoryOverride; }
			set
			{
				_arma2DirectoryOverride = value;
				PropertyHasChanged("Arma2DirectoryOverride");
				UserSettings.Current.Save();
			}
		}

		public string Arma2OADirectoryOverride
		{
			get { return _arma2OaDirectoryOverride; }
			set
			{
				_arma2OaDirectoryOverride = value;
				PropertyHasChanged("Arma2OADirectoryOverride");
				UserSettings.Current.Save();
			}
		}
        public string DayZDirectoryOverride
        {
            get { return _DayZDirectoryOverride; }
            set
            {
                _DayZDirectoryOverride = value;
                PropertyHasChanged("DayZDirectoryOverride");
                UserSettings.Current.Save();
            }
        }
	}
}