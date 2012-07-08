using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using NLog;

namespace Dotjosh.DayZCommander.App.Core
{
	public static class GameLauncher
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static void JoinServer(Server server)
		{
			var arguments = new StringBuilder();

			string exePath;
			
			if(UserSettings.Current.GameOptions.LaunchUsingSteam)
			{
				exePath = Path.Combine(LocalMachineInfo.SteamPath, "steam.exe");
				if(!File.Exists(exePath))
				{
					MessageBox.Show("Could not find Steam, please adjust your options or check your Steam installation.");
					return;
				}
				
				 arguments.Append(" -applaunch 33930");
			}
			else
			{
				exePath = LocalMachineInfo.Arma2OABetaExe;
			}

			if(UserSettings.Current.GameOptions.MultiGpu)
			{
				arguments.Append(" -winxp");
			}

			if(UserSettings.Current.GameOptions.WindowedMode)
			{
				arguments.Append(" -window");
			}

			if(!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.AdditionalStartupParameters))
			{
				arguments.Append(" " + UserSettings.Current.GameOptions.AdditionalStartupParameters);
			}

			arguments.Append(" -noSplash -noFilePatching");
			arguments.Append(" -connect=" + server.IpAddress);
			arguments.Append(" -port=" + server.Port);
			arguments.AppendFormat(" \"-mod={0};expansion;expansion\\beta;expansion\\beta\\expansion;@DayZ\"", LocalMachineInfo.Arma2Path);


			try
			{
				var p = new Process
			        		{
			        			StartInfo =
			        				{
			        					FileName = exePath,
			        					Arguments = arguments.ToString(),
			        					WorkingDirectory = LocalMachineInfo.Arma2OAPath,
			        					UseShellExecute = true,
			        				}
			        		};
				arguments.Clear();
				p.Start();
			}
			catch(Exception ex)
			{
				var joinServerException = new JoinServerException(exePath, arguments.ToString(), LocalMachineInfo.Arma2OAPath, ex);
				_logger.Error(joinServerException);
			}

		}
	}

	public class JoinServerException : Exception
	{
		public JoinServerException(string fileName, string arguments, string workingDirectory, Exception exception) : base(
				"There was an error launching the game.\r\n"
				+ "File Name:" + fileName + "\r\n"
				+ "Arguments:" + arguments + "\r\n"
				+ "Working Directory:" + workingDirectory,
			exception)
		{

		}
	}
}