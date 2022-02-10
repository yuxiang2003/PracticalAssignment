<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="PracticalAssignment.WebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration Page</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Lf6dmIeAAAAAHWMO1YdpTtP5wJFs8XR0WMoKNne"></script>
</head>
<body>
    <h2>Registration Page</h2>
    <form id="form1" runat="server">
        <div>
            <p>First Name <asp:TextBox ID="tb_first_name" runat="server" Width="200px"></asp:TextBox></p>
            <p>Last Name <asp:TextBox ID="tb_last_name" runat="server" Width="200px"></asp:TextBox></p>
            <p>Username <asp:TextBox ID="tb_username" runat="server" Width="200px"></asp:TextBox></p>
            <p>Credit Card Number <asp:TextBox ID="tb_credit_card" runat="server" Width="200px" TextMode="Phone"></asp:TextBox></p>
            <p>Email Address <asp:TextBox ID="tb_email" runat="server" Width="200px" TextMode="Email"></asp:TextBox></p>
            <p>Password <asp:TextBox ID="tb_password" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                &nbsp;<asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker" Visible="false"></asp:Label></p>
            <p><asp:Button ID="btn_checkPassword" runat="server" Text="Check password strength" onClick="checkPassword"/></p>
            <p>Date of Birth<asp:TextBox ID="tb_dateofbirth" runat="server" Width="200px" TextMode="Date"></asp:TextBox></p>
            <p>Photo <asp:FileUpload ID="file_photo" runat="server" accept=".png, .jpg, .jpeg"/></p>
            <p><asp:Label ID="lbl_errormsg" runat="server" Text="Error Message" Visible="False" ForeColor="Red"></asp:Label></p>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <p><asp:Button ID="btn_register" runat="server" Text="Submit" onClick="btn_register_click"/><asp:Button ID="btn_login" runat="server" Text="Go back to Login Page" onClick="btn_login_click"/></p>
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
