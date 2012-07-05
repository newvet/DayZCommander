using System.Diagnostics;
using System.Text;

namespace Dotjosh.DayZCommander.Core
{
	public static class GameLauncher
	{
		public static void JoinServer(Server server)
		{
			var arguments = new StringBuilder();

			arguments.Append(" -noSplash -noFilePatching");
			arguments.Append(" -connect=" + server.IpAddress);
			arguments.Append(" -port=" + server.Port);
			arguments.AppendFormat(" \"-mod={0};expansion;expansion\\beta;expansion\\beta\\expansion;@DayZ\"", LocalMachineInfo.Arma2Path);

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
	}
}