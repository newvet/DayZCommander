namespace Dotjosh.DayZCommander.App.Ui
{
	public class UpdatesViewModel : ViewModelBase
	{
		private bool _isVisible;

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}
		 
	}
}