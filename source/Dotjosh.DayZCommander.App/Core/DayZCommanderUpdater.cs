using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using Dotjosh.DayZCommander.Updater;
using Timer = System.Timers.Timer;

namespace Dotjosh.DayZCommander.App.Core
{
	public class DayZCommanderUpdater : BindableBase
	{
		private bool _restartToApplyUpdate;
		private bool _checkingForUpdates;
		private Timer _timer;

		public void StartCheckingForUpdates()
		{
			_timer = new Timer();
			_timer.Interval = TimeSpan.FromHours(2).TotalMilliseconds;
			_timer.Elapsed += TimerOnElapsed;
			_timer.Start();

			CheckForUpdate();
  		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
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

		public bool CheckingForUpdates
		{
			get { return _checkingForUpdates; }
			set
			{
				_checkingForUpdates = value;
				PropertyHasChanged("CheckingForUpdates");
			}
		}

		public Version CurrentVersion
		{
			get { return Assembly.GetEntryAssembly().GetName().Version; }
		}

		private void CheckForUpdate()
		{
			CheckingForUpdates = true;

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
			else
			{
				new Thread(() =>
				{
					Thread.Sleep(5000); //Give the ui time to show that it was checking for updates
					Execute.OnUiThread(() => CheckingForUpdates = false);
				}).Start();
			}
		}

		private void ExtractComplete(object sender, ExtractCompletedArgs args)
		{
			RestartToApplyUpdate = true;
			CheckingForUpdates = false;
		}
	}
}