<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="PracticalAssignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h2>Change Password</h2>
    <form id="form1" runat="server">
        <div>
            <p>Username <asp:TextBox ID="tb_username" runat="server" Width="200px"></asp:TextBox></p>
            <p>Old Password <asp:TextBox ID="tb_password" runat="server" Width="200px" TextMode="Password"></asp:TextBox></p>
            <p>New Password <asp:TextBox ID="tb_new" runat="server" Width="200px" TextMode="Password"></asp:TextBox></p>
            <p>Confirm Password <asp:TextBox ID="tb_confirm" runat="server" Width="200px" TextMode="Password"></asp:TextBox></p>
            <p><asp:Button ID="btn_change" runat="server" Text="Submit" onClick="checkChange"/><asp:Button ID="btn_login" runat="server" Text="Go back to Login Page" onClick="btn_login_click"/></p>
            <p><asp:Label ID="lbl_errormsg" runat="server" Text="Error Message" Visible="False" ForeColor="Red"></asp:Label></p>
        </div>
    </form>
</body>
</html>
