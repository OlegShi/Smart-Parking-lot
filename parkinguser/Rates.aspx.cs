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
    public partial class Rates : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["carnumber"] == null)
                Response.Redirect("Login");
            else
            {
                config.startUser();
                loadRates();
            }
        }

        public string getBaseDateNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString();
        }

        protected void loadRates()
        {
            string numbercar = Session["carnumber"].ToString();
            string datenow = getBaseDateNow();
            string tableRates = "";

            // Number of row to draw parking
            string req="";

            tableRates += @"<table class=""table""><thead class=""thead-dark"">";
            tableRates += "<tr>";
            tableRates += @"<th><span>Number</span></th>";
            tableRates += @"<th><span>From</span></th>";
            tableRates += @"<th><span>To</span></th>";
            tableRates += @"<th><span>Rate</span></th>";
            tableRates += @"<th><span>For Each</span></th>";
            tableRates += @"<th><span>Duration</span></th>";
            tableRates += @"<th><span>Price</span></th>";
            tableRates += "</tr></thead>";
            int matched = 0;
            for (int r = 0; r < 4; r++)
            {
                switch (r)
                {
                    case 0: // We check if there is a specific subscription for car number
                        req = "select s.*, r.ratename, r.unit, d.limitmns, d.price from subscription s, rates r, rate_details d where s.id_rate = r.id and s.id_rate = d.id_rate and s.ident = '" + numbercar + "' and s.from_dt <= '" + datenow + "' and s.to_dt >= '" + datenow + "' order by s.id desc, d.limitmns asc";
                        break;
                    case 1: // We check if there is a specific subscription for car number
                        req = "select s.*, r.ratename, r.unit, d.limitmns, d.price from subscription s, rates r, rate_details d where s.id_rate = r.id and s.id_rate = d.id_rate and s.ident = '" + numbercar + "' and s.from_dt <= '" + datenow + "' order by s.id desc, d.limitmns asc";
                        break;
                    case 2:
                        req = "select s.*, r.ratename, r.unit, d.limitmns, d.price from subscription s, rates r, rate_details d where s.id_rate = r.id and s.id_rate = d.id_rate and s.ident = '' and s.from_dt <= '" + datenow + "'  and s.to_dt >= '" + datenow + "' order by s.id desc, d.limitmns asc";
                        break;
                    case 3:
                        req = "select s.*, r.ratename, r.unit, d.limitmns, d.price from subscription s, rates r, rate_details d where s.id_rate = r.id and s.id_rate = d.id_rate and s.ident = '' and s.from_dt <= '" + datenow + "' order by s.id desc, d.limitmns asc";
                        break;
                }
                ArrayList subscription = config.Select(req);
                if (subscription.Count != 0)
                {
                    if (r == 0 || r == 2) r++;
                    matched++;
                }
                for (int i = 0; i < subscription.Count; i++)
                {
                    NameValueCollection row = (NameValueCollection)subscription[i];
                    string id_rate = row["id_rate"];
                    string classname = (matched == 1) ? @"class=""parkline""":"";
                    if (row["to_dt"] == "") row["from_dt"] = "";
                    // Label for the row
                    tableRates += "<tr>";
                    tableRates += @"<td><span " + classname + ">" + row["ident"] + "</span></td>";
                    tableRates += @"<td><span " + classname + ">" + row["from_dt"].Replace(" 00:00:00","") + " </span></td>";
                    tableRates += @"<td><span " + classname + ">" + row["to_dt"].Replace(" 00:00:00", "") + "</span></td>";
                    tableRates += @"<td><span>" + row["ratename"] + "</span></td>";
                    tableRates += @"<td><span>" + row["unit"] + " mns</span></td>";
                    tableRates += @"<td><span>0-" + row["limitmns"] + " mns</span></td>";
                    tableRates += @"<td><span>" + row["price"] + " sh</span></td>";
                    tableRates += "</tr>";
                }
            }

            tableRates += "<table>";

            labelParkingGrid.Text = tableRates;
            config.close();
        }
    }
}