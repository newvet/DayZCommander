using System.Collections.ObjectModel;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui
{
	public sealed class LeftPaneViewModel : BindableBase
	{
		private ViewModelBase _currentScreen;
		private readonly FiltersViewModel _filtersScreen;
		private readonly FriendsListViewModel _friendsListScreen;
		private ObservableCollection<ViewModelBase> _screens;

		public LeftPaneViewModel()
		{
			_filtersScreen = new FiltersViewModel();
			_friendsListScreen = new FriendsListViewModel();
			_screens = new ObservableCollection<ViewModelBase>(new ViewModelBase[] {_filtersScreen, _friendsListScreen});
			CurrentScreen = _filtersScreen;
		}

		public ViewModelBase CurrentScreen
		{
			get { return _currentScreen; }
			set
			{
				if(_currentScreen != null)
					_currentScreen.IsSelected = false;
				_currentScreen = value;
				if(_currentScreen != null)
					_currentScreen.IsSelected = true;
				PropertyHasChanged("CurrentScreen");
			}
		}

		public ObservableCollection<ViewModelBase> Screens
		{
			get { return _screens; }
			set
			{
				_screens = value;
				PropertyHasChanged("Screens");
			}
		}
	}

	public abstract class ViewModelBase : BindableBase
	{
		private string _title;
		private bool _isSelected;

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				PropertyHasChanged("Title");
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				PropertyHasChanged("IsSelected");
			}
		}
	}
}