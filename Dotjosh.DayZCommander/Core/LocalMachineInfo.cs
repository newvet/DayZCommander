using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Dotjosh.DayZCommander.Core
{
	public static class LocalMachineInfo
	{
		static LocalMachineInfo()
		{
			try
			{
				if(IntPtr.Size == 8)
				{
					SetPathsX64();
				}
				else
				{
					SetPathsX86();
				}
				SetArma2OABetaVersion();
				SetDayZVersion();
			}
			catch(Exception ex)
			{
			}
		}

		public static string Arma2Path { get; private set; }
		public static string Arma2OAPath { get; private set; }
		public static string Arma2OABetaExe { get; private set; }
		public static Version Arma2OABetaVersion { get; private set; }
		public static Version DayZVersion { get; private set; }

		public static void Touch()
		{
		}

		private static void SetPathsX64()
		{
			const string arma2Registry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2";
			const string arma2OARegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA";

			SetPaths(arma2Registry, arma2OARegistry);
		}

		private static void SetPathsX86()
		{
			const string arma2Registry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2";
			const string arma2OARegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA";

			SetPaths(arma2Registry, arma2OARegistry);
		}

		private static void SetPaths(string arma2Registry, string arma2OARegistry)
		{
			// Set game paths.
			Arma2Path = (string)Registry.GetValue(arma2Registry, "main", "");
			Arma2OAPath = (string)Registry.GetValue(arma2OARegistry, "main", "");

			// If a user does not run a game the path will be null.
			if(string.IsNullOrWhiteSpace(Arma2Path)
				&& !string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				var pathInfo = new DirectoryInfo(Arma2OAPath);
				if(pathInfo.Parent != null)
				{
					Arma2Path = Path.Combine(pathInfo.Parent.FullName, "arma 2");
				}
			}
			if(!string.IsNullOrWhiteSpace(Arma2Path)
				&& string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				var pathInfo = new DirectoryInfo(Arma2Path);
				if(pathInfo.Parent != null)
				{
					Arma2OAPath = Path.Combine(pathInfo.Parent.FullName, "arma 2 operation arrowhead");
				}
			}

			if(string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				return;
			}

			Arma2OABetaExe = Path.Combine(Arma2OAPath, @"Expansion\beta\arma2oa.exe");
		}

		private static void SetArma2OABetaVersion()
		{
			var versionInfo = FileVersionInfo.GetVersionInfo(Arma2OABetaExe);
			Version version;
			if(Version.TryParse(versionInfo.ProductVersion, out version))
			{
				Arma2OABetaVersion = version;
			}
		}

		private static void SetDayZVersion()
		{
		}
	}
}