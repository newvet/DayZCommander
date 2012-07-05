using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Dotjosh.DayZCommander.Core;
using Microsoft.Win32;

namespace Dotjosh.DayZCommander.Ui.ServerList
{
	public class ListViewModel : ViewModelBase,
		IHandle<ServersAdded>,
		IHandle<FilterUpdated>,
		IHandle<ServerUpdated>
	{
		private ListCollectionView _servers;
		private ObservableCollection<Server> _observableServers = new ObservableCollection<Server>();
		private DateTime? _lastLeftMouseDown;
		private Func<Server, bool> _filter;

		public ListViewModel()
		{
			ReplaceServers();
			Title = "servers";
		}

		private void ReplaceServers()
		{
			Servers = (ListCollectionView) CollectionViewSource.GetDefaultView(_observableServers);
			Servers.Filter = Filter;
			Servers.Refresh();
		}

		private bool Filter(object obj)
		{
			var server = (Server) obj;

			if(server.Ping == 0)
				return false;

			if(_filter != null)
				return _filter(server);
			return true;
		}

		public ListCollectionView Servers
		{
			get { return _servers; }
			set
			{
				_servers = value;
				PropertyHasChanged("Servers");
			}
		}

		public void Handle(FilterUpdated message)
		{
			_filter = message.Filter;
			Servers.Refresh();
		}

		public void Handle(ServerUpdated message)
		{
			if(_lastLeftMouseDown != null && DateTime.Now - _lastLeftMouseDown < TimeSpan.FromMilliseconds(750))
			{
				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(500));
					Execute.OnUiThread(() => Handle(message));
				});
			}
			else
			{
				_observableServers.Remove(message.Server);
				_observableServers.Add(message.Server);
			}
		}

		public void LeftMouseDown()
		{
			_lastLeftMouseDown = DateTime.Now;
		}

		public void Handle(ServersAdded message)
		{
		}


		public void JoinServer(Server server)
		{
			var arma2Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2", "main", "");
			var arma2OAPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", "");
			var arma2OaBetaExePath = Path.Combine(arma2OAPath, @"Expansion\beta\arma2oa.exe");

			if(string.IsNullOrWhiteSpace(arma2Path))
			{
				arma2Path = Path.Combine(new DirectoryInfo(arma2OAPath).Parent.FullName, "ArmA 2");
			}

			var arguments = @"";
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
						WorkingDirectory = arma2OAPath,
						UseShellExecute = true,
					}
			};
			p.Start();			
		}
	}
}