using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace parking
{
    public partial class Login : System.Web.UI.Page
    {
        // 7955059 for test
        InitSystem config = new InitSystem();

        protected void Page_Load(object sender, EventArgs e)
        {
            config.startUser();
            
        }

        private void updateSession(string number)
        {
            if (number == "")
            {
                labelLoginGrid.Text = "Invalid Login";
            }
            else
            {
                Session["carnumber"] = number;
                Response.Redirect("Default");
            }
        }

        protected void butLogin_Click(object sender, EventArgs e)
        {
            updateSession(dbCarsFilter.Text);
        }
    }
}