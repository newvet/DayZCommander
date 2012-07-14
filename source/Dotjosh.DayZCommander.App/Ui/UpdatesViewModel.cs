using System;
using System.ComponentModel;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui
{
	public class UpdatesViewModel : ViewModelBase
	{
		private bool _isVisible;
		private string STATUS_INPROGRESS = "STATUS_INPROGRESS";
		private string STATUS_ERROR = "STATUS_ERROR";
		private string STATUS_DEFAULT = "STATUS_DEFAULT";

		public UpdatesViewModel()
		{
			LocalMachineInfo = LocalMachineInfo.Current;
			DayZCommanderUpdater = new DayZCommanderUpdater();
			Arma2Updater = new Arma2Updater();
			DayZUpdater = new DayZUpdater();

			DayZCommanderUpdater.PropertyChanged += AnyModelPropertyChanged;
			Arma2Updater.PropertyChanged += AnyModelPropertyChanged;
			DayZUpdater.PropertyChanged += AnyModelPropertyChanged;

			CheckForUpdates();
		}

		private void AnyModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyHasChanged("Status");
		}

		public string Status
		{
			get
			{
				if(DayZCommanderUpdater.Status == DayZCommanderUpdater.STATUS_CHECKINGFORUPDATES
					|| Arma2Updater.Status == DayZCommanderUpdater.STATUS_CHECKINGFORUPDATES
					|| DayZUpdater.Status == DayZCommanderUpdater.STATUS_CHECKINGFORUPDATES)
				return DayZCommanderUpdater.STATUS_DOWNLOADING;

				if(DayZCommanderUpdater.VersionMismatch 
				   || Arma2Updater.VersionMismatch
				   || DayZUpdater.VersionMismatch)
					return DayZCommanderUpdater.STATUS_OUTOFDATE;

				if(!DayZCommanderUpdater.VersionMismatch 
					&& !Arma2Updater.VersionMismatch
					&& !DayZUpdater.VersionMismatch)
					return DayZCommanderUpdater.STATUS_UPTODATE;

				return "Error";
			}
		}

		public DayZCommanderUpdater DayZCommanderUpdater { get; private set; }
		public Arma2Updater Arma2Updater { get; private set; }
		public DayZUpdater DayZUpdater { get; private set; }
		public LocalMachineInfo LocalMachineInfo { get; private set; }

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
			LocalMachineInfo.Current.Update();
			DayZCommanderUpdater.CheckForUpdate();
			Arma2Updater.CheckForUpdate();
			DayZUpdater.CheckForUpdate();
		}		 
	}
}