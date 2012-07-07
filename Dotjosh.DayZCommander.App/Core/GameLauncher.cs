using System;
using System.Diagnostics;
using System.Text;
using NLog;

namespace Dotjosh.DayZCommander.App.Core
{
	public static class GameLauncher
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static void JoinServer(Server server)
		{
			var arguments = new StringBuilder();

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
			        					FileName = LocalMachineInfo.Arma2OABetaExe,
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
				var joinServerException = new JoinServerException(LocalMachineInfo.Arma2OABetaExe, arguments.ToString(), LocalMachineInfo.Arma2OAPath, ex);
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