using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Dotjosh.DayZCommander.Core;

namespace Dotjosh.DayZCommander
{
	public class MainWindowViewModel
	{
		private readonly Action<Action> _executeOnMainThread;

		public MainWindowViewModel(Dispatcher dispatcher)
		{
			_executeOnMainThread = action => dispatcher.BeginInvoke(DispatcherPriority.Input, action);
			Servers = new ObservableCollection<Server>();
			GetServerList();
		}

		private void GetServerList()
		{
			Servers.Clear();

			var getAllTask = Task.Factory.StartNew(() => ServerList.GetAll());

			getAllTask.ContinueWith(task =>
			{
				_executeOnMainThread(() =>
				{
					task.Result.ForEach(Servers.Add);
					UpdateServerDetails();
				});
			});
		}

		private void UpdateServerDetails()
		{
			Servers
				.ForEach(server =>
				         ThreadPool.QueueUserWorkItem(state => server.Update(_executeOnMainThread))
				);
		}

		public ObservableCollection<Server> Servers { get; private set; }
	}
}