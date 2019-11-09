using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web.UI;

namespace parking
{
    public partial class Simulation : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();
        int oldSelect = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            config.start();
            oldSelect = dbSensorList.SelectedIndex;
            LabelUpd.Text = "Last Update : " + DateTime.Now.ToString();

            // Put All sensor name in list
            string req = "select * from sensors order by ident";
            ArrayList results = config.Select(req);
            // tosee

            dbSensorList.Items.Clear();
            for (int i = 0; i < results.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)results[i];
                dbSensorList.Items.Add(row["ident"]);
            }
            dbSensorList.SelectedIndex = 0;
         
        }

        protected void TimerReadBlob_Tick(object sender, EventArgs e)
        {
            LabelUpd.Text = "Last Update : " + DateTime.Now.ToString();
            config.close();
        }
        // Get Current DateTime 
        private string getDateTimeNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.ToString(new CultureInfo("fr-FR"));
        }

        protected void butSendSensor_Click(object sender, EventArgs e)
        {
            dbSensorList.SelectedIndex = oldSelect;
            string selectedSensor = dbSensorList.Items[dbSensorList.SelectedIndex].ToString();
            string enter = (cbCarParked.Checked) ? "1" : "0";
            labelSimuation.Text += "<br />" + getDateTimeNow() + " : Sensor, " + selectedSensor + " " + ((cbCarParked.Checked) ? "1" : "0");

            config.writeSensorBlob(selectedSensor, enter);
 
            // Insert Data in database
            //config.insertSensorData(selectedSensor, (cbCarParked.Checked) ? "1" : "0");
            // Not working in ASP.NET but ok in .NET
            //labelSimuation.Text += "<br />" + config.SendSensorDeviceToCloudMessagesAsync(selectedSensor, (cbCarParked.Checked) ? "1" : "0");
        }

        protected void UploadFile(object sender, EventArgs e)
        {
            string folderPath = Server.MapPath("~/Files/");

            if (Directory.Exists(folderPath) && FileUploadImg.HasFile && FileUploadImg.FileName != "")
            {
                //Save the File to the Directory (Folder).
                string filename = folderPath + Path.GetFileName(FileUploadImg.FileName);
                byte[] filearray = FileUploadImg.FileBytes;
                FileUploadImg.SaveAs(filename);

                //Display the Picture in Image control.
                Image2.ImageUrl = "~/Files/" + Path.GetFileName(FileUploadImg.FileName);
                // Send to blob
                // For test we consider we have CAMERA1...CAMERA2
                string req = "select enter from camera_pictures order by id desc";
                ArrayList results = config.Select(req);
                string enter = "1";
                for (int l = 0; l < 1; l++)
                {
                    NameValueCollection row = (NameValueCollection)results[l];
                    enter = row["enter"];
                }
                string camera = (enter == "1")? "CAMERA1": "CAMERA2";
                config.writeCameraBlob(camera, enter, filearray);
                labelSimuation.Text += "<br />" + getDateTimeNow() + " : "+ camera + " " + enter;
            }
        }
    }
}