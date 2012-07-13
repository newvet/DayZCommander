using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui
{
	public class UpdatesViewModel : ViewModelBase
	{
		private bool _isVisible;

		public UpdatesViewModel()
		{
			DayZCommanderUpdater = new DayZCommanderUpdater();
			CheckForUpdates();
		}

		public DayZCommanderUpdater DayZCommanderUpdater { get; private set; }

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}

		public void CheckForUpdates()
		{
			DayZCommanderUpdater.CheckForUpdate();
		}		 
	}
}