using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dotjosh.DayZCommander.Core;
using Microsoft.Win32;

namespace Dotjosh.DayZCommander.UI
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
			var server = (Server)((DataGridRow) sender).DataContext;

			var arma2Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2", "main", "");
			var arma2OAPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", "");
			var arma2OaBetaExePath = Path.Combine(arma2OAPath, @"Expansion\beta\arma2oa.exe");

			var arguments = @"-noSplash -noFilePatching -showScriptErrors -cpuCount=2 -exThreads=7";
			arguments += string.Format(@" -mod={0};EXPANSION;ca;@dayz"" ""-mod=Expansion\beta;Expansion\beta\Expansion""", arma2Path);
			arguments += " -connect=" + server.IpAddress;
			arguments += " -port=" + server.Port;
			
			var p = new Process
			{
				StartInfo =
					{
						FileName = arma2OaBetaExePath,
						Arguments = arguments,
						WorkingDirectory = arma2Path,
						UseShellExecute = true
						
					}
			};
			p.Start();
		}
	}
}
