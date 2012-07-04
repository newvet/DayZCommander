using System;
using System.Deployment.Application;

namespace Dotjosh.DayZCommander.Core
{
	public class DayZCommanderUpdater : BindableBase
	{
		private bool _restartToApplyUpdate;

		public bool RestartToApplyUpdate
		{
			get { return _restartToApplyUpdate; }
			set
			{
				_restartToApplyUpdate = value;
				PropertyHasChanged("RestartToApplyUpdate");
			}
		}

		public void StartCheckingForUpdates()
		{
			var t = new System.Timers.Timer();
			t.Interval = TimeSpan.FromHours(2).TotalMilliseconds;
			t.Elapsed += (sender, args) => CheckForUpdates();
			CheckForUpdates();
			t.Start();
		}

		private void CheckForUpdates()
		{
			 if (ApplicationDeployment.IsNetworkDeployed)
			 {
			 	ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += (sender, args) =>
			 	{
					if(args.UpdateAvailable)
					{
						ApplicationDeployment.CurrentDeployment.UpdateCompleted += (o, eventArgs) =>
						{
							RestartToApplyUpdate = true;
						};
						ApplicationDeployment.CurrentDeployment.UpdateAsync();
					}
			 	};
				ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
			 }
		}
	}
}