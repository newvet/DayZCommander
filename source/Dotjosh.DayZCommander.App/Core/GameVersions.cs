using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Dotjosh.DayZCommander.App.Core
{
	public static class GameVersions
	{
		public static string BuildArma2OAExePath(string arma2OAPath)
		{
			return Path.Combine(arma2OAPath, @"arma2oa.exe");
		}

		public static Version ExtractArma2OABetaVersion(string arma2OAExePath)
		{
			if(!File.Exists(arma2OAExePath))
				return null;

			var versionInfo = FileVersionInfo.GetVersionInfo(arma2OAExePath);
			Version version;
			if(Version.TryParse(versionInfo.ProductVersion, out version))
			{
				return version;
			}
			return null;
		}

		public static Version ExtractDayZVersion(string dayZPath)
		{
			var changeLogPath = Path.Combine(dayZPath, "dayz_changelog.txt");
			if(!File.Exists(changeLogPath))
			{
				return null;
			}
			var changeLogLines = File.ReadAllLines(changeLogPath);
			foreach(var changeLogLine in changeLogLines)
			{
				if(!changeLogLine.Contains("* dayz_code"))
				{
					continue;
				}

				var match = Regex.Match(changeLogLine, @"\d(?:\.\d){1,3}");
				if(!match.Success)
				{
					continue;
				}
				Version version;
				if(Version.TryParse(match.Value, out version))
				{
					return version;
				}
			}
			return null;
		}
	}
}