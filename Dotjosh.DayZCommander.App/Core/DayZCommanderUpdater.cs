using System;
using System.IO;
using System.Reflection;
using Dotjosh.DayZCommander.Updater;

namespace Dotjosh.DayZCommander.App.Core
{
	public class DayZCommanderUpdater : BindableBase
	{
		private bool _restartToApplyUpdate;

		public void StartCheckingForUpdates()
		{
			CheckForUpdate();
  		}

		public bool RestartToApplyUpdate
		{
			get { return _restartToApplyUpdate; }
			set
			{
				_restartToApplyUpdate = value;
				PropertyHasChanged("RestartToApplyUpdate");
			}
		}

		public Version CurrentVersion
		{
			get { return Assembly.GetEntryAssembly().GetName().Version; }
		}

		private void CheckForUpdate()
		{
			var versionChecker = new VersionChecker();
			versionChecker.Complete += VersionCheckComplete;
			versionChecker.CheckForUpdate();
		}

		private void VersionCheckComplete(object sender, VersionCheckCompleteEventArgs args)
		{
			if(args.IsNew)
			{
				var extracter = new DownloadAndExtracter(args.Version);
				extracter.ExtractComplete += ExtractComplete;
				extracter.DownloadAndExtract();
			}
		}

		private void ExtractComplete(object sender, ExtractCompletedArgs args)
		{
			RestartToApplyUpdate = true;
		}
	}
}