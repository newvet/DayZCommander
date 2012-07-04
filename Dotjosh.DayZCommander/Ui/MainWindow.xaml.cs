using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Dotjosh.DayZCommander.Core;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Control = System.Windows.Controls.Control;

namespace Dotjosh.DayZCommander.Ui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainWindowViewModel(Dispatcher);

			var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			MaxHeight = screen.WorkingArea.Height;

		}

		private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MainWindow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(e.OriginalSource != VisualRoot)
				return;

			ToggleMaximized();
		}

		private void ToggleMaximized()
		{
			if(WindowState == WindowState.Normal)
			{
				var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
				MaxHeight = screen.WorkingArea.Height;
				WindowState = WindowState.Maximized;
			}
			else
			{
				WindowState = WindowState.Normal;
			}
		}

		private void JoinServer(Server server)
		{
			var arma2Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2", "main", "");
			var arma2OAPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", "");
			var arma2OaBetaExePath = Path.Combine(arma2OAPath, @"Expansion\beta\arma2oa.exe");

			if(string.IsNullOrWhiteSpace(arma2Path))
			{
				arma2Path = Path.Combine(new DirectoryInfo(arma2OAPath).Parent.FullName, "ArmA 2");
			}

			var arguments = @"";
			arguments += " -noSplash -noFilePatching";
			arguments += " -connect=" + server.IpAddress;
			arguments += " -port=" + server.Port;
			arguments += string.Format(" \"-mod={0};expansion;expansion\\beta;expansion\\beta\\expansion;@DayZ\"", arma2Path);
			var p = new Process
			{
				StartInfo =
					{
						FileName = arma2OaBetaExePath,
						Arguments = arguments,
						Verb = "runas",
						WorkingDirectory = arma2OAPath,
						UseShellExecute = true,
					}
			};
			p.Start();			
		}

		private void RefreshAll_Click(object sender, RoutedEventArgs e)
		{
			((MainWindowViewModel) DataContext).UpdateAllServers();
		}

		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var server = (Server) ((Control) sender).DataContext;

			JoinServer(server);
		}

		private void RowLeftButtonDown(object sender, RoutedEventArgs routedEventArgs)
		{
			((MainWindowViewModel) DataContext).LeftMouseDown();
		}
	}
}
