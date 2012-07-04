using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui
{
	/// <summary>
	/// Interaction logic for FriendsList.xaml
	/// </summary>
	public partial class FriendsList : UserControl
	{
		public FriendsList()
		{
			InitializeComponent();
		}

		private FriendsListViewModel ViewModel
		{
			get { return (FriendsListViewModel) DataContext; }
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
			ViewModel.CancelNewFriend();
		}
	}
}
