using System.Runtime.Serialization;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class GameOptions : BindableBase
	{
		private string _additionalStartupParameters;
		private bool _launchUsingSteam;
		private bool _windowedMode;
		private bool _multiGpu;

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
	}
}