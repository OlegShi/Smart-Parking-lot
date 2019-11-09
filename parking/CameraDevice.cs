using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace parking
{
    class CameraDevice
    {
        public string type = "camera";
        public string ident;
        public string enter;
        public string hardResult = "";
        public string cleanResult = "";
        public byte[] photo;

        public CameraDevice(string ident, string enter)
        {
            this.ident = ident;
            this.enter = enter;
        }

        public CameraDevice(string ident, string enter, byte[] photo)
        {
            this.ident = ident;
            this.enter = enter;
            ImageProcessing img = new ImageProcessing();
            this.photo = photo;
        }

        // Get Current DateTime in SQL Format
        public string getBaseDateTimeNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString() + " " + localDate.Hour.ToString() + ":" + localDate.Minute.ToString() + ":" + localDate.Second.ToString();
        }

        public void insertCameraData(DBConnect db, byte[] data, string hardOcrResult, string cleanOcrResult)
        {
            int maxsecdonothing = 20;
            string id_gate = "0";
            // Following the camera, we know if enter = 0 or 1
            string req = "select enter from cameras where ident='"+ ident + "'";
            ArrayList results = db.Select(req);
            for (int l = 0; l < 1; l++)
            {
                NameValueCollection row = (NameValueCollection)results[l];
                this.enter = row["enter"];
            }
            if (hardOcrResult != "")
            {
                req = "insert into camera_pictures (hardOcr,cleanOcr,ident,enter,photoname, dt) OUTPUT INSERTED.id VALUES ('" + hardOcrResult + "','" + cleanOcrResult + "','" + ident + "','" + enter + "','', '" + getBaseDateTimeNow() + "');";
                int newid = db.noSelect(req, "@cdata", data);
                req = "update camera_pictures set photoname='camerapicture-"+ newid + ".jpg' where id=" + newid;
                db.noSelect(req);

                ImageProcessing image = new ImageProcessing();
                string path = HttpContext.Current.Server.MapPath("~");
                image.saveToFile(data, path, @"images\camerapicture-" + newid + ".jpg");

                // Get the gate (id_parking) of the camera 
                req = "select id_parking from cameras where ident = '" + ident + "'";
                results = db.Select(req);
                if (results.Count != 0)
                {
                    NameValueCollection row = (NameValueCollection)results[0];
                    id_gate = row["id_parking"];
                }
                // we check if the car is enter with out exit, so consider it's exit (for test 1 camera)
                if (enter == "1") // the car is entering in the parking
                {
                    // to prevent multiple update, we check date time
                    req = "select c.*, DATEDIFF(second, c.enter_dt, '"+ getBaseDateTimeNow() + "') as diff from cars c where c.enter = '1' and c.number = '" + cleanOcrResult + "' order by c.id desc";
                    results = db.Select(req);
                    string previd = "0";
                    if (results.Count != 0)
                    {
                        NameValueCollection row = (NameValueCollection)results[0];
                        // if < 20s we do nothing
                        if (Int32.Parse(row["diff"]) < maxsecdonothing) return;
                        previd = row["id"];
                        enter = "0";
                    }
                   
                }

                if (enter == "1") // the car is entering in the parking
                {
                    // Create an entry in the cars table
                    req = "insert into cars (number, enter, enter_dt, id_enter_gate, id_enter_picture, dt, paid) OUTPUT INSERTED.id VALUES ('" + cleanOcrResult + "','" + enter + "','" + getBaseDateTimeNow() + "','" + id_gate + "','" + newid.ToString() + "','" + getBaseDateTimeNow() + "', '0');";
                    newid = db.noSelect(req);
                }
                else // The car is exiting the parking
                {
                    req = "select c.*, DATEDIFF(minute, c.enter_dt, '" + getBaseDateTimeNow() + "') as diff from cars c where c.number = '" + cleanOcrResult + "' order by c.id desc";
                    results = db.Select(req);
                    if (results.Count != 0)
                    {
                        NameValueCollection row = (NameValueCollection)results[0];
                        string id = row["id"];
                        // Update the previous entry
                        price p = new price(db, ident, row["diff"]);
                        req = "update cars set enter='" + enter + "', exit_dt='" + getBaseDateTimeNow() + "', id_exit_gate='" + id_gate + "', id_exit_picture='" + newid.ToString() + "', dt='" + getBaseDateTimeNow() + "', total_mns='" + row["diff"] + "', total_dhm='" + p.dayshoursmns + "' where id = '" + id + "'";
                        newid = db.noSelect(req);
                    }
                }
            }
        }

        public void readMessagesFromBlob(string connectionString, string blobcontainername, string ocrKey)
        {
            try
            {
                listenBlob(blobcontainername, connectionString, ocrKey);
            }
            catch (Exception) { }
        }

        // Read Blob message from container 
        public async void listenBlob(string blobcontainername, string connectionString, string ocrKey)
        {
            int maxloop = 1;
            for (int l = 0; l < maxloop; l++)
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
                                    string uri = item.Uri.ToString();
                                    // Parse the file name 
                                    // "https://charlenestorage.blob.core.windows.net/devicecameracontainer/CAMERA2-1-2018-8-12-10-32-41.jpg"
                                    string[] suri = uri.Split('/');
                                    string filename = suri[suri.Length - 1];
                                    suri = filename.Split('-');
                                    string cameraname = suri[0];
                                    string enter = suri[1];
                                    suri = filename.Split('.');
                                    string filetype = suri[suri.Length - 1];

                                    // get data byte from file in container
                                    byte[] data = (new WebClient()).DownloadData(item.Uri);
                                    // Request the OCR to azure computer vision
                                    ImageProcessing image = new ImageProcessing();
                                    image.getOcrFromAzure(data, ocrKey);

                                    // Build event for form and storage in database
                                    CameraDevice camera = new CameraDevice(cameraname, enter, image.data);
                                    // Properties contain specific properties of the message, set by device
                                    camera.hardResult = image.hardResult;
                                    camera.cleanResult = image.cleanResult;
                                    // Send Sensor Object to Form Side
                                    OnNetClientReceived(this, new NetClientReceivedEventArgs("camera", camera));
                                    // Wait a little to dont lock Form GUI
                                    await Task.Delay(1000);
                                    // delete item
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
