using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Windows.Forms;

namespace Dotjosh.DayZCommander.InstallUtilities
{
	[RunInstaller(true)]
	public partial class InstallActions : Installer
	{
		private const string _mainExecutable = "DayZCommander.exe";

		public InstallActions()
		{
			InitializeComponent();
		}

		public override void Install(IDictionary stateSaver)
		{
			//MessageBox.Show("attach install");
			base.Install(stateSaver);
			CreateShortcuts();
		}

		public override void Uninstall(IDictionary savedState)
		{
			//MessageBox.Show("attach uninstall");
			DeleteShortcuts();
			base.Uninstall(savedState);
		}

		private void CreateShortcuts()
		{
			var allUsers = Context.Parameters["allusers"];
			CreateDayZCommanderShortcut(null);
			if(!string.IsNullOrEmpty(allUsers)
				&& allUsers.Equals("1"))
			{
				// Everyone.
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
			}
			else
			{
				// Just me.
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
			}
		}

		private static void DeleteShortcuts()
		{
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
		}

		private void CreateDayZCommanderShortcut(string shortcutPath)
		{
			var installDirectory = Context.Parameters["targetdir"].Replace("|", "");
			var workingDirectory = Path.Combine(installDirectory, @"Current");
			var targetPath = Path.Combine(workingDirectory, _mainExecutable);
			if(string.IsNullOrEmpty(shortcutPath))
			{
				shortcutPath = installDirectory;
			}
			var shortcutFile = Path.Combine(shortcutPath, "DayZ Commander.lnk");
			CreateShortcut(targetPath, workingDirectory, "DayZ Commander", shortcutFile);
		}

		private static void CreateShortcut(string target, string workingDirectory, string description, string shortcutFile)
		{
			using(var shortcut = new ShellLink())
			{
				shortcut.Target = target;
				shortcut.WorkingDirectory = workingDirectory;
				shortcut.Description = description;
				shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
				shortcut.Save(shortcutFile);
			}
		}

		private static void DeleteShortcut(string shortcutPath)
		{
			var shortcutFile = Path.Combine(shortcutPath, "DayZ Commander.lnk");
			if(!File.Exists(shortcutFile))
			{
				return;
			}
			File.Delete(shortcutFile);
		}
	}
}