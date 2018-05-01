<%@ Page Language="C#"  ClientIDMode="Static" AutoEventWireup="true" CodeBehind="TestForm.aspx.cs" Inherits="HeadlinesWithLocations.TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Find City/Country Names</title>
    <style>
        .loader {
            margin:auto;
            border: 16px solid #f3f3f3;
            border-radius: 50%;
            border-top: 16px solid #3498db;
            width: 120px;
            height: 120px;
            -webkit-animation: spin 2s linear infinite;
            animation: spin 2s linear infinite;
        }
        .center {
            position:relative;
            padding: 70px 0;
            text-align: center;
            margin: auto;
            width: 50%;
            top: 200px;
            border: 3px solid grey;
        }
        /* Safari */
        @-webkit-keyframes spin {
            0% {
                -webkit-transform: rotate(0deg);
            }

            100% {
                -webkit-transform: rotate(360deg);
            }
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="center" runat="server" id="window">
                <h2 id="text"></h2>
                <div id="container">
                    <input  runat="server" id="startButton" type="button" value="Click to start" onclick="if(showLoader())" onserverclick="ButtonClicked"/>
                </div>                
            </div>
        </div>
    </form>
    <script>
        function showLoader() {
            var container = document.getElementById("container");
            container.innerHTML = "";
            container.className = "loader";
            document.getElementById("text").innerHTML = "This will take a few minutes.";
            return true;
        }
    </script>
</body>
</html>
