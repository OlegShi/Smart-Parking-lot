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
    public partial class Payment : System.Web.UI.Page
    {
        InitSystem config = new InitSystem();
        string id = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["carnumber"] == null)
                Response.Redirect("Login");
            else
            {
                id = Request.QueryString["id"];
                config.startUser();
                updatePrice();
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
            string req = "select c.*, DATEDIFF(minute, c.enter_dt, '" + getBaseDateTimeNow() + "') as diff from cars c where c.id = '" + id + "' and c.enter = '1' and c.paid != '1' order by c.id desc";
            ArrayList results = config.Select(req);
            string topay = "";
            for (int i = 0; i < results.Count; i++)
            {
                NameValueCollection row = (NameValueCollection)results[i];
             
                // Update the previous entry
                price p = new price(config.db, row["number"], row["diff"]);
                req = "update cars set payment_dt='" + getBaseDateTimeNow() + "', payment_mns='" + row["diff"] + "', payment_dhm='" + p.dayshoursmns + "', amount='" + p.amount + "' where id = '" + id + "'";
                config.noSelect(req);
                topay = p.amount.ToString();
            }
            if (results.Count == 0 || topay == "")
            {
                labelPaymentGrid.Text = "Error Invalid Reference";
                butPay.Visible = false;
            }
            else
            {
                labelPaymentGrid.Text = "TOTAL to PAY : " + topay +" sh";
            }
        }

        protected void butPay_Click(object sender, EventArgs e)
        {
            String s = Request.QueryString["id"];
            string req = "update cars set paid = '1' where id = '" + s + "'";
            config.noSelect(req);
            Response.Redirect("Account");
        }

        protected void butCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Account");
        }
    }
}