using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander.Ui.ServerList
{
	public class FiltersViewModel : ViewModelBase,
		IHandle<PublishFiltersRequest>
	{
		private long ?_maxPing;
		private string _name;
		private bool _hideEmpty;
		private bool _hideFull;
		private bool _supressPublish;
		private string _timeOfDay;
		private bool _hideUnresponsive;

		public FiltersViewModel()
		{
			LoadDefaults();
			Title = "filters";
		}

		public void LoadDefaults()
		{
			_supressPublish = true;
			MaxPing = null;
			Name = null;
			HideEmpty = false;
			HideUnresponsive = true;
			HideFull = false;
			TimeOfDay = "Any time of day";
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

		public bool HideUnresponsive
		{
			get { return _hideUnresponsive; }
			set
			{
				_hideUnresponsive = value;
				PropertyHasChanged("HideUnresponsive");
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

		private bool _hasArmor;
		public bool HasArmor
		{
			get { return _hasArmor; }
			set
			{
				_hasArmor = value;
				PropertyHasChanged("HasArmor");
				PublishFilter();
			}
		}

		private bool _hasThirdPerson;
		public bool HasThirdPerson
		{
			get { return _hasThirdPerson; }
			set
			{
				_hasThirdPerson = value;
				PropertyHasChanged("HasThirdPerson");
				PublishFilter();
			}
		}

		private bool _hasTracers;
		public bool HasTracers
		{
			get { return _hasTracers; }
			set
			{
				_hasTracers = value;
				PropertyHasChanged("HasTracers");
				PublishFilter();
			}
		}

		private bool _hasNameplates;
		public bool HasNameplates
		{
			get { return _hasNameplates; }
			set
			{
				_hasNameplates = value;
				PropertyHasChanged("HasNameplates");
				PublishFilter();
			}
		}

		private bool _hasCrosshairs;
		public bool HasCrosshairs
		{
			get { return _hasCrosshairs; }
			set
			{
				_hasCrosshairs = value;
				PropertyHasChanged("HasCrosshairs");
				PublishFilter();
			}
		}

		private bool _hasDeathMessages;
		public bool HasDeathMessages
		{
			get { return _hasDeathMessages; }
			set
			{
				_hasDeathMessages = value;
				PropertyHasChanged("HasDeathMessages");
				PublishFilter();
			}
		}

		private bool _hasScores;
		public bool HasScores
		{
			get { return _hasScores; }
			set
			{
				_hasScores = value;
				PropertyHasChanged("HasScores");
				PublishFilter();
			}
		}

		public void PublishFilter()
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
//
								if(HasArmor && !s.Info.Armor.Enabled)
								{
									return false;
								}

								if(HasThirdPerson && !s.Info.ThirdPerson.Enabled)
								{
									return false;
								}

								if(HasTracers && !s.Info.Tracers.Enabled)
								{
									return false;
								}

								if(HasNameplates && !s.Info.Nameplates.Enabled)
								{
									return false;
								}

								if(HasCrosshairs && !s.Info.Crosshairs.Enabled)
								{
									return false;
								}

								if(HasDeathMessages && !s.Info.DeathMessages.Enabled)
								{
									return false;
								}

								if(HasScores && !s.Info.Scores.Enabled)
								{
									return false;
								}

								if(HideUnresponsive && s.LastException != null)
								{
									return false;
								}

								return true;
			             	};
			App.Events.Publish(new FilterUpdated(filter));
		}

		#region Implementation of IHandle<PublishFiltersRequest>

		public void Handle(PublishFiltersRequest message)
		{
			PublishFilter();			
		}

		#endregion
	}

	public class PublishFiltersRequest
	{
	}
}