using System.ComponentModel;

namespace Dotjosh.DayZCommander.Core
{
	public class BindableBase : INotifyPropertyChanged
	{
		#region Implementation of INotifyPropertyChanged

		protected void PropertyHasChanged(params string[] names)
		{
			if(PropertyChanged != null)
			{
				foreach(var name in names)
					PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
				
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}