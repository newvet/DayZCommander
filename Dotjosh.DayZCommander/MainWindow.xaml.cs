using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dotjosh.DayZCommander.Core;

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

			DataContext = new MainWindowViewModel();
		}
	}

	public class MainWindowViewModel
	{
		public MainWindowViewModel()
		{
			Servers = new ObservableCollection<Query.Server>();
			var q = new Query("94.242.227.14");
			var server = q.Execute();
			Servers.Add(server);
		}

		public ObservableCollection<Query.Server> Servers { get; private set; }
	}
}
