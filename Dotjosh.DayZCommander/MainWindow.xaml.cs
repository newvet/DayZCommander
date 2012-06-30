using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dotjosh.DayZCommander
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
	}
}
