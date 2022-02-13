using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            List<String> lockedOutAccounts = new List<string>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select Id FROM Users WHERE Lockout=1", connection);
                using (command)
                {
                    connection.Open();
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Id"] != null)
                                {
                                    if (reader["Id"] != DBNull.Value)
                                    {
                                        lockedOutAccounts.Add(reader["Id"].ToString());
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
            foreach (string account in lockedOutAccounts) {
                DateTime dt = DateTime.Now;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("select LockoutDateTime FROM Users WHERE Id=@Id", connection);
                    command.Parameters.AddWithValue("@Id", account);
                    using (command)
                    {
                        connection.Open();
                        try
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["LockoutDateTime"] != null)
                                    {
                                        if (reader["LockoutDateTime"] != DBNull.Value)
                                        {
                                            dt = (DateTime)reader["LockoutDateTime"];
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
                if ((DateTime.Now - dt).TotalSeconds > 60) 
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                        {
                            SqlCommand command = new SqlCommand("update Users SET Lockout=0, LockoutDateTime=Null, FailedLoginAttempts=0 WHERE Id=@Id", connection);
                            using (command)
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    command.Parameters.AddWithValue("@Id", account);
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
                }
            }
        }
        protected void checkLogin(object sender, EventArgs e) 
        {
            string username = HttpUtility.HtmlEncode(tb_username.Text.Trim());
            string password = HttpUtility.HtmlEncode(tb_password.Text.Trim());

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(username);
            string dbSalt = getDBSalt(username);

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string passwordWithSalt = password + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
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
                            if ((DateTime.Now - dt).TotalSeconds < 600)
                            {
                                Session["LoggedIn"] = tb_username.Text.Trim();
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                Response.Redirect("HomePage.aspx", false);
                                try
                                {
                                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                                    {
                                        SqlCommand command = new SqlCommand("update Users SET FailedLoginAttempts=0 WHERE Username=@Username", connection);
                                        using (command)
                                        {
                                            using (SqlDataAdapter sda = new SqlDataAdapter())
                                            {
                                                command.Parameters.AddWithValue("@Username", username);
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
                            }
                            else 
                            {
                                lbl_errormsg.Visible = true;
                                lbl_errormsg.Text = "Account password needs to be changed";
                            }
                        }
                        else
                        {
                            lbl_errormsg.Visible = true;
                            lbl_errormsg.Text = "Account has been locked";
                        }
                    }
                    else
                    {
                        int fails = 0;
                        using (SqlConnection connection = new SqlConnection(ConnectionString)) {
                            SqlCommand command = new SqlCommand("select FailedLoginAttempts FROM Users WHERE Username=@Username", connection);
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
                                            if (reader["FailedLoginAttempts"] != null)
                                            {
                                                if (reader["FailedLoginAttempts"] != DBNull.Value)
                                                {
                                                    fails = (int)reader["FailedLoginAttempts"];
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
                        if (fails >= 2)
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
                                try
                                {
                                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                                    {
                                        SqlCommand command = new SqlCommand("update Users SET Lockout=1, LockoutDateTime=@LDT WHERE Username=@Username", connection);
                                        using (command)
                                        {
                                            using (SqlDataAdapter sda = new SqlDataAdapter())
                                            {
                                                command.Parameters.AddWithValue("@Username", username);
                                                command.Parameters.AddWithValue("@LDT", DateTime.Now);
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
                            }
                            lbl_errormsg.Visible = true;
                            lbl_errormsg.Text = "Account has been locked";
                        }
                        else
                        {
                            fails += 1;
                            try 
                            {
                                using (SqlConnection connection = new SqlConnection(ConnectionString))
                                {
                                    SqlCommand command = new SqlCommand("update Users SET FailedLoginAttempts=@NewFails WHERE Username=@Username", connection);
                                    using (command)
                                    {
                                        using (SqlDataAdapter sda = new SqlDataAdapter())
                                        {
                                            command.Parameters.AddWithValue("@NewFails", fails);
                                            command.Parameters.AddWithValue("@Username", username);
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
                            lbl_errormsg.Text = "Wrong username or password";
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.ToString());
            }

        }

        protected string getDBHash(string username) {
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

        protected void btn_register_click(object sender, EventArgs e) 
        {
            Response.Redirect("Registration.aspx", false);
        }

        protected void btn_change_click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx", false);
        }
    }
}