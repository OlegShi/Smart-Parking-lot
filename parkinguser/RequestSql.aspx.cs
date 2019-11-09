using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace parking
{
    public partial class RequestSql : System.Web.UI.Page
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

        protected void butSelect_Click(object sender, EventArgs e)
        {
            string req = rtbRequest.Text;
            string selectedreq = req;
            if (selectedreq.Trim() == "") selectedreq = req;
            if (selectedreq.Trim().ToLower().IndexOf("select") == 0)
            {
                config.Select(selectedreq, labelParkingGrid);
                //rtbLastError.Text = config.lastError;
            }
            else
            {
                config.noSelect(selectedreq);
                labelParkingGrid.Text = config.lastError;
            }
        }
    }
}