<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication2.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        .style2
        {
            width: 234px;
        }
        .style3
        {
            width: 240px;
        }
        .style4
        {
            width: 377px;
            height: 553px;
        }
        .style5
        {
            height: 164px;
        }
        .style6
        {
            width: 303px;
            height: 209px;
        }
        .style7
        {
            width: 373px;
        }
    </style>
<script language="javascript" type="text/javascript">
// <!CDATA[

function Button3_onclick() {

}

// ]]>
</script>
</head>
<body>
    <form id="form1" runat="server" class="style4">
    <div class="style3">
    
        <span lang="hu">vbcdvfdsvfdvfdvdfvdfv vfdsvfdsv vfdsv</span></div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Read" />
    <span lang="hu">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    </span>
    <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Write" />
    <span lang="hu">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    </span>
    <input id="Button3" type="button" value="button" onclick="return Button3_onclick()" /><span 
        lang="hu">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="Button4" runat="server" onclick="Button4_Click" Text="Button" />
    </span><p class="style7">
    <asp:TextBox ID="TextBox1" runat="server" Height="320px" Rows="15" 
            TextMode="MultiLine" Width="240px"></asp:TextBox>
    </p>
    <p class="style7" title="fds">
        <span lang="hu">vfdsvfdvsdvfdsvfdvfdvssfdvdsva vfdavfdv </span>
    </p>
    <p class="style7">
        <textarea id="TextArea1" class="style5" name="S1"></textarea><asp:MultiView 
            ID="MultiView1" runat="server">
            <asp:View ID="View1" runat="server">
            </asp:View>
        </asp:MultiView>
    </p>
    <p class="style2">
        <input id="Text1" class="style6" type="text" /></p>
    </form>
</body>
</html>
