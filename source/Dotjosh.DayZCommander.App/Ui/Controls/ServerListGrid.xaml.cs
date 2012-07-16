using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Controls
{
	/// <summary>
	/// Interaction logic for ServerListGrid.xaml
	/// </summary>
	public partial class ServerListGrid : UserControl
	{

		public ServerListGrid()
		{
			InitializeComponent();
		}

		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var control = e.OriginalSource as FrameworkElement;
			if(control != null)
			{
				if(control.Name == "Refresh" || control.Name == "Favorite")
				{
					e.Handled = true;
					return;
				}
			}
			var server = (Server) ((Control) sender).DataContext;

			GameLauncher.JoinServer(server);
		}

		private void RowLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			App.Events.Publish(new DataGridRowSelected());
		}
	}

	public class DataGridRowSelected
	{
	}
}
