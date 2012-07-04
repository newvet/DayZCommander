using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui
{
	public sealed class LeftPaneViewModel : BindableBase
	{
		private ViewModelBase _currentScreen;
		private readonly FiltersViewModel _filtersScreen;
		private readonly FriendsListViewModel _friendsListScreen;

		public LeftPaneViewModel()
		{
			_filtersScreen = new FiltersViewModel();
			_friendsListScreen = new FriendsListViewModel();
			CurrentScreen = _filtersScreen;
		}


		public ViewModelBase CurrentScreen
		{
			get { return _currentScreen; }
			set
			{
				_currentScreen = value;
				PropertyHasChanged("CurrentScreen");
			}
		}
	}

	public abstract class ViewModelBase : BindableBase
	{
		private string _title;

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				PropertyHasChanged("Title");
			}
		}
	}
}