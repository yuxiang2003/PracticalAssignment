<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authentication.aspx.cs" Inherits="PracticalAssignment.Authentication" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Authenticating...</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lbl_auth" runat="server" Text="Authentication" Visible="False" Font-Size="Larger"></asp:Label>
        <div>
            <p>Enter verification code <asp:TextBox ID="tb_authentication" runat="server" Width="200px"></asp:TextBox></p>
            <p><asp:Button ID="btn_submit" runat="server" Text="Submit" /></p>
        </div>
    </form>
</body>
</html>
