<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>
        <%:ViewData["title"] %></title>
    <link href="../../Content/default.css" rel="stylesheet" type="text/css" />
    <link href="../../Content/papershow.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
    <script src="../../Scripts/swfobject.js" type="text/javascript"></script>
    <script src="../../Scripts/papershow.js" type="text/javascript"></script>
</head>
<body>
    <%using (Html.BeginForm())
      { %>
    <div class="paperShow">
        <div style="display: none">
            <input type="hidden" id="PaperID" name="PaperID" />
            <input type="hidden" id="StartTime" name="StartTime" />
            <input type="text" id="Continues" name="Continues" />
            <!--快速阅读
            time1:开始做快速阅读的时间; 点击下一题时候由一个题型切换到另一个题型时候保存当前题型用时到time1
            temptime1:暂存做题的时间
            sspc：题型下的大题个数
            SspcTotalNum：题型下的小题总数，统计正确率使用


            -->
            <input type="text" id="time1" name="time1" readonly="readonly" />
            <input type="text" id="temptime1" name="temptime1" value="0" readonly="readonly" />
            <input type="text" id="Sspc" name="Sspc" style="display: none;" />
            <input type="text" id="SspcTotalNum" name="SspcTotalNum" style="display: none;" />
            <!--短对话-->
            用时:<input type="text" id="time2" name="time2" readonly="readonly" />
            <input type="text" id="temptime2" name="temptime2" value="0" readonly="readonly" />
            <input type="text" id="Slpo" name="Slpo" style="display: none;" />
            <input type="text" id="SlpoTotalNum" name="SlpoTotalNum" style="display: none;" />
            <!--长对话-->
            用时:<input type="text" id="time3" name="time3" readonly="readonly" />
            <input type="text" id="temptime3" name="temptime3" value="0" readonly="readonly" />
            <input type="text" id="Llpo" name="Llpo" style="display: none;" />
            <input type="text" id="LlpoTotalNum" name="LlpoTotalNum" style="display: none;" />
            <!--短文听力理解-->
            用时:<input type="text" id="time4" name="time4" readonly="readonly" />
            <input type="text" id="temptime4" name="temptime4" value="0" readonly="readonly" />
            <input type="text" id="Rlpo" name="Rlpo" style="display: none;" />
            <input type="text" id="RlpoTotalNum" name="RlpoTotalNum" style="display: none;" />
            <!--复合型听力-->
            用时:<input type="text" id="time5" name="time5" readonly="readonly" />
            <input type="text" id="temptime5" name="temptime5" value="0" readonly="readonly" />
            <input type="text" id="Lpc" name="Lpc" style="display: none;" />
            <input type="text" id="LpcTotalNum" name="LpcTotalNum" style="display: none;" />
            <!--阅读理解-选词填空-->
            用时:<input type="text" id="time6" name="time6" readonly="readonly" />
            <input type="text" id="temptime6" name="temptime6" value="0" readonly="readonly" />
            <input type="text" id="Rpc" name="Rpc" style="display: none;" />
            <input type="text" id="RpcTotalNum" name="RpcTotalNum" style="display: none;" />
            <!--阅读理解-选择题型-->
            用时:<input type="text" id="time7" name="time7" readonly="readonly" />
            <input type="text" id="temptime7" name="temptime7" value="0" readonly="readonly" />
            <input type="text" id="Rpo" name="Rpo" style="display: none;" />
            <input type="text" id="RpoTotalNum" name="RpoTotalNum" style="display: none;" />
            <!--阅读理解-信息匹配-->
            用时:<input type="text" id="time8" name="time8" readonly="readonly" />
            <input type="text" id="temptime8" name="temptime8" value="0" readonly="readonly" />
            <input type="text" id="InfoMat" name="InfoMat" style="display: none;" />
            <input type="text" id="InfoMatTotalNum" name="InfoMatTotalNum" style="display: none;" />
            <!--完形填空-->
            用时:<input type="text" id="time9" name="time9" readonly="readonly" />
            <input type="text" id="temptime9" name="temptime9" value="0" readonly="readonly" />
            <input type="text" id="Cp" name="Cp" style="display: none;" />
            <input type="text" id="CpTotalNum" name="CpTotalNum" style="display: none;" />
            <input type="submit" id="Submin" value="提交" style="display: none;" />
            <input type="text" id="Temp" name="Temp" value="1" style="display: none;" />
        </div>
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
                    已用时间 <span class="timeUsed"></span>剩余时间 <span class="timeRemaind"></span><span class="time">
                    </span>
                </div>
                <div class="paperControls">
                    <a class="btnQuit" href="#link">退出测试</a> <a class="btnPrev" href="#link">上一题</a>
                    <a class="btnNext" href="#link">下一题</a> <a class="btnSubmit" href="#link">提交</a>
                </div>
                <div id="GlobalPlayer">
                </div>
                <div id="LcControl">
                </div>
            </div>
        </div>
    </div>
</body>
<%} %>
</html>
