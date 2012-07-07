using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Ionic.Zip;
using NLog;

namespace Dotjosh.DayZCommander.Updater
{
	public class DownloadAndExtracter
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly Version _serverVersion;
		private readonly Uri _serverZipUri;
		private readonly string _tempDownloadFileLocation;
		private readonly string _tempExtractedLocation;
		public static readonly string PENDING_UPDATE_DIRECTORYNAME = "_pendingupdate";

		public DownloadAndExtracter(Version serverVersion)
		{
			_serverVersion = serverVersion;
			_serverZipUri = new Uri(String.Format("http://files.dayzcommander.com/releases/{0}.zip", _serverVersion));
			var uniqueToken = Guid.NewGuid().ToString();
			_tempDownloadFileLocation = Path.GetTempPath() + uniqueToken + ".zip";
			_tempExtractedLocation = Path.GetTempPath() + uniqueToken;
		}

		public string TempExtractedLocation
		{
			get { return _tempExtractedLocation; }
		}

		public event EventHandler<ExtractCompletedArgs> ExtractComplete;

		public void DownloadAndExtract()
		{
			var checkForUpdateClient = new WebClient();
			checkForUpdateClient.DownloadFileCompleted += DownloadFileComplete;
			checkForUpdateClient.DownloadFileAsync(_serverZipUri, _tempDownloadFileLocation);
		}

		private void DownloadFileComplete(object sender, AsyncCompletedEventArgs args)
		{
			if(args.Error != null)
			{
				return;
			}
			Extract();
		}

		private void Extract()
		{
			//Take advantage of async IO for the download, but start a thread for the extract and file operations
			new Thread(() =>
			           	{
							try
							{
								using (var zipFile = ZipFile.Read(_tempDownloadFileLocation))
								{
									zipFile.ExtractAll(_tempExtractedLocation, ExtractExistingFileAction.OverwriteSilently);
								}

								var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
								var targetSwapDirectory = Path.Combine(currentDirectory, PENDING_UPDATE_DIRECTORYNAME);

								if (Directory.Exists(targetSwapDirectory))
									Directory.Delete(targetSwapDirectory);

								Directory.Move(_tempExtractedLocation, targetSwapDirectory);

								Action action = OnExtractComplete;
								Application.Current.Dispatcher
									.BeginInvoke(action, DispatcherPriority.Background);
							}
							catch(Exception ex)
							{
								_logger.Error(ex);
							}
			           	}).Start();

		}

		private void OnExtractComplete()
		{
			if(ExtractComplete != null)
			{
				ExtractComplete(this, new ExtractCompletedArgs());
			}
		}
	}
}