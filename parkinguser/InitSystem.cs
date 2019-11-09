using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace parking
{
    public class InitSystem
    {
        public DBConnect db = new DBConnect("charlene.database.windows.net", "parking", "dbcharlenetlv", "ch705896Xy#");
        //TableLayoutPanel tableLayoutPanel = null;
        // To Get Messages from Storage container 
        SensorDevice[] storageSensorDevice;
        CameraDevice[] storageCameraDevice;
        // For tests and simulations only
        AzureDeviceClient SensorDeviceClient;
        AzureDeviceClient CameraDeviceClient;
        string OcrKey = "";

        public string lastError = "";

        public InitSystem()
        {

        }

        public void start()
        {
            db.OpenConnection();
            requestAndLoadConfig();
        }

        public void startUser()
        {
            db.OpenConnection();
        }

        public void close()
        {
            db.CloseConnection();
        }

        public ArrayList Select(string req)
        {
            ArrayList res = db.Select(req);
            lastError = db.lastError;
            return res;
        }

        public int noSelect(string req)
        {
            int res = db.noSelect(req);
            lastError = db.lastError;
            return res;
        }

        public void Select(string req, Label dgCarsUserResults)
        {
            db.Select(req, dgCarsUserResults);
        }

        public void insertSensorData(string ident, string enter)
        {
            SensorDevice sensor = new SensorDevice(ident, enter);
            sensor.insertSensorData(db);
        }

        public void writeCameraBlob(string cameraName, string enter, byte[] data)
        {
            CameraDeviceClient.writeCameraBlob(cameraName, enter, data);
        }

        public void writeSensorBlob(string sensorName, string enter)
        {
            SensorDeviceClient.writeSensorBlob(sensorName, enter);
        }

        public string SendSensorDeviceToCloudMessagesAsync(string ident, string enter)
        {
            //SensorDeviceClient.SendSensorDeviceToCloudMessagesAsync(ident, enter);
            string res = Task.Run(() => SensorDeviceClient.SendSensorDeviceToCloudMessagesAsync(ident, enter)).Result;
            return res;
        }

        public string getBaseDateTimeNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString() + " " + localDate.Hour.ToString() + ":" + localDate.Minute.ToString() + ":" + localDate.Second.ToString();
        }

        public void requestAndLoadConfig()
        {
            // Request parameters devices to get messages from storage if route used in azure 
            // Sensors
            string req = "select * from azure where type = 'STORAGE' and ident = 'SENSOR'";
            ArrayList results = db.Select(req);
            storageSensorDevice = new SensorDevice[results.Count];
            for (int l = 0; l < results.Count; l++)
            {
                System.Collections.Specialized.NameValueCollection row = (NameValueCollection)results[l];
                // To Get Message from Storage container (for Sensors)
                storageSensorDevice[l] = new SensorDevice("", "");
                // Readed messages sent using OnNetClientReceived to form
                storageSensorDevice[l].OnNetClientReceived += new SensorDevice.NetClientReceived(netClient_DataReceived);
                // Read message from Storage Account, buth the specific Blob container : TFMinis devicetfminicontainer
                storageSensorDevice[l].readMessagesFromBlob(row["ConnectionString"], row["EventsEndpoint"]);
                if (l == 0)
                    SensorDeviceClient = new AzureDeviceClient(row["ConnectionString"], row["EventsEndpoint"], OcrKey);
            }

            // OCR Computer Vision Key
            req = "select * from azure where type = 'VISION' and ident = 'OCR'";
            results = db.Select(req);
            for (int l = 0; l < 1; l++)
            {
                NameValueCollection row = (NameValueCollection)results[l];
                OcrKey = row["PrimaryKey"];
            }

            // Cameras
            req = "select * from azure where type = 'STORAGE' and ident = 'CAMERA'";
            results = db.Select(req);
            storageCameraDevice = new CameraDevice[results.Count];
            for (int l = 0; l < results.Count; l++)
            {
                NameValueCollection row = (NameValueCollection)results[l];
                // To Get Message from Storage container (for Sensors)
                storageCameraDevice[l] = new CameraDevice("", "");
                // Readed messages sent using OnNetClientReceived to form
                storageCameraDevice[l].OnNetClientReceived += new CameraDevice.NetClientReceived(netClient_DataReceived);
                // Read message from Storage Account, buth the specific Blob container : TFMinis devicecameracontainer
                storageCameraDevice[l].readMessagesFromBlob(row["ConnectionString"], row["EventsEndpoint"], OcrKey);
                // Create a Test camera device for c# program
                if (l == 0)
                    CameraDeviceClient = new AzureDeviceClient(row["ConnectionString"], row["EventsEndpoint"], OcrKey);
            }

            // Update parking price for all cars inside
            req = "select c.*, DATEDIFF(minute, c.enter_dt, '" + getBaseDateTimeNow() + "') as diff from cars c where c.enter = '1' and c.paid != '1' order by c.id desc";
            results = db.Select(req);
            for (int i=0; i < results.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)results[i];
                string id = row["id"];
                // Update the previous entry
                price p = new price(db, row["number"], row["diff"]);
                req = "update cars set payment_dt='"+ getBaseDateTimeNow() + "', payment_mns='" + row["diff"] + "', payment_dhm='" + p.dayshoursmns + "', amount='"+p.amount+"' where id = '" + id + "'";
                db.noSelect(req);
            }

        }

        // Recept a message from Azure in event mode in form side
        private void netClient_DataReceived(object sender, NetClientReceivedEventArgs e)
        {
            my_NetClient_DataReceived(sender, e);
        }
        // Manage message received message from Azure in Form side
        private void my_NetClient_DataReceived(object sender, NetClientReceivedEventArgs e)
        {
            switch (e.datatype)
            {
                case "sensor":
                    SensorDevice sensor = (SensorDevice)e.data;
                    // Insert Data in database
                    sensor.insertSensorData(db);
                    break;
                case "camera":
                    CameraDevice camera = (CameraDevice)e.data;
                    // Display Image received in form (test)
                    ImageProcessing image = new ImageProcessing();
                    // tosee
                    //pictureBoxDB.Image = image.convertToImage(camera.photo);
                    if (camera.cleanResult == "")
                    {
                        // Request to Azure the OCR and process json result
                        image.getOcrFromAzure(camera.photo, OcrKey);
                        // Display trace log in textbox
                        //rtbLastErrorTest.Text += "\n" + getDateTimeNow() + " : Received Camera, Hard Result : " + image.hardResult + "\n\nClean Result : " + image.cleanResult;
                        // Insert Image and Ocr text in database
                        camera.insertCameraData(db, image.data, image.hardResult, image.cleanResult);
                    }
                    else
                    {
                        // Display trace log in textbox
                        //rtbLastErrorTest.Text += "\n" + getDateTimeNow() + " : Received Camera, Hard Result : " + camera.hardResult + "\n\nClean Result : " + camera.cleanResult;
                        // Insert Image and Ocr text in database
                        camera.insertCameraData(db, camera.photo, camera.hardResult, camera.cleanResult);
                    }
                    
                    break;
            }
        }
    }

    public class NetClientReceivedEventArgs : EventArgs
    {
        public string datatype;
        public object data;

        public NetClientReceivedEventArgs(string datatype, object data)
            : base()
        {
            this.datatype = datatype;
            this.data = data;
        }
    }
}