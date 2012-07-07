using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using NLog;

namespace Dotjosh.DayZCommander.App
{
	public partial class App : Application
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static EventAggregator Events = new EventAggregator();
		private bool _isUncaughtUiThreadException;

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += UncaughtThreadException;
			DispatcherUnhandledException += UncaughtUiThreadException;

			base.OnStartup(e);
		}

		private void UncaughtUiThreadException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			_isUncaughtUiThreadException = true;
			_logger.Fatal(e.Exception);
			var messageBoxResult = MessageBox.Show(
				"It wasn't your fault, but something went really wrong!\r\nWould you like me to try and restart DayZ Commander for you?",
				"Oh noes",
				MessageBoxButton.YesNo);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				e.Handled = true;
				//System.Windows.Forms.Application.Restart();
				Current.Shutdown();
			}
		}

		private void UncaughtThreadException(object sender, UnhandledExceptionEventArgs e)
		{
			if(_isUncaughtUiThreadException)
				return;
			var exception = e.ExceptionObject as Exception;
			_logger.Fatal(exception);
		}
	}
}
