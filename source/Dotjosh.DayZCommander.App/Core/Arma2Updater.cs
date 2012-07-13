using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dotjosh.DayZCommander.App.Core
{
	public class Arma2Updater : BindableBase
	{
		private string _latestDownloadUrl;
		private Version _latestVersion;
		private bool _isChecking;
		private string _status;
		public const string ArmaBetaListingUrl = "http://www.arma2.com/beta-patch.php";

		public bool VersionMismatch
		{
			get
			{
				if(CurrentVersion == null)
					return true;
				if(LatestVersion == null)
					return false;
				
				return !CurrentVersion.Equals(LatestVersion);
			}
		}

		public void CheckForUpdate()
		{
			if(_isChecking)
				return;

			_isChecking = true;

			Status = DayZCommanderUpdater.STATUS_CHECKINGFORUPDATES;

			string responseBody;
			Version latestVersion = null;

			new Thread(() =>
			           	{
							try
							{
								Thread.Sleep(750);  //In case this happens so fast the UI looks like it didn't work
								if(!GameUpdater.HttpGet(ArmaBetaListingUrl, out responseBody))
								{
									Status = "Arma2.com not responding";
									return;
								}		
								var latestBetaUrlMatch = Regex.Match(responseBody, @"Latest\s+beta\s+patch:\s*<a\s+href\s*=\s*(?:'|"")([^'""]+)(?:'|"")", RegexOptions.IgnoreCase);
								if(!latestBetaUrlMatch.Success)
								{
									Status = "Latest patch url doesn't match pattern";
									return;
								}
								_latestDownloadUrl = latestBetaUrlMatch.Groups[1].Value;
								var latestBetaRevisionMatch = Regex.Match(_latestDownloadUrl, @"(\d+)\.(?:zip|rar)", RegexOptions.IgnoreCase);
								if(!latestBetaRevisionMatch.Success)
								{
									Status = "Latest patch doesn't match pattern";
									return;
								}
								var latestRevision = latestBetaRevisionMatch.Groups[1].Value.TryIntNullable();
								if(latestRevision != null)
								{
									latestVersion = new Version(1,60,0, (int) latestRevision);
									if(!latestVersion.Equals(CurrentVersion))
									{
										Status = DayZCommanderUpdater.STATUS_OUTOFDATE;
									}
									else
									{
										Status = DayZCommanderUpdater.STATUS_UPTODATE;
									}
								}	
								else
								{
									Status = "Coult not determine revision";
								}
				
							}
							catch(Exception ex)
							{
								Status = "Error getting version";
							}
							finally
							{
								_isChecking = false;
								LatestVersion = latestVersion;
							}
						}).Start();
		}

		public Version CurrentVersion
		{
			get { return LocalMachineInfo.Arma2OABetaVersion; }
		}

		public Version LatestVersion
		{
			get { return _latestVersion; }
			set
			{
				_latestVersion = value;
				Execute.OnUiThread(() => PropertyHasChanged("LatestVersion"));			
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				Execute.OnUiThread(() => PropertyHasChanged("Status", "VersionMismatch"));
			}
		}
	}
}