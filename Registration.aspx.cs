using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Security.Cryptography; 
using System.Text; 
using System.Data; 
using System.Data.SqlClient;

namespace PracticalAssignment
{
    //Site key:
    //6Lf6dmIeAAAAAHWMO1YdpTtP5wJFs8XR0WMoKNne
    //Secret key:
    //6Lf6dmIeAAAAAO1_xdMi3SseET_3_47vUzDCIr-V
    public partial class WebForm : System.Web.UI.Page
    {
        string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        static string creditCardHash;
        static string finalPasswordHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int passwordStrength(string password) {
            int score = 0;
            if (password.Length >= 12) {
                if (Regex.IsMatch(password, "[a-z]")) {
                    score += 1;
                }
                if (Regex.IsMatch(password, "[A-Z]"))
                {
                    score += 1;
                }
                if (Regex.IsMatch(password, "[0-9]"))
                {
                    score += 1;
                }
                if (Regex.IsMatch(password, "[^a-zA-Z0-9]") && score==3)
                {
                    score += 1;
                }
            }
            return score;
        }

        private Boolean checkName(string username) 
        {
            Boolean b = false;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select Username FROM Users", connection);
                using (command)
                {
                    connection.Open();
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Username"] != null)
                                {
                                    if (reader["Username"] != DBNull.Value)
                                    {
                                        string name = reader["Username"].ToString().Trim();
                                        if (name == username) 
                                        {
                                            b = true;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                    return b;
                }
            }
        }

        protected void checkPassword(object sender, EventArgs e) {
            int score = passwordStrength(tb_password.Text);
            string status = "";
            switch (score) {
                case 0:
                    status = "Very Weak";
                    break;
                case 1:
                    status = "Weak";
                    break;
                case 2:
                    status = "Average";
                    break;
                case 3:
                    status = "Strong";
                    break;
                case 4:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Visible = true;
            lbl_pwdchecker.Text = status;
            if (score < 3)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
            }
            else {
                lbl_pwdchecker.ForeColor = Color.Green;
            }
        }

        public class MyObject 
        { 
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha() 
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            ("https://www.google.com/recaptcha/api/siteverify?secret=6Lf6dmIeAAAAAO1_xdMi3SseET_3_47vUzDCIr-V &response=" + captchaResponse);

            try
            {
                using (WebResponse webResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);

                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void btn_register_click(object sender, EventArgs e) 
        {
            string firstName = HttpUtility.HtmlEncode(tb_first_name.Text.Trim());
            string lastName = HttpUtility.HtmlEncode(tb_last_name.Text.Trim());
            string username = HttpUtility.HtmlEncode(tb_username.Text.Trim());
            string creditCard = HttpUtility.HtmlEncode(tb_credit_card.Text.Trim());
            string email = HttpUtility.HtmlEncode(tb_email.Text.Trim());
            string password = HttpUtility.HtmlEncode(tb_password.Text.Trim());
            string dob = HttpUtility.HtmlEncode(tb_dateofbirth.Text.Trim());
            bool validDOB = true;
            try
            {
                DateTime dateOfBirth = DateTime.Parse(dob);
            }
            catch 
            {
                validDOB = false;
            }
            
            Boolean validate = true;
            if (ValidateCaptcha())
            {
                if (!(Regex.IsMatch(firstName, "^[a-zA-Z0-9_]{2,15}$") && Regex.IsMatch(lastName, "^[a-zA-Z0-9_]{2,15}$")))
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "First name and last name must be 2-15 characters long without special characters";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (!Regex.IsMatch(username, "^[a-zA-Z0-9_]{4,20}$"))
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Username must be 4-20 characters long without special characters";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (checkName(username))
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Username already exists";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (!Regex.IsMatch(creditCard, "^[0-9]{10,20}$"))
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Credit card number must be between 10-20 numbers and contain only digits";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (!(Regex.IsMatch(email, "[a-zA-Z0-9]+@+[a-z]+.+[a-z]{2,3}"))) 
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Invalid email";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (passwordStrength(password) < 3)
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Invalid password";
                    lbl_errormsg.ForeColor = Color.Red;
                }
                else if (!(validDOB))
                {
                    validate = false;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Invalid date of birth";
                    lbl_errormsg.ForeColor = Color.Red;
                }

                // validation success v
                if (validate) 
                {
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); 
                    byte[] saltByte = new byte[8];
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string creditCardWithSalt = creditCard + salt;
                    creditCardHash = Convert.ToBase64String(hashing.ComputeHash(Encoding.UTF8.GetBytes(creditCardWithSalt)));
                    string passwordWithSalt = password + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(password));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    finalPasswordHash = Convert.ToBase64String(hashWithSalt);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    lbl_errormsg.Visible = true;
                    lbl_errormsg.Text = "Account registered";
                    lbl_errormsg.ForeColor = Color.Green;
                    createAccount();
                    btn_register.Visible = false;
                }
            }
        }

        protected void createAccount()
        {
            string firstName = HttpUtility.HtmlEncode(tb_first_name.Text.Trim());
            string lastName = HttpUtility.HtmlEncode(tb_last_name.Text.Trim());
            string name = firstName + " " + lastName;
            string username = HttpUtility.HtmlEncode(tb_username.Text.Trim());
            string creditCard = HttpUtility.HtmlEncode(tb_credit_card.Text.Trim());
            string email = HttpUtility.HtmlEncode(tb_email.Text.Trim());
            string password = HttpUtility.HtmlEncode(tb_password.Text.Trim());
            string photo = file_photo.ToString();
            DateTime dateOfBirth = DateTime.Parse(HttpUtility.HtmlEncode(tb_dateofbirth.Text.Trim()));
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES(@Name,@Username,@CreditCard,@Email,@PasswordHash,@Salt,@DateOfBirth,@Photo,0,null,0,@PasswordDateTime,null)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@CreditCard", creditCardHash);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalPasswordHash);
                            cmd.Parameters.AddWithValue("@Salt", salt);
                            cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                            cmd.Parameters.AddWithValue("@Photo", photo);
                            cmd.Parameters.AddWithValue("@PasswordDateTime", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null; 
            try
            {
                RijndaelManaged cipher = new RijndaelManaged(); 
                cipher.IV = IV; 
                cipher.Key = Key; 
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,plainText.Length);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            { }
            return cipherText;
        }
        protected void btn_login_click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx", false);
        }
    }
}