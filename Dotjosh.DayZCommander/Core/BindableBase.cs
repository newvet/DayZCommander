using System.ComponentModel;

namespace Dotjosh.DayZCommander.Core
{
	public class BindableBase : INotifyPropertyChanged
	{
		#region Implementation of INotifyPropertyChanged

		protected void PropertyHasChanged(string name)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}