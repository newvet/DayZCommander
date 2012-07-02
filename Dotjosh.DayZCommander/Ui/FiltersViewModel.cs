using System;
using System.Collections.Generic;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui
{
	public class FiltersViewModel : BindableBase
	{
		private long ?_maxPing;
		private string _name;
		private bool _hideEmpty;
		private bool _hideFull;
		private bool _supressPublish;
		private string _timeOfDay;
		private int _maximumDifficulty;

		public FiltersViewModel()
		{
			LoadDefaults();
		}

		public void LoadDefaults()
		{
			_supressPublish = true;
			MaxPing = null;
			Name = null;
			HideEmpty = false;
			HideFull = false;
			TimeOfDay = "Any time of day";
			MaximumDifficulty = 3;
			_supressPublish = false;
		}

		public long? MaxPing
		{
			get { return _maxPing; }
			set
			{
				if(value == 0)
					_maxPing = null;
				else
					_maxPing = value;
				PropertyHasChanged("MaxPing");
				PublishFilter();
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				PropertyHasChanged("Name");
				PublishFilter();
			}
		}

		public string TimeOfDay
		{
			get { return _timeOfDay; }
			set
			{
				_timeOfDay = value;
				PropertyHasChanged("TimeOfDay");
				PublishFilter();
			}
		}

		public IEnumerable<string> TimeOfDayOptions
		{
			get
			{
				yield return "Any time of day";
				yield return "Night only";
				yield return "Day only";
			}
		}

		public bool HideEmpty
		{
			get { return _hideEmpty; }
			set
			{
				_hideEmpty = value;
				PropertyHasChanged("HideEmpty");
				PublishFilter();
			}
		}

		public int MaximumDifficulty
		{
			get { return _maximumDifficulty; }
			set
			{
				_maximumDifficulty = value;
				PropertyHasChanged("MaximumDifficulty");
				PublishFilter();
			}
		}

		public bool HideFull
		{
			get { return _hideFull; }
			set
			{
				_hideFull = value;
				PropertyHasChanged("HideFull");
				PublishFilter();
			}
		}

		private void PublishFilter()
		{
			if(_supressPublish)
				return;

			Func<Server, bool> filter = s =>
			             	{
								if(MaxPing != null && s.Ping > MaxPing)
									return false;

								if(!string.IsNullOrWhiteSpace(Name))
								{
									if(s.Name.IndexOf(Name, StringComparison.OrdinalIgnoreCase) == -1)
										return false;
								}

								if(HideEmpty && s.CurrentPlayers == 0)
									return false;

								if(HideFull && s.FreeSlots == 0)
									return false;

								if(TimeOfDay == "Night only")
								{
									if(s.IsNight == null || s.IsNight == false)
										return false;
								}

								if(TimeOfDay == "Day only")
								{
									if(s.IsNight == null || s.IsNight == true)
										return false;							
								}

								if(s.Difficulty != null && s.Difficulty > MaximumDifficulty)
									return false;

								return true;
			             	};
			App.Events.Publish(new FilterUpdated(filter));
		}
	}
}