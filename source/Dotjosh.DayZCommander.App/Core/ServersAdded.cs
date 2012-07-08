using System.Collections.Generic;

namespace Dotjosh.DayZCommander.App.Core
{
	public class ServersAdded
	{
		public IEnumerable<Server> Servers { get; set; }

		public ServersAdded(IEnumerable<Server> servers)
		{
			Servers = servers;
		}
	}
}