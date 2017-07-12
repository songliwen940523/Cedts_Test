<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>题目详情</title>
    <link href="../../../../Content/default.css" rel="stylesheet" type="text/css" />
    <link href="../../../../Content/papershow.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/swfobject.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../../../Scripts/itemdetail.js"></script>
</head>
<body>
    <div class="paperShow">
        <div class="paperTitle wrp">
            <h1>
                <label id="Title">
                </label>
            </h1>
        </div>
        <div id="Navigation" class="paperNav">
        </div>
        <div id="paperBody" class="paperBody wrp">
        </div>
        <div class="paperFoot">
            <div class="wrp">
                <div class="paperInfo">
                </div>
                <div class="paperControls">
                    <a class="btnQuit" href="#link">返回</a> <a class="btnPrev" href="#link">上一题</a>
                    <a class="btnNext" href="#link">下一题</a>
                </div>
                <div id="GlobalPlayer">
                </div>
                <div id="LcControl">
                </div>
            </div>
        </div>
    </div>
    <div style="display:none;"> <input type="text" id="AssessmentItemID" value="<%:ViewData["AssessmentItemID"] %>" /></div>
</body>
</html>
