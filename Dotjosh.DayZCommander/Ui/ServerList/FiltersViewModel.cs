using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui.ServerList
{
	public class FiltersViewModel : ViewModelBase
	{
		public FiltersViewModel()
		{
			Title = "filters";

			Filter = UserSettings.Current.Filter;
		}

		public Filter Filter { get; set; }
	}
}