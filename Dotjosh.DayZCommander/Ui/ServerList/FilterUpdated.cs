using System;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui.ServerList
{
	public class FilterUpdated
	{
		public Func<Server, bool> Filter { get; set; }

		public FilterUpdated(Func<Server, bool> filter)
		{
			Filter = filter;
		}
	}
}