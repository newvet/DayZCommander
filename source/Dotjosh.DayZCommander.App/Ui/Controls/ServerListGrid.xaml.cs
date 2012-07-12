using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.App.Ui.ServerList;

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
