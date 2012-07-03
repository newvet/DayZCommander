using System;

namespace Dotjosh.DayZCommander.Core
{
	public class ServerInfo
	{
		public ServerSetting Armor { get; private set; }
		public ServerSetting ThirdPerson { get; private set; }
		public ServerSetting Tracers { get; private set; }
		public ServerSetting Nameplates { get; private set; }
		public ServerSetting Crosshairs { get; private set; }
		public ServerSetting DeathMessages { get; private set; }
		public ServerSetting Scores { get; private set; }

		public ServerInfo(ServerDifficulty? difficulty, string serverName)
		{
			SetDefaults(); //Null values suck

			if(difficulty == null)
			{
				ParseName(serverName);
				return;
			}

			switch(difficulty.Value)
			{
				case ServerDifficulty.Recruit:
					SetRecruitDefaults();
					break;
				case ServerDifficulty.Regular:
					SetRegularDefaults();
					break;
				case ServerDifficulty.Veteran:
					SetVeteranDefaults();
					break;
				case ServerDifficulty.Expert:
					SetExpertDefaults();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			ParseName(serverName);
		}

		private void SetDefaults()
		{
			Armor = new ServerSetting();
			ThirdPerson = new ServerSetting();
			Tracers = new ServerSetting();
			Nameplates = new ServerSetting();
			Crosshairs = new ServerSetting();
			DeathMessages = new ServerSetting();
			Scores = new ServerSetting();
		}

		private void SetRecruitDefaults()
		{
			Armor = new ServerSetting { Enabled = true };
			ThirdPerson = new ServerSetting { Enabled = true };
			Tracers = new ServerSetting { Enabled = true };
			Nameplates = new ServerSetting { Enabled = true };
			Crosshairs = new ServerSetting { Enabled = true };
			DeathMessages = new ServerSetting { Enabled = true };
			Scores = new ServerSetting { Enabled = true };
		}

		private void SetRegularDefaults()
		{
			Armor = new ServerSetting { Enabled = true };
			ThirdPerson = new ServerSetting { Enabled = true };
			Tracers = new ServerSetting { Enabled = false }; // Forum says it on, but game defaults say off.
			Nameplates = new ServerSetting { Enabled = true };
			Crosshairs = new ServerSetting { Enabled = true };
			DeathMessages = new ServerSetting { Enabled = true };
			Scores = new ServerSetting { Enabled = true };
		}

		private void SetVeteranDefaults()
		{
			Armor = new ServerSetting { Enabled = false, Confirmed = true };
			ThirdPerson = new ServerSetting { Enabled = true };
			Tracers = new ServerSetting { Enabled = false };
			Nameplates = new ServerSetting { Enabled = false, Confirmed = true };
			Crosshairs = new ServerSetting { Enabled = false };
			DeathMessages = new ServerSetting { Enabled = true };
			Scores = new ServerSetting { Enabled = true }; // Game defaults to on.
		}

		private void SetExpertDefaults()
		{
			Armor = new ServerSetting { Enabled = false, Confirmed = true };
			ThirdPerson = new ServerSetting { Enabled = false, Confirmed = true };
			Tracers = new ServerSetting { Enabled = false, Confirmed = true };
			Nameplates = new ServerSetting { Enabled = false, Confirmed = true };
			Crosshairs = new ServerSetting { Enabled = false, Confirmed = true };
			DeathMessages = new ServerSetting { Enabled = false };
			Scores = new ServerSetting { Enabled = true }; // Game defaults to on.
		}

		private void ParseName(string serverName)
		{
		}
	}
}