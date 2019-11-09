using System;
using System.Web.UI;

namespace parking
{
    public partial class _Default : Page
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

        protected void menuAllCars_Click(object sender, EventArgs e)
        {
            // Filter Car Number
            string carFilter = dbCarsFilter.Text.Trim();
            string condCleanOcr = (carFilter == "") ? "" : " and cars.number like '%" + carFilter + "%'";
            // Filter with selected date
            string condDate = "";
            if (calDateText.Text != "")
            {
                string seldate = calDateText.Text;
                string[] sDate = seldate.Split('/');
                if (sDate.Length >= 3)
                { 
                    string selectedDate = sDate[2] + "-" + sDate[1] + "-" + sDate[0];
                    condDate = " and ((enter_dt >= '" + selectedDate + " 00:00' and enter_dt <= '" + selectedDate + " 23:59:59') or (exit_dt >= '" + selectedDate + " 00:00' and exit_dt <= '" + selectedDate + " 23:59:59'))";
                }
            }
            // Build request
           string req = "select cars.number as Number, cars.enter as Enter, enter_dt as 'Date_Enter', concat('Level ', p.level, ', ', p.row, ' ', p.number) as Gate_Enter, c.photoname as enter_picture, exit_dt as 'Date_Exit', concat('Level ', p2.level, ', ', p2.row, ' ', p2.number) as Gate_Exit, c2.photoname as exit_picture, payment_dt as Payment, payment_dhm as Duration, amount, paid, total_dhm from cars LEFT JOIN camera_pictures c ON cars.id_enter_picture = c.id LEFT JOIN camera_pictures c2 ON cars.id_exit_picture = c2.id LEFT JOIN parking p ON cars.id_enter_gate = p.id LEFT JOIN parking p2 ON cars.id_exit_gate = p2.id where 1=1 " + condDate + condCleanOcr + " order by cars.enter_dt desc";

            // Process the request
            config.Select(req, labelCarGrid);
        }
    }
}