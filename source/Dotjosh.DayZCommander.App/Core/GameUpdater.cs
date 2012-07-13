using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using FtpLib;
using SharpCompress.Common;
using SharpCompress.Reader;

// ReSharper disable InconsistentNaming
namespace Dotjosh.DayZCommander.App.Core
{
	public class GameUpdater
	{
		const string _dayZPage = "http://cdn.armafiles.info/latest/";
		public int? LatestArma2OABetaRevision { get; private set; }
		public string LatestArma2OABetaUrl { get; private set; }
		public Version LatestDayZVersion { get; private set; }

		public bool UpdateArma2OABeta()
		{
			if(string.IsNullOrEmpty(LocalMachineInfo.Arma2OABetaPath)
				|| string.IsNullOrEmpty(LatestArma2OABetaUrl))
			{
				return false;
			}
			var latestArma2OABetaFile = Path.GetFileName(LatestArma2OABetaUrl);
			if(string.IsNullOrEmpty(latestArma2OABetaFile))
			{
				return false;
			}
			var arma2OABetaFilePath = Path.Combine(LocalMachineInfo.Arma2OABetaPath, latestArma2OABetaFile);
			if(LatestArma2OABetaUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
			{
				var webClient = new WebClient();
				webClient.DownloadFile(LatestArma2OABetaUrl, arma2OABetaFilePath);
			}
			else // FTP.
			{
				// ftp://downloads.bistudio.com/arma2.com/update/beta/ARMA2_OA_Build_94444.zip
				var latestArma2OABetaUri = new Uri(LatestArma2OABetaUrl);
				var remoteDirectory = latestArma2OABetaUri.LocalPath.Replace(latestArma2OABetaFile, "");
				using(var ftp = new FtpConnection(latestArma2OABetaUri.Host))
				{
					ftp.Open();
					ftp.Login();
					ftp.SetCurrentDirectory(remoteDirectory);
					ftp.GetFile(latestArma2OABetaFile, arma2OABetaFilePath, false);
				}
			}
			using(var stream = File.OpenRead(arma2OABetaFilePath))
			{
				using(var reader = ReaderFactory.Open(stream))
				{
					while(reader.MoveToNextEntry())
					{
						if(reader.Entry.IsDirectory)
						{
							continue;
						}
						var fileName = Path.GetFileName(reader.Entry.FilePath);
						if(string.IsNullOrEmpty(fileName))
						{
							continue;
						}
						reader.WriteEntryToDirectory(LocalMachineInfo.Arma2OABetaPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
						if(fileName.EndsWith(".exe"))
						{
							var p = new Process
							        	{
							        		StartInfo =
							        			{
							        				CreateNoWindow = true,
							        				UseShellExecute = false,
							        				WindowStyle = ProcessWindowStyle.Hidden,
							        				WorkingDirectory = LocalMachineInfo.Arma2OABetaPath,
							        				FileName = Path.Combine(LocalMachineInfo.Arma2OABetaPath, fileName)
							        			}
							        	};
							p.Start();
						}
					}
				}
			}
			File.Delete(arma2OABetaFilePath);

			return true;
		}

		public bool UpdateDayZ()
		{
			if(string.IsNullOrEmpty(LocalMachineInfo.DayZPath))
			{
				return false;
			}
			var dayZFiles = GetDayZFiles();
			foreach(var dayZFile in dayZFiles)
			{
				var dayZAddonPath = Path.Combine(LocalMachineInfo.DayZPath, @"Addons").MakeSurePathExists();
				var dayZFileUrl = Path.Combine(_dayZPage, dayZFile);
				var dayZFilePath = Path.Combine(LocalMachineInfo.DayZPath, dayZFile);
				var webClient = new WebClient();
				webClient.DownloadFile(dayZFileUrl, dayZFilePath);
				if(dayZFile.EndsWithAny("zip", "rar"))
				{
					using(var stream = File.OpenRead(dayZFilePath))
					{
						using(var reader = ReaderFactory.Open(stream))
						{
							while(reader.MoveToNextEntry())
							{
								if(reader.Entry.IsDirectory)
								{
									continue;
								}
								reader.WriteEntryToDirectory(dayZAddonPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
							}
						}
					}
					File.Delete(dayZFilePath);
				}
			}
			return true;
		}



		private List<string> GetDayZFiles()
		{
			var files = new List<string>();
			string responseBody;
			if(!HttpGet(_dayZPage, out responseBody))
			{
				return files;
			}
			var fileMatches = Regex.Matches(responseBody, @"<a\s+href\s*=\s*(?:'|"")([^'""]+\.[^'""]{3})(?:'|"")", RegexOptions.IgnoreCase);
			foreach(Match match in fileMatches)
			{
				if(!match.Success)
				{
					continue;
				}
				var file = match.Groups[1].Value;
				if(string.IsNullOrEmpty(file))
				{
					continue;
				}

				files.Add(file);
			}

			return files;
		}

		public static bool HttpGet(string page, out string responseBody)
		{
			responseBody = null;
			var request = (HttpWebRequest)WebRequest.Create(page);
			request.Method = "GET";
			request.Timeout = 120000; // ms

			try
			{
				using(var response = request.GetResponse())
				{
					using(var responseStream = response.GetResponseStream())
					{
						if(responseStream == null)
						{
							return false;
						}
						var streamReader = new StreamReader(responseStream);
						responseBody = streamReader.ReadToEnd();
						streamReader.Close();
					}
				}
			}
			catch//(Exception e)
			{
				return false;
			}
			return true;
		}
	}
}