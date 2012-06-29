using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dotjosh.DayZCommander.Core
{
	public class ServerList
	{
		public static List<Server> GetAll()
		{
			ExecuteGSList("-u");
			return ExecuteGSList("-n arma2oapc -f \"mod LIKE '%@dayz%'\"")
				.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(line => new Server(
					                line.Substring(0, 15).Trim(),
					                line.Substring(16).Trim().TryInt()
					                )
				)
				.ToList();
		}

		private static string ExecuteGSList(string arguments)
		{
			var p = new Process
			{
				StartInfo =
					{
						UseShellExecute = false,
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden,
						RedirectStandardOutput = true,
						FileName = @"C:\Program Files (x86)\SIX Projects\Six Updater\tools\gslist.exe",
						Arguments = arguments
					}
			};
			p.Start();
			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return output;
		}
	}
}