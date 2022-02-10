using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignment
{
    public partial class Authentication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_submit_Click(object sender, EventArgs e) 
        {
            string code = HttpUtility.HtmlEncode(tb_authentication.Text.Trim());
        }
    }
}