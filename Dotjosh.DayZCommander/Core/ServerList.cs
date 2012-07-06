using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dotjosh.DayZCommander.Core
{
	public class ServerList : BindableBase
	{
		private int _processedServersCount;
		private bool _downloadingServerList;
		private ObservableCollection<Server> _items;
		private bool _isUpdating;

		public ServerList()
		{
			Items = new ObservableCollection<Server>();
		}

		public ObservableCollection<Server> Items
		{
			get { return _items; }
			private set
			{
				_items = value;
				PropertyHasChanged("Items");
				App.Events.Publish(new ServersAdded(_items));
			}
		}

		public int UnprocessedServersCount
		{
			get { return Items.Count - ProcessedServersCount; }
		}

		public int ProcessedServersCount
		{
			get { return _processedServersCount; }
			set
			{
				_processedServersCount = value;
				PropertyHasChanged("ProcessedServersCount", "UnprocessedServersCount");
			}
		}

		public bool DownloadingServerList
		{
			get { return _downloadingServerList; }
			set
			{
				_downloadingServerList = value;
				PropertyHasChanged("DownloadingServerList");
			}
		}

		public void GetAndUpdateAll()
		{
			GetAll(
				() => UpdateAll() 
				);
		}

		public void GetAll(Action uiThreadOnComplete)
		{
			DownloadingServerList = true;
			new Thread(() =>
			                    {
			                        var servers = GetAllSync();
									Execute.OnUiThread(() =>
														{
															Items = new ObservableCollection<Server>(servers);
															DownloadingServerList = false;
															uiThreadOnComplete();
														});

			                    }).Start();
		}

		private List<Server> GetAllSync()
		{
			ExecuteGSList("-u");
			return ExecuteGSList("-n arma2oapc -f \"mod LIKE '%@dayz%'\" -X \\hostname")
				.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(line =>
				{
					var indexOfFirstSpace = line.IndexOf(" ");
					var fullIpAddressWithPort = line.Substring(0, indexOfFirstSpace).Split(':');
					var server = new Server(fullIpAddressWithPort[0], fullIpAddressWithPort[1].TryInt());

					server.Settings = new SortedDictionary<string, string>
					{
						{ "hostname", line.Substring(indexOfFirstSpace + 11) }
					};

					return server;
				}
				)
				.ToList();
		}

		private int _processed = 0;
		public void UpdateAll()
		{
			if(_isUpdating)
				return;

			object incrementLock = new object();

			_isUpdating = true;
			ProcessedServersCount = 0;

			_processed = 0;
			var totalCount = Items.Count;

			new Thread(() =>
			{
				try
				{
					while(_processed <= totalCount)
					{
						Execute.OnUiThread(() =>
						{
							ProcessedServersCount = _processed;
						});
						Thread.Sleep(50);
						if(_processed == totalCount)
						{
							_isUpdating = false;
							break;
						}
					}
				}
				finally
				{
					Execute.OnUiThread(() =>
					{
						ProcessedServersCount = totalCount;
					});
					_isUpdating = false;
				}
			}).Start();


			new Thread(() =>
			{
				for(var index = 0; index < totalCount; index++)
				{
					var server = Items[index];
					new Thread(() =>
					{
						try
						{
							server.Update();
						}
						finally
						{
							lock(incrementLock)
							{
								_processed++;
							}
						}
					}, 1).Start();
					Thread.Sleep(new Random().Next(5, 15));

					while(index - _processed > 200)
					{
						Thread.Sleep(20);
					}
				}
			}).Start();
		}

		private static string ExecuteGSList(string arguments)
		{
			var currentDirectoryUri = new Uri( Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().GetName().CodeBase));

			var currentDirectory = currentDirectoryUri.AbsolutePath;

			var p = new Process
			{
				StartInfo =
					{
						UseShellExecute = false,
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden,
						RedirectStandardOutput = true,
						FileName = Path.Combine(currentDirectory, @"GSList\gslist.exe"),
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