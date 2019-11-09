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
    public partial class User : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();
        string selectedLevel = "0";
        protected void Page_Load(object sender, EventArgs e)
        {
            config.startUser();
        }

        private byte[] Hex2Bin(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
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

            // History
            if (dbCarsFilter.Text != "")
            {
                req = "select cars.number as Number, enter_dt as 'Date_Enter', concat('Level ', p.level, ', ', p.row, ' ', p.number) as Gate_Enter, c.sdata as enter_sdata, c.cdata as enter_picture, exit_dt as 'Date_Exit', concat('Level ', p2.level, ', ', p2.row, ' ', p2.number) as Gate_Exit, c2.sdata as exit_sdata, c2.cdata as exit_picture, payment_dt as Payment, payment_dhm as Duration, amount, paid from cars LEFT JOIN camera_pictures c ON cars.id_enter_picture = c.id LEFT JOIN camera_pictures c2 ON cars.id_exit_picture = c2.id LEFT JOIN parking p ON cars.id_enter_gate = p.id LEFT JOIN parking p2 ON cars.id_exit_gate = p2.id where 1=1 and cars.number = '"+ dbCarsFilter.Text + "' order by cars.dt desc";
                ArrayList carRows = config.Select(req);

                tableParking += @"<table class=""table""><thead class=""thead-dark"">";
                tableParking += "<tr>";
                for (int i = 0; i < config.db.lastRowsNames.Count; i++)
                {
                    if (config.db.lastRowsNames[i] != "enter_sdata" && config.db.lastRowsNames[i] != "exit_sdata")
                        tableParking += @"<th><span>"+ config.db.lastRowsNames[i] + "</span></th>";
                }
                tableParking += "</tr></thead>";

                for (int i = 0; i < carRows.Count; i++)
                {
                    NameValueCollection row = (NameValueCollection)carRows[i];
                    // Label for the row
                    tableParking += "<tr>";
                    for (int j = 0; j < config.db.lastRowsNames.Count; j++)
                        if (config.db.lastRowsNames[j] != "enter_sdata" && config.db.lastRowsNames[j] != "exit_sdata")
                        {
                            string value = row[config.db.lastRowsNames[j]];
                            if (row[config.db.lastRowsNames[j]] != "" && (config.db.lastRowsNames[j] == "enter_picture" || config.db.lastRowsNames[j] == "exit_picture" || config.db.lastRowsNames[j] == "cdata"))
                            {
                                byte[] img = Hex2Bin(row[config.db.lastRowsNames[j]]);
                                value = @"<img width=""140"" src=""data:image/jpeg;base64," + Convert.ToBase64String(img) + @"""/>";
                            }
                            tableParking += @"<td><span>" + value + "</span></td>";
                        }
                    tableParking += "</tr>";
                }
                tableParking += "<table>";
            }

            labelParkingGrid.Text = tableParking;
            config.close();
        }
    }
}