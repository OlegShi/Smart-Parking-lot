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
    public partial class Account : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["carnumber"] == null)
                Response.Redirect("Login");
            else
            {
                config.startUser();
                dbCarsFilter.Text = Session["carnumber"].ToString();
                if (dbCarsFilter.Text != "")
                {
                    updatePrice();
                }
            }
        }

        public string getBaseDateTimeNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString() + " " + localDate.Hour.ToString() + ":" + localDate.Minute.ToString() + ":" + localDate.Second.ToString();
        }

        private void updatePrice()
        {
            // Update parking price for car number
            string req = "select c.*, DATEDIFF(minute, c.enter_dt, '" + getBaseDateTimeNow() + "') as diff from cars c where c.number = '"+ dbCarsFilter.Text + "' and c.enter = '1'  and c.paid != '1' order by c.id desc";
            ArrayList results = config.Select(req);
            for (int i = 0; i < results.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)results[i];
                string id = row["id"];
                // Update the previous entry
                price p = new price(config.db, row["number"], row["diff"]);
                req = "update cars set payment_dt='" + getBaseDateTimeNow() + "', payment_mns='" + row["diff"] + "', payment_dhm='" + p.dayshoursmns + "', amount='" + p.amount + "' where id = '" + id + "'";
                config.noSelect(req);
            }
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

            // History
            if (dbCarsFilter.Text != "")
            {
                string req = "select cars.id as carid, cars.enter as Enter, cars.number as Number, enter_dt as 'Date_Enter', concat('Level ', p.level, ', ', p.row, ' ', p.number) as Gate_Enter, c.photoname as enter_picture, exit_dt as 'Date_Exit', concat('Level ', p2.level, ', ', p2.row, ' ', p2.number) as Gate_Exit, c2.photoname as exit_picture, payment_dt as Payment, payment_dhm as Duration, amount, paid, id_payment from cars LEFT JOIN camera_pictures c ON cars.id_enter_picture = c.id LEFT JOIN camera_pictures c2 ON cars.id_exit_picture = c2.id LEFT JOIN parking p ON cars.id_enter_gate = p.id LEFT JOIN parking p2 ON cars.id_exit_gate = p2.id where 1=1 and cars.number = '" + dbCarsFilter.Text + "' order by cars.enter_dt desc";
                ArrayList carRows = config.Select(req);

                tableParking += @"<table class=""table""><thead class=""thead-dark"">";
                tableParking += "<tr>";
                for (int i = 0; i < config.db.lastRowsNames.Count; i++)
                {
                    if (config.db.lastRowsNames[i] != "enter_sdata" && config.db.lastRowsNames[i] != "exit_sdata" && config.db.lastRowsNames[i] != "carid" && config.db.lastRowsNames[i] != "id_payment" && config.db.lastRowsNames[i] != "Enter")
                        tableParking += @"<th><span>" + config.db.lastRowsNames[i] + "</span></th>";
                }
                tableParking += "</tr></thead>";

                for (int i = 0; i < carRows.Count; i++)
                {
                    string id = "";
                    string paid = "";
                    string enter = "";

                    NameValueCollection row = (NameValueCollection)carRows[i];

                    // Label for the row
                    tableParking += "<tr>";
                    for (int j = 0; j < config.db.lastRowsNames.Count; j++)
                    {
                        if (config.db.lastRowsNames[j] == "carid")
                        {
                            id = row[config.db.lastRowsNames[j]];
                            continue;
                        }
                        if (config.db.lastRowsNames[j] == "Enter")
                        {
                            enter = row[config.db.lastRowsNames[j]];
                            continue;
                        }
                        if (config.db.lastRowsNames[j] == "paid")
                            paid = row[config.db.lastRowsNames[j]];

                        if (config.db.lastRowsNames[j] != "enter_sdata" && config.db.lastRowsNames[j] != "exit_sdata")
                        {
                            string value = row[config.db.lastRowsNames[j]];
                            if (row[config.db.lastRowsNames[j]] != "" && (config.db.lastRowsNames[j] == "enter_picture" || config.db.lastRowsNames[j] == "exit_picture" || config.db.lastRowsNames[j] == "photoname"))
                            {
                                value = @"<img width=""140"" src=""http://parkingcharlene.azurewebsites.net/images/" + row[config.db.lastRowsNames[j]] + @""" />";
                            }
                            if (config.db.lastRowsNames[j] == "id_payment" && (paid == "0" || paid == "") && enter == "1")
                                tableParking += @"<td><a class=""btn btn-primary"" href=""Payment?id=" + id + @""">" + "Payment" + "</a></td>";
                            else tableParking += @"<td><span>" + value + "</span></td>";
                        }
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