<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RichTextEditor.ascx.cs"
    Inherits="Controls_RichTextEditor" %>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div class="post" style="vertical-align: top; width: 100%">
                <div id="content" runat="server">
                    <table style="border-right: #000000 thin solid; border-top: #000000 thin solid; border-left: #000000 thin solid;
                        border-bottom: #000000 thin solid; background-color: buttonface">
                        <tr>
                            <td align="center">
                                <div>
                                    <img src="images/bold.gif" style="cursor: hand" title="make bold" id="bold" onclick="fontEdit('bold')">
                                    <img src="images/italic.gif" style="cursor: hand" title="make italic" id="italic"
                                        onclick="fontEdit('italic')">
                                    <img src="images/underline.gif" style="cursor: hand" title="underline selected" id="underline"
                                        onclick="fontEdit('underline')">
                                    <img src="images/left-align.gif" style="cursor: hand" title="justify left" onclick="fontEdit('justifyleft')">
                                    <img src="images/center-align.gif" style="cursor: hand" title="justify center" onclick="fontEdit('justifycenter')">
                                    <img src="images/right-align.gif" style="cursor: hand" title="justify right" onclick="fontEdit('justifyright')">
                                    <select id="fonts" onchange="fontEdit('fontname',this[this.selectedIndex].value)">
                                        <option value="Arial" selected>Arial</option>
                                        <option value="Comic Sans MS">Comic Sans MS</option>
                                        <option value="Courier New">Courier New</option>
                                        <option value="Tahoma">Tahoma</option>
                                        <option value="Times">Times</option>
                                        <option value="Latha">Tamil</option>
                                    </select>
                                    <select id="size" onchange="fontEdit('fontsize',this[this.selectedIndex].value)">
                                        <option value="1" selected>1</option>
                                        <option value="2">2</option>
                                        <option value="3">3</option>
                                        <option value="4">4</option>
                                        <option value="5">5</option>
                                    </select>
                                    <div id="colorMatrix" style="position: absolute; padding-top: 26px; display: none;"
                                        onmouseout="this.style.display='none';" onmouseover="this.style.display='block';">
                                        <table cellpadding="1" cellspacing="1" border="0" style="background-color: buttonface;
                                            border: black thin solid; border-top: black thin solid; border-left: black thin solid;
                                            border-bottom: black thin solid">
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','aqua'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: aqua;"><span style="color: aqua; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','black'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: black;"><span style="color: black; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','blue'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: blue;"><span style="color: blue; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','fuchsia'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: fuchsia;"><span style="color: fuchsia; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','gray'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: gray;"><span style="color: gray; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','green'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: green;"><span style="color: green; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','lime'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: lime;"><span style="color: lime; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','maroon'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: maroon;"><span style="color: maroon; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','navy'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: navy;"><span style="color: navy; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','olive'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: olive;"><span style="color: olive; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','purple'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: purple;"><span style="color: purple; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','red'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: red;"><span style="color: red; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','silver'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: silver;"><span style="color: silver; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','teal'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: teal;"><span style="color: teal; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','white'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: white;"><span style="color: white; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('ForeColor','yellow'); document.getElementById('colorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: yellow;"><span style="color: yellow; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td colspan="4">
                                                    #<input type="text" maxlength="6" id="customColorVal" style="width: 40px; height: 20px;">
                                                    <a onclick="var color = document.getElementById('customColorVal').value;  fontEdit('ForeColor','#'+color); document.getElementById('colorMatrix').style.display = 'none'"
                                                        href="#">Apply</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <img src="images/font-color.gif" style="cursor: hand" title="change font color" onclick="document.getElementById('colorMatrix').style.display = 'block'">
                                    <div id="backGoroundColorMatrix" style="position: absolute; padding-top: 26px; display: none;"
                                        onmouseout="this.style.display='none';" onmouseover="this.style.display='block';">
                                        <table cellpadding="1" cellspacing="1" border="0" style="background-color: buttonface;
                                            border: black thin solid; border-top: black thin solid; border-left: black thin solid;
                                            border-bottom: black thin solid" id="Table1">
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','aqua'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: aqua;"><span style="color: aqua; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','black'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: black;"><span style="color: black; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','blue'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: blue;"><span style="color: blue; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','fuchsia'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: fuchsia;"><span style="color: fuchsia; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','gray'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: gray;"><span style="color: gray; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','green'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: green;"><span style="color: green; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','lime'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: lime;"><span style="color: lime; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','maroon'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: maroon;"><span style="color: maroon; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','navy'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: navy;"><span style="color: navy; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','olive'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: olive;"><span style="color: olive; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','purple'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: purple;"><span style="color: purple; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','red'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: red;"><span style="color: red; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','silver'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: silver;"><span style="color: silver; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','teal'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: teal;"><span style="color: teal; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','white'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: white;"><span style="color: white; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                                <td>
                                                    <a onclick="fontEdit('BackColor','yellow'); document.getElementById('backGoroundColorMatrix').style.display = 'none'; return false;"
                                                        href="#" style="background-color: yellow;"><span style="color: yellow; text-decoration: none">
                                                            ---</span></a>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td colspan="4">
                                                    #<input type="text" maxlength="6" id="backGroundcolorVal" style="width: 40px; height: 20px;"
                                                        name="Text1">
                                                    <a onclick="var color = document.getElementById('backGroundcolorVal').value;  fontEdit('BackGroundColor','#'+color); document.getElementById('backGoroundColorMatrix').style.display = 'none'"
                                                        href="#">Apply</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <img src="images/background-color.gif" style="cursor: hand" title="change background color"
                                        onclick="document.getElementById('backGoroundColorMatrix').style.display = 'block'">
                                    <img src="images/bullets.gif" style="cursor: hand" title="make as unordered list"
                                        onclick="fontEdit('insertunorderedlist')">
                                    <img src="images/numbers.gif" style="cursor: hand" title="make as ordered list" onclick="fontEdit('insertorderedlist')">
                                    <img src="images/left-indent.gif" style="cursor: hand" title="left indent" onclick="fontEdit('outdent')">
                                    <img src="images/right-indent.gif" style="cursor: hand" title="right indent" onclick="fontEdit('indent')">
                                    <img src="images/insert-image.gif" style="cursor: hand" title="insert image" onclick="var url = prompt('Enter the image url to insert-', ''); if(url != null && url != '') insertImage(url)">
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <div id='testDiv'>
                                </div>
                                <textarea class="Controls_RichTextEditor" style="display: none" id="textArea" runat="server" name="textArea"></textarea>
                            </td>
                        </tr>
                    </table>
                    <br />
                </div>
                <br style="clear: both">
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="paging">
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript">
function def()
{
    var testframe =document.createElement("iframe");
    testframe.name = testframe.id = "textEditor";
    if (testframe.addEventListener)
    {
        testframe.addEventListener("load",function(e){this.contentWindow.document.designMode = "on";}, false);
    } 
    else if (testframe.attachEvent)
    {
        testframe.attachEvent("load", function(e){this.contentWindow.document.designMode = "on";});
    }
    var divElem = document.getElementById('testDiv');
    divElem.appendChild(testframe);
    textEditor.document.designMode="on";
    textEditor.document.open();
    var textAreaID = '<%=this.textArea.ClientID %>';
    textEditor.document.write('<head><style type="text/css">body{ font-family:arial; font-size:13px; }</style> </head><body>' + document.getElementById(textAreaID).value  + '</body>');
    textEditor.document.close();
    //textEditor.focus();
    testframe.style.width = '<%=this.EditorWidth %>' + 'px';
    testframe.style.height = '<%=this.EditorHeight %>' + 'px';
    testframe.style.backgroundColor = 'white' ;
}
function fontEdit(x,y)
{
    textEditor.document.execCommand(x,"",y);
    textEditor.focus();
}
function insertImage(imgUrl)
{
	if(imgUrl != '')
	{
		textEditor.document.execCommand('InsertImage',false,imgUrl);
	}
    textEditor.focus();
}
def();
</script>

