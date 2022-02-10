<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="PracticalAssignment.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h2>Home Page</h2>
    <form id="form1" runat="server">
        <div>
            Welcome to the home page!<br />
            <br />
            <br />
            <asp:Button ID="btn_logout" runat="server" Text="Logout" onClick="Logout" Visible="false"/>
        </div>
    </form>
</body>
</html>
