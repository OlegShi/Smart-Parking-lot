using System.Collections;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob; // For Blob container
using Microsoft.WindowsAzure.Storage.File; // For File container
using Microsoft.WindowsAzure.Storage.Queue; // For Queue container
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;

namespace parking
{
    class SensorDevice
    {
        public string type = "sensor";
        public string ident;
        public string enter;
 
        public SensorDevice(string ident, string enter)
        {
            this.ident = ident;
            this.enter = enter;
        }

        public void insertSensorData(DBConnect db)
        {
            // Get the gate (id_parking) of the camera 
            string id_parking = "0";
            string req = "select id_parking from sensors where ident = '" + ident + "'";
            ArrayList results = db.Select(req);
            if (results.Count != 0)
            {
                NameValueCollection row = (NameValueCollection)results[0];
                id_parking = row["id_parking"];
            }
            req = "update parking set busy = '" + enter + "' where id = '" + id_parking + "'";
            db.noSelect(req);
        }

        /// Documentation References: 
        /// - Azure Storage client library for .NET - https://docs.microsoft.com/dotnet/api/overview/azure/storage?view=azure-dotnet
        /// - Asynchronous Programming with Async and Await - http://msdn.microsoft.com/library/hh191443.aspx
        /// 
        /* A Storage contains severals blob or file shares or queues containers
            a container has a name and contains a list of elements
        */

        public void readMessagesFromBlob(string connectionString, string blobcontainername)
        {
            try
            {
                listenBlob(blobcontainername, connectionString);
            }
            catch (Exception) { }
        }

        // Read Blob message from container 
        public async void listenBlob(string blobcontainername, string connectionString)
        {
            int maxloop = 1;
            for (int l = 0;  l < maxloop; l++)
            {
                CloudStorageAccount storageAccount = null;
                CloudBlobContainer cloudBlobContainer = null;

                // Check whether the connection string can be parsed.
                if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                    try
                    {
                        // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                        cloudBlobContainer = cloudBlobClient.GetContainerReference(blobcontainername);

                        // Set the permissions so the blobs are public. 
                        BlobContainerPermissions permissions = new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        };
                        await cloudBlobContainer.SetPermissionsAsync(permissions);

                        // List the blobs in the container.
                        BlobContinuationToken blobContinuationToken = null;
                        do
                        {
                            var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                            // Get the value of the continuation token returned by the listing call.
                            blobContinuationToken = results.ContinuationToken;
                            foreach (IListBlobItem item in results.Results)
                            {
                                try
                                {
                                    var textFromAzureBlob = (new WebClient()).DownloadString(item.Uri);
                                    // Match device TFMini event
                                    MatchCollection match = Regex.Matches(textFromAzureBlob, @"""type""\:""([^""]*)"",""ident""\:""([^""]*)"",""enter""\:""([^""]*)""}", RegexOptions.IgnoreCase);

                                    for (int i = 0; i < match.Count; i++)
                                    {
                                        string data = "{" + match[i].Value;
                                        SensorDevice sensor = JsonConvert.DeserializeObject<SensorDevice>(data);
                                        // Send Sensor Object to Form Side
                                        OnNetClientReceived(this, new NetClientReceivedEventArgs(sensor.type, sensor));
                                        // Wait a little to dont lock Form GUI
                                        await Task.Delay(1000);
                                    }
                                    // delete item file
                                    ((ICloudBlob)item).Delete();
                                }
                                catch (Exception ex) { string error = ex.Message; }
                            }
                        } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                    }
                    catch (StorageException)
                    {
                        // Error returned from the service: {0}", ex.Message
                    }
                    finally
                    {
                        // After reading delete content
                        if (cloudBlobContainer != null)
                        {
                            // Not good delete the container itself
                            //await cloudBlobContainer.DeleteIfExistsAsync();
                            // Delete all items
                            // Parallel.ForEach(cloudBlobContainer.ListBlobs(useFlatBlobListing: true), x => ((CloudBlob)x).Delete());
                        }
                    }
                }
                //await Task.Delay(1000);
            }
        }
        public delegate void Invoke_NetClient_DataReceived(object sender, NetClientReceivedEventArgs e);
        public delegate void NetClientReceived(object sender, NetClientReceivedEventArgs e);
        public event NetClientReceived OnNetClientReceived;
    }
}
