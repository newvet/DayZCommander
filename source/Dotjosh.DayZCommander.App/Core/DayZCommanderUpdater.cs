using System;
using System.Reflection;
using Dotjosh.DayZCommander.Updater;
using Timer = System.Timers.Timer;

namespace Dotjosh.DayZCommander.App.Core
{
	public class DayZCommanderUpdater : BindableBase
	{
		private string _status;
		private Version _latestVersion;
		public static readonly string STATUS_CHECKINGFORUPDATES = "Checking for updates";
		public static readonly string STATUS_DOWNLOADING = "Downloading";
		public static readonly string STATUS_UPTODATE = "Up To Date";
		public static readonly string STATUS_RESTARTTOAPPLY = "Restart to apply update";

		public Version CurrentVersion
		{
			get { return Assembly.GetEntryAssembly().GetName().Version; }
		}

		public Version LatestVersion
		{
			get { return _latestVersion; }
			set
			{
				_latestVersion = value;
				PropertyHasChanged("LatestVersion");
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				PropertyHasChanged("Status");
			}
		}

		public void CheckForUpdate()
		{
			Status = STATUS_CHECKINGFORUPDATES;

			var versionChecker = new VersionChecker();
			versionChecker.Complete += VersionCheckComplete;
			versionChecker.CheckForUpdate();
		}

		private void VersionCheckComplete(object sender, VersionCheckCompleteEventArgs args)
		{
			LatestVersion = args.Version;

			if(args.IsNew)
			{
				var extracter = new DownloadAndExtracter(args.Version);
				extracter.ExtractComplete += ExtractComplete;
				extracter.DownloadAndExtract();
				Status = STATUS_DOWNLOADING;
			}
			else
			{	
				Status = STATUS_UPTODATE;
			}
		}

		private void ExtractComplete(object sender, ExtractCompletedArgs args)
		{
			Status = STATUS_RESTARTTOAPPLY;
		}
	}
}