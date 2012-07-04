using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dotjosh.DayZCommander.Ui.Friends
{
	/// <summary>
	/// Interaction logic for FriendsList.xaml
	/// </summary>
	public partial class ManageView : UserControl
	{
		public ManageView()
		{
			InitializeComponent();
		}

		private ManageViewModel ViewModel
		{
			get { return (ManageViewModel) DataContext; }
		}

		private void NewFriend(object sender, RoutedEventArgs e)
		{
			ViewModel.NewFriend();
			NewFriendName.Focus();
		}

		private void NewFriendName_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				NewFriendName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
				ViewModel.CreateFriend();
			}
		}

		private void NewFriendName_LostFocus(object sender, RoutedEventArgs e)
		{
		//	ViewModel.CancelNewFriend();
		}
	}
}
