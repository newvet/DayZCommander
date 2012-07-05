using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dotjosh.DayZCommander.Deploy.Properties;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Dotjosh.DayZCommander.Deploy
{
	class Program
	{
		static void Main(string[] args)
		{
			// Retrieve storage account from connection-string
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
				Convert.ToString(Settings.Default.Properties["StorageConnectionString"].DefaultValue));

			// Create the blob client
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container
			CloudBlobContainer container = blobClient.GetContainerReference("releases");

			
			var localDeployDirectory = Convert.ToString(Settings.Default.Properties["LocalDeployDirectory"].DefaultValue);
			var localDeployDirectoryInfo = new DirectoryInfo(localDeployDirectory);

			var files = localDeployDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files.Reverse())
            {
            	var partialFileName = file.FullName.Replace(localDeployDirectoryInfo.FullName + "\\", "");
                var blob = container.GetBlobReference(partialFileName);

                try
                {
                    blob.FetchAttributes();
                }
                catch (StorageClientException) { }

                var lastModified = DateTime.MinValue;

                if (!String.IsNullOrWhiteSpace(blob.Metadata["LastModified"]))
                {
                    long timeTicks = long.Parse(blob.Metadata["LastModified"]);
                    lastModified = new DateTime(timeTicks, DateTimeKind.Utc);
                }

                if (lastModified != file.LastWriteTimeUtc)
                {
					Console.WriteLine("Uploading File: " + partialFileName);
                    blob.UploadFile(file.FullName);

                    blob.Metadata["LastModified"] = file.LastWriteTimeUtc.Ticks.ToString();
                    blob.SetMetadata();
                    blob.SetProperties();
                }
				else
                {
                	Console.WriteLine("Skipping File: " + partialFileName);
                }
            }
			Console.WriteLine("Deployment complete.");
			Console.ReadLine();
		}
	}
}
