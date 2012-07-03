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

			DataContext = new FiltersViewModel();

		}

		private void Name_KeyUp(object sender, KeyEventArgs e)
		{
			NameEntry.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}
	}
}
