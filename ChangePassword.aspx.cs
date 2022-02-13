using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_login_click(object sender, EventArgs e) 
        {
            Response.Redirect("Login.aspx", false);
        }

        protected void checkChange(object sender, EventArgs e) 
        {
            string username = HttpUtility.HtmlEncode(tb_username.Text.Trim());
            string password = HttpUtility.HtmlEncode(tb_password.Text.Trim());
            string newPassword = HttpUtility.HtmlEncode(tb_new.Text.Trim());
            string confirm = HttpUtility.HtmlEncode(tb_confirm.Text.Trim());

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(username);
            string dbSalt = getDBSalt(username);

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0) 
                {
                    string passwordWithSalt = password + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    string passwordHash = Convert.ToBase64String(hashWithSalt);
                    string newPasswordWithSalt = newPassword + dbSalt;
                    byte[] newHashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPasswordWithSalt));
                    string newPasswordHash = Convert.ToBase64String(newHashWithSalt);
                    if (passwordHash.Equals(dbHash))
                    {
                        int lockout = 0;
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                        {
                            SqlCommand command = new SqlCommand("select Lockout FROM Users WHERE Username=@Username", connection);
                            command.Parameters.AddWithValue("@Username", username);
                            using (command)
                            {
                                connection.Open();
                                try
                                {
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader["Lockout"] != null)
                                            {
                                                if (reader["Lockout"] != DBNull.Value)
                                                {
                                                    lockout = (int)reader["Lockout"];
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
                            }
                        }
                        if (lockout == 0) 
                        {
                            if (passwordStrength(newPassword) < 3)
                            {
                                lbl_errormsg.Visible = true;
                                lbl_errormsg.Text = "New password must be between 12-20 characters and have a mix of lower-case letters, upper-case letters and numbers";
                            }
                            else 
                            {
                                if (newPassword == confirm)
                                {
                                    DateTime dt = DateTime.Now;
                                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                                    {
                                        SqlCommand command = new SqlCommand("select PasswordDateTime FROM Users WHERE Username=@Username", connection);
                                        command.Parameters.AddWithValue("@Username", username);
                                        using (command)
                                        {
                                            connection.Open();
                                            try
                                            {
                                                using (SqlDataReader reader = command.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        if (reader["PasswordDateTime"] != null)
                                                        {
                                                            if (reader["PasswordDateTime"] != DBNull.Value)
                                                            {
                                                                dt = (DateTime)reader["PasswordDateTime"];
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
                                        }
                                    }
                                    if ((DateTime.Now - dt).TotalSeconds > 300)
                                    {
                                        string oldPasswordHash = null;
                                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                                        {
                                            SqlCommand command = new SqlCommand("select OldPasswordHash FROM Users WHERE Username=@Username", connection);
                                            command.Parameters.AddWithValue("@Username", username);
                                            using (command)
                                            {
                                                connection.Open();
                                                try
                                                {
                                                    using (SqlDataReader reader = command.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {
                                                            if (reader["OldPasswordHash"] != null)
                                                            {
                                                                if (reader["OldPasswordHash"] != DBNull.Value)
                                                                {
                                                                    oldPasswordHash = reader["OldPasswordHash"].ToString();
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
                                            }
                                        }
                                        if (newPasswordHash != passwordHash && newPasswordHash != oldPasswordHash)
                                        {
                                            try
                                            {
                                                using (SqlConnection connection = new SqlConnection(ConnectionString))
                                                {
                                                    SqlCommand command = new SqlCommand("update Users SET PasswordHash=@PasswordHash, OldPasswordHash=@OldPasswordHash, PasswordDateTime=@PasswordDateTime WHERE Username=@Username", connection);
                                                    using (command)
                                                    {
                                                        using (SqlDataAdapter sda = new SqlDataAdapter())
                                                        {
                                                            command.Parameters.AddWithValue("@Username", username);
                                                            command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                                                            command.Parameters.AddWithValue("@OldPasswordHash", passwordHash);
                                                            command.Parameters.AddWithValue("@PasswordDateTime", DateTime.Now);
                                                            connection.Open();
                                                            command.ExecuteNonQuery();
                                                            connection.Close();
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new Exception(ex.ToString());
                                            }
                                            lbl_errormsg.Visible = true;
                                            lbl_errormsg.Text = "Password successfully changed";
                                            lbl_errormsg.ForeColor = Color.Green;
                                            btn_change.Visible = false;
                                        }
                                        else 
                                        {
                                            lbl_errormsg.Visible = true;
                                            lbl_errormsg.Text = "New password cannot be the same as the two previous passwords.";
                                        }
                                    }
                                    else
                                    {
                                        lbl_errormsg.Visible = true;
                                        lbl_errormsg.Text = "Password cannot be changed this early.";
                                    }
                                }
                                else 
                                {
                                    lbl_errormsg.Visible = true;
                                    lbl_errormsg.Text = "New password and confirm password don't match.";
                                }
                            }
                        }
                        else 
                        {
                            lbl_errormsg.Visible = true;
                            lbl_errormsg.Text = "This account has been locked. You cannot change its password.";
                        }
                    }
                    else 
                    {
                        lbl_errormsg.Visible = true;
                        lbl_errormsg.Text = "Wrong username or password";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private int passwordStrength(string password)
        {
            int score = 0;
            if (password.Length >= 12 && password.Length <= 20)
            {
                if (Regex.IsMatch(password, "[a-z]"))
                {
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
                if (Regex.IsMatch(password, "[^a-zA-Z0-9]") && score == 3)
                {
                    score += 1;
                }
            }
            return score;
        }
        protected string getDBHash(string username)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string sql = "select PasswordHash FROM Users WHERE Username=@Username";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", username);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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
            return h;
        }

        protected string getDBSalt(string username)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string sql = "select Salt FROM Users WHERE Username=@Username";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", username);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Salt"] != null)
                        {
                            if (reader["Salt"] != DBNull.Value)
                            {
                                s = reader["Salt"].ToString();
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
            return s;
        }
    }
}