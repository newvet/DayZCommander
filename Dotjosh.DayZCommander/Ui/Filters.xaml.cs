using System.Windows.Controls;
using System.Windows.Input;

namespace Dotjosh.DayZCommander.Ui
{
	/// <summary>
	/// Interaction logic for Filters.xaml
	/// </summary>
	public partial class Filters : UserControl
	{
		public Filters()
		{
			InitializeComponent();

		}

		private void Name_KeyUp(object sender, KeyEventArgs e)
		{
			NameEntry.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}
	}
}
