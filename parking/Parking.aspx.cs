using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;

namespace parking
{
    public partial class About : Page
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

        protected void butClearParking_Click(object sender, EventArgs e)
        {
            string req = "update parking set busy = '0'";
            config.noSelect(req);
            butParking_Click(null, null);
        }

        public void butParking_Click(object sender, EventArgs e)
        {
            // Put all levels
            string req = "select distinct level from parking order by level";
            ArrayList results = config.Select(req);
            ddParkingLevel.Items.Clear();
            for (int i = 0; i < results.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)results[i];
                ddParkingLevel.Items.Add(row["level"]);
            }
            ddParkingLevel.SelectedIndex = 0;
            string selectedLevel = ddParkingLevel.Items[ddParkingLevel.SelectedIndex].ToString();

            // Number of row to draw parking
            req = "select distinct row from parking where level='" + selectedLevel + "' order by row";
            ArrayList parkingRows = config.Select(req);
            int nbRow = parkingRows.Count;
            int nbMaxCols = 0;
            // Number of col for each row
            for (int i = 0; i < parkingRows.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)parkingRows[i];
                req = "select number from parking where level='" + selectedLevel + "' and row='" + row["row"] + "' order by number";
                ArrayList parkingRowNumbers = config.Select(req);
                if (parkingRowNumbers.Count > nbMaxCols) nbMaxCols = parkingRowNumbers.Count;
            }
            string tableParking = @"<table class=""table"">";
            // Create the table
            int ColumnCount = nbMaxCols + 1;
            int RowCount = 2 * nbRow;

            for (int i = 0; i < parkingRows.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)parkingRows[i];
                req = "select number, busy from parking where level='" + selectedLevel + "' and row='" + row["row"] + "' order by number";
                ArrayList parkingRowNumbers = config.Select(req);
                // Label for the row
                tableParking += "<tr>";
                tableParking += @"<td><span class=""parkline"">" + row["row"] + "</span></td>";

                for (int c = 0; c < parkingRowNumbers.Count; c++)
                {
                    NameValueCollection col = (NameValueCollection)parkingRowNumbers[c];
                    tableParking += @"<td><span class=""parkcol"">" + row["row"]+col["number"] + "</span></td>";
                }
                tableParking += "</tr>";
                tableParking += "<tr>";
                tableParking += @"<td></td>";

                for (int c = 0; c < parkingRowNumbers.Count; c++)
                {
                    NameValueCollection col = (NameValueCollection)parkingRowNumbers[c];
                    if (col["busy"] == "1")
                        tableParking += @"<td><img width=""40"" src=""images/car.jpg""/></td>";
                    else tableParking += @"<td>&nbsp;</td>";
                }
                tableParking += "</tr>";
            }
            tableParking += "<table>";
            labelParkingGrid.Text = tableParking;
            LabelUpd.Text = "Last Update : " + DateTime.Now.ToString();
            config.close();
        }
    }
}