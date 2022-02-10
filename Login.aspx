<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PracticalAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login page</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Lf6dmIeAAAAAHWMO1YdpTtP5wJFs8XR0WMoKNne"></script>
</head>
<body>
    <h2>Login Page</h2>
    <form id="form1" runat="server">
        <div>
            <p>Username <asp:TextBox ID="tb_username" runat="server" Width="200px"></asp:TextBox></p>
            <p>Password <asp:TextBox ID="tb_password" runat="server" Width="200px" TextMode="Password"></asp:TextBox></p>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <p><asp:Button ID="btn_login" runat="server" Text="Submit" onClick="checkLogin"/></p>
            <p><asp:Label ID="lbl_errormsg" runat="server" Text="Error Message" Visible="False" ForeColor="Red"></asp:Label></p>
        </div>
        <hr>
        <div>
            <p>No Account? Register here.</p>
            <p><asp:Button ID="btn_register" runat="server" Text="Register" onClick="btn_register_click"/></p>
        </div>
        <hr>
        <div>
            <p>Change Password here.</p>
            <p><asp:Button ID="btn_change" runat="server" Text="Change password" onClick="btn_change_click"/></p>
        </div>
    </form>
        
    <script>
        grecaptcha.ready(
            function () {
                grecaptcha.execute('6Lf6dmIeAAAAAHWMO1YdpTtP5wJFs8XR0WMoKNne', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
    </script>
</body>
</html>
