using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace parking
{
    class AzureDeviceClient
    {
        private ServiceClient serviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private string s_connectionString = "";
        // For Camera Container
        private string containerName = "";
        private string ocrKey = "";

        public AzureDeviceClient(string connectionString)
        {
            s_connectionString = connectionString;
            serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString);
        }

        public AzureDeviceClient(string connectionString, string containerName, string ocrKey)
        {
            s_connectionString = connectionString;
            this.containerName = containerName;
            this.ocrKey = ocrKey;
        }

        public string getAzureDeviceId(string connectionString)
        {
            MatchCollection match = Regex.Matches(connectionString, @"DeviceId=([^;]*)", RegexOptions.IgnoreCase);
            string result = "";
            for (int i = 0; i < match.Count; i++)
            {
                result = match[i].Value.Replace("DeviceId=","");
            }
            return result;
        }
            // Async method to send simulated telemetry
            // ident : identification / number of the sensor to identify the place of teh car
            // enter : 1 the car is entered, 0 the car exited
        public async Task<string> SendSensorDeviceToCloudMessagesAsync(string ident, string enter)
        {
            // Create JSON message body with SensorDeviceObject
            SensorDevice sensor = new SensorDevice(ident, enter);
            string res = "";
            var messageString = JsonConvert.SerializeObject(sensor);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            //message.Properties.Add("sensorXXXX", zzzz);

            // Send the sensor message
            try
            {
                string deviceid = getAzureDeviceId(s_connectionString);
                await serviceClient.SendAsync(deviceid, message);
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        private string getBlobNameNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString() + "-" + localDate.Hour.ToString() + "-" + localDate.Minute.ToString() + "-" + localDate.Second.ToString();
        }

        // Write an json to the blob sensor container (for test purpose)
        public async void writeSensorBlob(string sensorName, string enter)
        {
            string sensorjson = " {\"type\":\"sensor\",\"ident\":\""+ sensorName + "\",\"enter\":\""+ enter + "\"} ";
            byte[] bytes = Encoding.ASCII.GetBytes(sensorjson);
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(s_connectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // CharleneIoTHub-0-2018-07-12-08-03
                    string blobName = sensorName + "-" + enter + "-" + getBlobNameNow() + ".txt";

                    CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                    await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
                }
                catch (StorageException ex)
                {
                    // Error returned from the service: {0}", ex.Message
                    string error = ex.Message;
                }
            }
        }

            // Write an image to the blob camera container (for test purpose)
        public async void writeCameraBlob(string cameraName, string enter, byte[] image)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(s_connectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
 
                    // CharleneIoTHub-0-2018-07-12-08-03
                    string blobName = cameraName + "-" + enter + "-" + getBlobNameNow() + ".jpg";

                    CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                    await blob.UploadFromByteArrayAsync(image, 0, image.Length);
                }
                catch (StorageException ex)
                {
                    // Error returned from the service: {0}", ex.Message
                    string error = ex.Message;
                }
            }
        }
    }
}
    