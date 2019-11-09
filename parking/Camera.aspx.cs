using System;
using System.Web.UI;

namespace parking
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();
        protected void Page_Load(object sender, EventArgs e)
        {
            config.start();
        }

        protected void TimerReadBlob_Tick(object sender, EventArgs e)
        {
            LabelUpd.Text = "Last Update : " + DateTime.Now.ToString();
            config.close();
        }

        // Display Camera image for the current day etc ...
        protected void menuAllCars_Click(object sender, EventArgs e)
        {
            // Filter Car Number
            string cameraFilter = dbCarsFilter.Text.Trim();
            string condCleanOcr = (cameraFilter == "") ? "" : " and cleanOcr like '%" + cameraFilter + "%'";
            // Filter with selected date
            string condDate = "";
            if (calDateText.Text != "")
            {
                string seldate = calDateText.Text;
                string[] sDate = seldate.Split('/');
                if (sDate.Length >= 3)
                {
                    string selectedDate = sDate[2] + "-" + sDate[1] + "-" + sDate[0];
                    condDate = " and (dt >= '" + selectedDate + " 00:00' and dt <= '" + selectedDate + " 23:59:59')";
                }
            }
            // Build request
            string req = "select * from camera_pictures where 1=1 " + condDate + condCleanOcr + " order by dt desc";
            // Process the request
            config.Select(req, labelCarGrid);
        }
    }
}