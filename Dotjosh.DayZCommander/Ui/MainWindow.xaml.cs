using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dotjosh.DayZCommander.Core;
using Microsoft.Win32;

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

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			DataContext = new MainWindowViewModel(Dispatcher);
		}

		private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void LowestPing_Click(object sender, RoutedEventArgs e)
		{
			((MainWindowViewModel) DataContext).SortByPing = true;
		}

		private void MostPlayers_Click(object sender, RoutedEventArgs e)
		{
			((MainWindowViewModel) DataContext).SortByMostPlayers = true;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void MainWindow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
			}
			else
			{
				WindowState = WindowState.Normal;
			}
		}

		private void OnPlayerSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PlayersList.UnselectAll();
		}

		private void JoinServer(object sender, RoutedEventArgs e)
		{
			var server = ((MainWindowViewModel) DataContext).SelectedServer;

			var arma2Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2", "main", "");
			var arma2OAPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", "");
			var arma2OaBetaExePath = Path.Combine(arma2OAPath, @"Expansion\beta\arma2oa.exe");

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

		private void RefreshServer(object sender, RoutedEventArgs e)
		{
			((MainWindowViewModel) DataContext).UpdateSelectedServer();
		}

		private void RefreshAll_Click(object sender, RoutedEventArgs e)
		{
			((MainWindowViewModel) DataContext).UpdateAllServers();
		}
	}
}
