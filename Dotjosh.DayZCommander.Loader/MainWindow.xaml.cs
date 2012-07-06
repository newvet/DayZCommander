using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;

namespace Dotjosh.DayZCommander.Loader
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();


			var currentDirectoryUri = new Uri( Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().GetName().CodeBase));

			var currentDirectory = currentDirectoryUri.AbsolutePath;

			var p = new Process
			{
				StartInfo =
					{
						UseShellExecute = true,
						FileName = Path.Combine(currentDirectory, @"_DeployedFiles\DayZCommander.exe"),
						Arguments = CurrentVersion
					}
			};
			p.EnableRaisingEvents = true;
			p.Exited += OnCommanderExited;
			p.Start();

			WindowState = WindowState.Minimized;

			new DayZCommanderUpdater()
				.StartCheckingForUpdates();

			p.WaitForExit();
		}

		private void OnCommanderExited(object sender, EventArgs eventArgs)
		{
			Application.Current.Shutdown();
		}


		public string CurrentVersion
		{
			get
			{
				var xmlDoc = new XmlDocument();
				var asmCurrent = Assembly.GetExecutingAssembly();
				string executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

				xmlDoc.Load(executePath + ".manifest");
				string retval = string.Empty;
				if (xmlDoc.HasChildNodes)
				{
					retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value;
				}
				return new Version(retval).ToString();
			
			}
		}
	}
}
