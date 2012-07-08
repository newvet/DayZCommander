using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NLog;

namespace Dotjosh.DayZCommander.Updater
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public MainWindow()
		{
			InitializeComponent();

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			try
			{
				ApplyUpdate();
			}
			catch(Exception ex)
			{
				_logger.Error(ex);
				LaunchDayZCommander();
				Environment.Exit(0);
			}
		}

		private void ApplyUpdate()
		{
			var pendingUpdateDirectory = Path.Combine(App.ApplicationInstallDirectory, DownloadAndExtracter.PENDING_UPDATE_DIRECTORYNAME);

			var tempUpdatePath = Path.GetTempPath() + Guid.NewGuid();
			var lastVersionPath = Path.GetTempPath() + Guid.NewGuid();

			KillDayzCommanderProcesses();
			Directory.Move(pendingUpdateDirectory, tempUpdatePath);
			Directory.Move(App.ApplicationInstallDirectory, lastVersionPath);
			Directory.Move(tempUpdatePath, App.ApplicationInstallDirectory);
			LaunchDayZCommander();
		}

		private void LaunchDayZCommander()
		{
			var p = new Process()
			{
				StartInfo = new ProcessStartInfo()
				                {
				                   	CreateNoWindow = false,
				                   	UseShellExecute = true,
									WorkingDirectory = App.ApplicationInstallDirectory,
				                   	FileName = Path.Combine(App.ApplicationInstallDirectory, "DayZCommander.exe")
				                }
			};
			p.Start();
			Environment.Exit(0);
		}

		private void KillDayzCommanderProcesses()
		{
			try
				{
				var processes = Process.GetProcessesByName("DayZCommander.exe");
				if(processes.Length == 0)
					return;

				foreach (var process in processes)
				{
					process.Kill();
				}
			}
			catch(Exception)
			{
					
			}
		}
	}
}
