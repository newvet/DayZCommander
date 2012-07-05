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

		public void GetAndUpdateAll()
		{
			GetAll(
				() => UpdateAll() 
				);
		}

		public void GetAll(Action uiThreadOnComplete)
		{
			Task.Factory.StartNew(() =>
			                    {
			                        var servers = GetAllSync();
									Execute.OnUiThread(() =>
														{
															Items = new ObservableCollection<Server>(servers);
															uiThreadOnComplete();
														});

			                    });
		}

		private List<Server> GetAllSync()
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

		public void UpdateAll()
		{
			if(_isUpdating)
				return;

			_isUpdating = true;
			ProcessedServersCount = 0;

			int processed = 0;

			var superQueue = new SuperQueue<Server>(100, server =>
			                                             	{
																try
																{
			                                             			server.Update();
																}
																finally
																{
																	processed++;
																}
			                                             	});

			Task.Factory.StartNew(() =>
			                      	{

			                      		for (int index = 0; index < Items.Count; index++)
			                      		{
			                      			var server = Items[index];
			                      			superQueue.EnqueueTask(server);
			                      		}

			                      		while (processed <= Items.Count)
			                      		{
			                      			Execute.OnUiThread(() =>
			                      			                   	{
			                      			                   		ProcessedServersCount = processed;
			                      			                   	});
			                      			if (processed == Items.Count)
			                      			{
			                      				superQueue.Dispose();
			                      				_isUpdating = false;
			                      				break;
			                      			}
			                      			Thread.Sleep(50);
			                      		}

			                      		Execute.OnUiThread(() =>
			                      			                {
			                      			                   	ProcessedServersCount = processed;
			                      			                });
			                      	});

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