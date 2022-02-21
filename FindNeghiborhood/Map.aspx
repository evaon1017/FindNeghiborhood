<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="FindNeghiborhood.Map" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        table {
            table-layout: fixed;
            word-wrap:break-word;
        }

        td {
            width: 70px;
            height: 70px;
            text-align: center;
            word-wrap:break-word;
            font-size:x-small;
            overflow:hidden;    
        }

        .canvas {
            overflow:hidden;
            border: 1px solid gray;
            min-height:200px;
        }

        .canvas > * {
            margin: auto auto auto auto;
        }

        .draggable{
            text-align:center;
        }

        h2 {
            margin-top:40px;
            margin-bottom:0px;
            text-align:center;
        }

        body {
            padding-left: 30px;
            padding-right: 30px;
        }
    </style>
    <link href="scripts/jquery-ui.min.css" rel="stylesheet" />
    <script src="node_modules/jquery/dist/jquery.min.js"></script>
    <script src="scripts/jquery-ui.min.js"></script>
    <script src="node_modules/jquery-ui-touch-punch/jquery.ui.touch-punch.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Repeater runat="server" ID="UI_repCities">
            <ItemTemplate>
                <h2 runat="server" id="UI_mapName"></h2>
                <div class="canvas" runat="server" id="canvas">
                    <style id="dynamic-css"></style>
                    <table cellspacing="0" border="1" class="draggable ui-widget-content">
                        <caption></caption>
                        <asp:PlaceHolder runat="server" ID="UI_ph" />
                    </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </form>

    <script>
        $(function () {
            $(".draggable").draggable();
        });

        $('table.draggable').each(function (idx, el) {
            $(el).css('width', ($(el).find('tr').first().find('td').length * 70).toString() + 'px');
        });
    </script>
</body>
</html>
