using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace parking
{
    public partial class _Default : Page
    {
        InitSystem config = new InitSystem();
   
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["carnumber"] == null)
                Response.Redirect("Login");
            else config.startUser();
        }

        protected void butParking_Click(object sender, EventArgs e)
        {
            string tableParking = "";

            // Number of row to draw parking
            string req = "select level, row,count(*) as sums from parking where busy='0' and row != 'GATE' group by level,row order by level, row";
            ArrayList parkingRows = config.Select(req);

            tableParking += @"<table class=""table""><thead class=""thead-dark"">";
            tableParking += "<tr>";
            tableParking += @"<th><span>Level</span></th>";
            tableParking += @"<th><span>Row</span></th>";
            tableParking += @"<th><span>Free</span></th>";
            tableParking += "</tr></thead>";

            for (int i = 0; i < parkingRows.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)parkingRows[i];
                // Label for the row
                tableParking += "<tr>";
                tableParking += @"<td><span class=""parkline"">" + row["level"] + "</span></td>";
                tableParking += @"<td><span class=""parkline"">" + row["row"] + "</span></td>";
                tableParking += @"<td><span>" + row["sums"] + "</span></td>";
                tableParking += "</tr>";
            }
            tableParking += "<table>";

            labelParkingGrid.Text = tableParking;
            config.close();
        }
    }
}