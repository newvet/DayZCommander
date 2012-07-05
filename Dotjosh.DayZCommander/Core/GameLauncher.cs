using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Dotjosh.DayZCommander.Core
{
	public class GameLauncher
	{
		private static Dictionary<string, string> _armaOARegistryLocations = new Dictionary<string, string>()
        {
            { "x86", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA"},
            { "x64", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA" }
        };

        private static Dictionary<string, string> _armaRegistryLocations = new Dictionary<string, string>()
        {
            { "x86", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2"},
            { "x64", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2" }
        };

		private static string _steamRegistryLocation = "SOFTWARE\\Valve\\Steam";
	
		private static string CpuArchitecture
		{
			get
			{
				switch(IntPtr.Size)
				{
					case 4:
						return "x86";
					case 8:
						return "x64";
					default:
						return "x86";
				}
			}
		}

		private static string Arma2Path
		{
			get
			{

				return (string) Registry.GetValue(_armaRegistryLocations[CpuArchitecture], "main", "");
			}
		}

		private static string Arma2OAPath
		{
			get
			{
				return (string) Registry.GetValue(_armaOARegistryLocations[CpuArchitecture], "main", "");
			}
		}

		public static void JoinServer(Server server)
		{
			var arma2Path = Arma2Path;
			var arma2OaPath = Arma2OAPath;
			var arma2OaBetaExePath = Path.Combine(arma2OaPath, @"Expansion\beta\arma2oa.exe");

			if(string.IsNullOrWhiteSpace(arma2Path))
			{
				arma2Path = Path.Combine(new DirectoryInfo(arma2OaPath).Parent.FullName, "ArmA 2");
			}

			var arguments = @"";

//			var installedWithSteam = arma2OaBetaExePath.IndexOf("steamapps") > 0;

			arguments += " -noSplash -noFilePatching";
			arguments += " -connect=" + server.IpAddress;
			arguments += " -port=" + server.Port;
			arguments += string.Format(" \"-mod={0};expansion;expansion\\beta;expansion\\beta\\expansion;@DayZ\"", arma2Path);

			var p = new Process
			{
				StartInfo =
					{
						FileName = arma2OaBetaExePath,
						Arguments = arguments,
						WorkingDirectory = arma2OaPath,
						UseShellExecute = true,
					}
			};
			p.Start();			
		}
	}
}