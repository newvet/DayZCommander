using System;
using System.Collections.Generic;
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

namespace Dotjosh.DayZCommander.App.Ui
{
	/// <summary>
	/// Interaction logic for UpdatesView.xaml
	/// </summary>
	public partial class UpdatesView : UserControl
	{
		public UpdatesView()
		{
			InitializeComponent();
		}

		private void Done_Click(object sender, RoutedEventArgs e)
		{
			((UpdatesViewModel) DataContext).IsVisible = false;
		}

		private void CheckNow_Click(object sender, RoutedEventArgs e)
		{
			((UpdatesViewModel) DataContext).CheckForUpdates();
		}
	}
}
