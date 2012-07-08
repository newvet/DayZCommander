using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui
{
	public class SettingsViewModel : ViewModelBase
	{
		private bool _isVisible;

		public SettingsViewModel()
		{
			Settings = UserSettings.Current;
		}

		public UserSettings Settings { get; set; }

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}

		public void Done()
		{
			IsVisible = false;
		}
	}
}