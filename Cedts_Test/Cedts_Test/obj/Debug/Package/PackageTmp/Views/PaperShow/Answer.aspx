<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Answer</title>
</head>
<link href="../../Content/default.css" rel="stylesheet" type="text/css" />
<link href="../../Content/papershow.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
<script src="../../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
<script src="../../Scripts/swfobject.js" type="text/javascript"></script>
<script type="text/javascript" src="../../Scripts/answer.js"></script>
<%--<script type="text/javascript">
    //判断浏览器
    var Type;

    $(function () {
        $("#Btn").click(function () {
            window.location.href = '/PaperShow/UserTestInfo';
        })
        var TestID = $("#TestID").val();
        $.post("/PaperShow/GetPaper", { id: TestID }, function (data) {
            Type = data.Type;
            var Num = 0;
            //添加快速阅读题型
            if (data.SspcList.length > 0) {
                for (var i = 0; i < data.SspcList.length; i++) {
                    $("#SkimmingAndScanning").append("<div id='Sspc" + i + "'><div id='SspcC" + i + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='SspcO" + i + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    $("#SspcC" + i).append("<fieldset style='width: auto;'><legend>第" + (i + 1) + "大题原文：</legend>" + data.SspcList[i].Content + "</fieldset>");
                    for (var j = 0; j < data.SspcList[i].ChoiceNum; j++) {
                        if (data.SspcList[i].Info.AnswerValue[j] != data.SspcList[i].Info.UserAnswer[j]) {
                            var div1 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + j + 1) + "小题：</legend>";
                        }
                        else {
                            var div1 = "<fieldset style='width: auto;'><legend>第" + (Num + j + 1) + "小题：</legend>";
                        }
                        div1 = div1 + data.SspcList[i].Info.Problem[j] + "<br/>";
                        div1 = div1 + "A:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='A'></input>" + data.SspcList[i].Choices[(j * 4)] + "<br/>";
                        div1 = div1 + "B:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='B'></input>" + data.SspcList[i].Choices[(j * 4 + 1)] + "<br/>";
                        div1 = div1 + "C:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='C'></input>" + data.SspcList[i].Choices[(j * 4 + 2)] + "<br/>";
                        if (data.SspcList[i].Choices[(j * 4 + 3)] == "") {
                        }
                        else {
                            div1 = div1 + "D:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='D'></input>" + data.SspcList[i].Choices[(j * 4 + 3)] + "<br/>";
                        }
                        div1 = div1 + "正确答案：" + data.SspcList[i].Info.AnswerValue[j] + "<br/>";
                        div1 = div1 + "</fieldset>";
                        $("#SspcO" + i).append(div1);
                        if (data.SspcList[i].Info.UserAnswer[j] == "A") {
                            $("input[name=SspRadio" + i + "_" + j + "]").get(0).checked = true;
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "B") {
                            $("input[name=SspRadio" + i + "_" + j + "]").get(1).checked = true;
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "C") {
                            $("input[name=SspRadio" + i + "_" + j + "]").get(2).checked = true;
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "D") {
                            $("input[name=SspRadio" + i + "_" + j + "]").get(3).checked = true;
                        }
                        $("#SspAnswerValue" + i + "_" + j).val(data.SspcList[i].Info.AnswerValue[j]);

                    }
                    for (var k = 0; k < data.SspcList[i].TermNum; k++) {
                        if (data.SspcList[i].Info.UserAnswer[(data.SspcList[i].ChoiceNum + k)] != data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)]) {
                            var div2 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "小题：</legend>";
                        }
                        else {
                            var div2 = "<fieldset style='width: auto'><legend>第" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "小题：</legend>";
                        }
                        div2 = div2 + data.SspcList[i].Info.Problem[(data.SspcList[i].ChoiceNum + k)] + "<br/>";
                        div2 = div2 + "答案：" + data.SspcList[i].Info.UserAnswer[(data.SspcList[i].ChoiceNum + k)] + "<br/>";
                        div2 = div2 + "正确答案：" + data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)] + "<br/>";
                        div2 = div2 + "</fieldset>";
                        $("#SspcO" + i).append(div2);
                    }
                    Num = Num + data.SspcList[i].Info.QuestionCount;
                }
            }

            //添加短对话听力题型
            if (data.SlpoList.length > 0) {
                for (var m = 0; m < data.SlpoList.length; m++) {
                    $("#ShortConversations").append("<div id='Slpo" + m + "'><div id='SlpoC" + m + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='SlpoO" + m + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    $("#SlpoC" + m).append("<fieldset style='width: auto;'><legend>第" + (m + 1) + "大题原文：</legend>" + data.SlpoList[m].Script + "</fieldset>");
                    //$("#SlpoO"+m).append("<div id='SlpoO" + m + "' style='width: 100%'></div><br/>");
                    if (data.SlpoList[m].Info.AnswerValue != data.SlpoList[m].Info.UserAnswer) {
                        var div3 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + 1) + "小题：</legend>";
                    }
                    else {
                        var div3 = "<fieldset style='width: auto'><legend>第" + (Num + 1) + "小题：</legend>";
                    }
                    if ($.browser.mozilla) {
                        div3 = div3 + "<img src='../../Images/btn_playaudio.gif' onclick='Short" + m + ".controls.play()'" + "/><br/>";
                        div3 = div3 + "<object type='video/x-ms-wmv' id='Short" + m + "'" + "data='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3'>";
                        div3 = div3 + "<param name='src' value='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3' />";
                        div3 = div3 + "<param name='autostart' value='false' />";
                        div3 = div3 + "<param name='controller' value='true' />";
                        div3 = div3 + "<embed id='Short" + m + "'" + " src='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3' autostart='false' volume='100'></embed>";
                        div3 = div3 + "</object>";
                    }
                    if ($.browser.msie) {
                        div3 = div3 + "<img src='../../Images/btn_playaudio.gif' onclick='Short" + m + ".play()'" + "/><br/>";
                        div3 = div3 + "<div style='display:none'><embed id='Short" + m + "'" + " src='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3' autostart='false' volume='100'></embed></div>";
                    }
                    div3 = div3 + "A:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='A'>" + data.SlpoList[m].Choices[0] + "</input><br/>";
                    div3 = div3 + "B:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='B'>" + data.SlpoList[m].Choices[1] + "</input><br/>";
                    div3 = div3 + "C:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='C'>" + data.SlpoList[m].Choices[2] + "</input><br/>";
                    div3 = div3 + "D:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='D'>" + data.SlpoList[m].Choices[3] + "</input><br/>";
                    div3 = div3 + "正确答案：" + data.SlpoList[m].Info.AnswerValue + "<br/>";
                    div3 = div3 + "</fieldset>";
                    $("#SlpoO" + m).append(div3);
                    if (data.SlpoList[m].Info.UserAnswer == "A") {
                        $("input[name=SlpRadio" + m + "]").get(0).checked = true;
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "B") {
                        $("input[name=SlpRadio" + m + "]").get(1).checked = true;
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "C") {
                        $("input[name=SlpRadio" + m + "]").get(2).checked = true;
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "D") {
                        $("input[name=SlpRadio" + m + "]").get(3).checked = true;
                    }
                    Num = Num + data.SlpoList[m].Info.QuestionCount;
                }
            }

            //添加长对话听力题型
            if (data.LlpoList.length > 0) {
                var Llpo = 0;
                for (var l = 0; l < data.LlpoList.length; l++) {
                    $("#LongConversations").append("<div id='Llpo" + l + "'><div id='LlpoC" + l + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='LlpoO" + l + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    //$("#LongConversations").append("<div id='Llpo" + l + "' style='width: 100%'></div><br/>");
                    $("#LlpoC" + l).append("<fieldset style='width: auto;'><legend>第" + (l + 1) + "大题原文：</legend>" + data.LlpoList[l].Script + "</fieldset>");
                    if ($.browser.mozilla) {
                        $("#LlpoO" + l).append("<img src='../../Images/btn_playaudio.gif' onclick='Long" + l + ".controls.play()'" + "/><br/>");
                        $("#LlpoO" + l).append("<object type='video/x-ms-wmv' id='Long" + l + "'" + "data='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3'><param name='src' value='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3' /><param name='autostart' value='false' /><param name='controller' value='true' /><embed id='Long" + l + "'" + " src='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3' autostart='false' volume='100'></embed></object>");
                    }
                    if ($.browser.msie) {
                        $("#LlpoO" + l).append("<img src='../../Images/btn_playaudio.gif' onclick='Long" + l + ".play()'" + "/>");
                        $("#LlpoO" + l).append("<div style='display:none'><embed id='Long" + l + "'" + " src='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3' autostart='false' volume='100'></embed></div>");
                    }
                    for (var n = 0; n < data.LlpoList[l].Info.QuestionCount; n++) {
                        if (data.LlpoList[l].Info.AnswerValue[n] != data.LlpoList[l].Info.UserAnswer[n]) {
                            var div4 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + n + 1) + "小题：</legend>";
                        }
                        else {
                            var div4 = "<fieldset style='width: auto;'><legend>第" + (Num + n + 1) + "小题：</legend>";
                        }
                        if ($.browser.mozilla) {
                            div4 = div4 + "<img src='../../Images/btn_playaudio.gif' onclick='Long" + l + "_" + n + ".controls.play()'" + "/><br/>";
                            div4 = div4 + "<object type='video/x-ms-wmv' id='Long" + l + "_" + n + "'" + "data='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3'>";
                            div4 = div4 + "<param name='src' value='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3' />";
                            div4 = div4 + "<param name='autostart' value='false' />";
                            div4 = div4 + "<param name='controller' value='true' />";
                            div4 = div4 + "<embed id='Long" + l + "_" + n + "'" + " src='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3' autostart='false' volume='100'></embed>";
                            div4 = div4 + "</object>";
                        }
                        else {
                            div4 = div4 + "<img src='../../Images/btn_playaudio.gif' onclick='Long" + l + "_" + n + ".play()'" + "/><br/>";
                            div4 = div4 + "<div style='display:none'><embed id='Long" + l + "_" + n + "'" + " src='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3' autostart='false' volume='100'></embed></div>";
                        }
                        div4 = div4 + "A:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='A'>" + data.LlpoList[l].Choices[(n * 4 + 0)] + "</input><br/>";
                        div4 = div4 + "B:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='B'>" + data.LlpoList[l].Choices[(n * 4 + 1)] + "</input><br/>";
                        div4 = div4 + "C:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='C'>" + data.LlpoList[l].Choices[(n * 4 + 2)] + "</input><br/>";
                        div4 = div4 + "D:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='D'>" + data.LlpoList[l].Choices[(n * 4 + 3)] + "</input><br/>";
                        div4 = div4 + "正确答案：" + data.LlpoList[l].Info.AnswerValue[n] + "<br/>";
                        div4 = div4 + "</fieldset>";
                        $("#LlpoO" + l).append(div4);
                        if (data.LlpoList[l].Info.UserAnswer[n] == "A") {
                            $("input[name=LlpRadio" + l + "_" + n + "]").get(0).checked = true;
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "B") {
                            $("input[name=LlpRadio" + l + "_" + n + "]").get(1).checked = true;
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "C") {
                            $("input[name=LlpRadio" + l + "_" + n + "]").get(2).checked = true;
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "D") {
                            $("input[name=LlpRadio" + l + "_" + n + "]").get(3).checked = true;
                        }
                    }
                    Num = Num + data.LlpoList[l].Info.QuestionCount;
                }
            }

            //添加短文听力理解题型
            if (data.RlpoList.length > 0) {
                var Rlpo = 0;
                for (var p = 0; p < data.RlpoList.length; p++) {
                    $("#ComprehensionListen").append("<div id='Rlpo" + p + "'><div id='RlpoC" + p + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='RlpoO" + p + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    //$("#ComprehensionListen").append("<div id='Rlpo" + p + "'style='width: 100%'></div><br/>");
                    $("#RlpoC" + p).append("<fieldset style='width: auto;'><legend>第" + (p + 1) + "大题原文：</legend>" + data.RlpoList[p].Script + "</fieldset>");
                    if ($.browser.mozilla) {
                        $("#RlpoO" + p).append("<img src='../../Images/btn_playaudio.gif' onclick='RlpoS" + p + ".controls.play()'" + "/><br/>");
                        $("#RlpoO" + p).append("<object type='video/x-ms-wmv' id='RlpoS" + p + "'" + "data='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3'><param name='src' value='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3' /><param name='autostart' value='false' /><param name='controller' value='true' /><embed id='RlpoS" + p + "'" + " src='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3' autostart='false' volume='100'></embed></object>");
                    }
                    else {
                        $("#RlpoO" + p).append("<img src='../../Images/btn_playaudio.gif' onclick='RlpoS" + p + ".play()'" + "/>");
                        $("#RlpoO" + p).append("<div style='display:none'><embed id='RlpoS" + p + "'" + " src='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3' autostart='false' volume='100'></embed></div>");
                    }
                    for (var p1 = 0; p1 < data.RlpoList[p].Info.QuestionCount; p1++) {
                        if (data.RlpoList[p].Info.AnswerValue[p1] != data.RlpoList[p].Info.UserAnswer[p1]) {
                            var div5 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + p1 + 1) + "小题：</legend>";
                        }
                        else {
                            var div5 = "<fieldset style='width: auto'><legend>第" + (Num + p1 + 1) + "小题：</legend>";
                        }
                        if ($.browser.mozilla) {
                            div5 = div5 + "<img src='../../Images/btn_playaudio.gif' onclick='RlpoS" + p + "_" + p1 + ".controls.play()'" + "/><br/>";
                            div5 = div5 + "<object type='video/x-ms-wmv' id='RlpoS" + p + "_" + p1 + "'" + "data='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3'>";
                            div5 = div5 + "<param name='src' value='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3' />";
                            div5 = div5 + "<param name='autostart' value='false' />";
                            div5 = div5 + "<param name='controller' value='true' />";
                            div5 = div5 + "<embed id='RlpoS" + p + "_" + p1 + "'" + " src='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3' autostart='false' volume='100'></embed>";
                            div5 = div5 + "</object>";
                        }
                        else {
                            div5 = div5 + "<img src='../../Images/btn_playaudio.gif' onclick='RlpoS" + p + "_" + p1 + ".play()'" + "/><br/>";
                            div5 = div5 + "<div style='display:none'><embed id='RlpoS" + p + "_" + p1 + "'" + " src='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3' autostart='false' volume='100'></embed></div>";
                        }
                        div5 = div5 + "A:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='A'>" + data.RlpoList[p].Choices[(p1 * 4 + 0)] + "</input><br/>";
                        div5 = div5 + "B:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='B'>" + data.RlpoList[p].Choices[(p1 * 4 + 1)] + "</input><br/>";
                        div5 = div5 + "C:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='C'>" + data.RlpoList[p].Choices[(p1 * 4 + 2)] + "</input><br/>";
                        div5 = div5 + "D:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='D'>" + data.RlpoList[p].Choices[(p1 * 4 + 3)] + "</input><br/>";
                        div5 = div5 + "正确答案：" + data.RlpoList[p].Info.AnswerValue[p1] + "<br/>";
                        div5 = div5 + "</fieldset>";
                        $("#RlpoO" + p).append(div5);
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "A") {
                            $("input[name=RlpRadio" + p + "_" + p1 + "]").get(0).checked = true;
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "B") {
                            $("input[name=RlpRadio" + p + "_" + p1 + "]").get(1).checked = true;
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "C") {
                            $("input[name=RlpRadio" + p + "_" + p1 + "]").get(2).checked = true;
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "D") {
                            $("input[name=RlpRadio" + p + "_" + p1 + "]").get(3).checked = true;
                        }
                    }
                    Num = Num + data.RlpoList[p].Info.QuestionCount;
                }
            }
            //添加复合型听力题型
            if (data.LpcList.length > 0) {
                var Lpc = 0;
                for (var t = 0; t < data.LpcList.length; t++) {
                    $("#ComplexListen").append("<div id='Lpc" + t + "'style='width: 100%'></div><br/>")

                    if ($.browser.mozilla) {
                        $("#Lpc" + t).append("<img src='../../Images/btn_playaudio.gif' onclick='LpcS" + t + ".controls.play()'" + "/><br/>");
                        $("#Lpc" + t).append("<object type='video/x-ms-wmv' id='LpcS" + t + "'" + "data='../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3'><param name='src' value='../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3' /><param name='autostart' value='false' /><param name='controller' value='true' /><embed id='LpcS" + t + "'" + " src='../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3' autostart='false' volume='100'></embed></object>");
                    }
                    else {
                        $("#Lpc" + t).append("<img src='../../Images/btn_playaudio.gif' onclick='LpcS" + t + ".play()'" + "/>");
                        $("#Lpc" + t).append("<div style='display:none'><embed id='LpcS" + t + "'" + " src=../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3  autostart='false' volume='100'></embed></div>");
                    }
                    $("#Lpc" + t).append("<div id='LpcC" + t + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='LpcO" + t + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div>");
                    $("#LpcC" + t).append("<fieldset style='width: 50%'><legend>第" + (t + 1) + "大题原文：</legend>" + data.LpcList[t].Script + "</fieldset>");

                    for (var t1 = 0; t1 < data.LpcList[t].Info.QuestionCount; t1++) {
                        if (data.LpcList[t].Info.UserAnswer[t1] != data.LpcList[t].Info.AnswerValue[t1]) {
                            var div6 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + t1 + 1) + "小题：</legend>";
                        }
                        else {
                            var div6 = "<fieldset style='width: auto'><legend>第" + (Num + t1 + 1) + "小题：</legend>";
                        }
                        div6 = div6 + "答案：" + data.LpcList[t].Info.UserAnswer[t1] + "<br/>";
                        div6 = div6 + "正确答案：" + data.LpcList[t].Info.AnswerValue[t1] + "<br/>";
                        div6 = div6 + "</fieldset>";
                        $("#LpcO" + t).append(div6);
                    }
                    Num = Num + data.LpcList[t].Info.QuestionCount;
                }
            }

            //添加阅读理解-选词填空题型
            if (data.RpcList.length > 0) {
                var Rpc = 0;
                for (var s = 0; s < data.RpcList.length; s++) {
                    $("#BankedCloze").append("<div id='Rpc" + s + "'><div id='RpcC" + s + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='RpcO" + s + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    $("#RpcC" + s).append("<fieldset style='width: auto'><legend>第" + (s + 1) + "大题原文：</legend>" + data.RpcList[s].Content + "</fieldset>");
                    $("#RpcO" + s).append("A:<lable>" + data.RpcList[s].WordList[0] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("B:<lable>" + data.RpcList[s].WordList[1] + "</lable><br/>");
                    $("#RpcO" + s).append("C:<lable>" + data.RpcList[s].WordList[2] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("D:<lable>" + data.RpcList[s].WordList[3] + "</lable><br/>");
                    $("#RpcO" + s).append("E:<lable>" + data.RpcList[s].WordList[4] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("F:<lable>" + data.RpcList[s].WordList[5] + "</lable><br/>");
                    $("#RpcO" + s).append("G:<lable>" + data.RpcList[s].WordList[6] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("H:<lable>" + data.RpcList[s].WordList[7] + "</lable><br/>");
                    $("#RpcO" + s).append("I:<lable>" + data.RpcList[s].WordList[8] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("J:<lable>" + data.RpcList[s].WordList[9] + "</lable><br/>");
                    $("#RpcO" + s).append("K:<lable>" + data.RpcList[s].WordList[10] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("L:<lable>" + data.RpcList[s].WordList[11] + "</lable><br/>");
                    $("#RpcO" + s).append("M:<lable>" + data.RpcList[s].WordList[12] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    $("#RpcO" + s).append("N:<lable>" + data.RpcList[s].WordList[13] + "</lable><br/>");
                    $("#RpcO" + s).append("O:<lable>" + data.RpcList[s].WordList[14] + "</lable>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    for (var s1 = 0; s1 < data.RpcList[s].Info.QuestionCount; s1++) {
                        if (data.RpcList[s].Info.UserAnswer[s1] != data.RpcList[s].Info.AnswerValue[s1]) {
                            var div7 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + s1 + 1) + "小题：</legend>";
                        }
                        else {
                            var div7 = "<fieldset style='width: auto'><legend>第" + (Num + s1 + 1) + "小题：</legend>";
                        }
                        div7 = div7 + "答案：" + data.RpcList[s].Info.UserAnswer[s1] + "<br/>";
                        div7 = div7 + "正确答案：" + data.RpcList[s].Info.AnswerValue[s1] + "<br/>";
                        div7 = div7 + "</fieldset>";
                        debugger;
                        $("#RpcO" + s).append(div7);
                    }
                    Num = Num + data.RpcList[s].Info.QuestionCount;
                }
            }

            //添加阅读理解-选择题型
            if (data.RpoList.length > 0) {
                var Rpo = 0;
                for (var d = 0; d < data.RpoList.length; d++) {
                    $("#MultipleChoice").append("<div id='Rpo" + d + "'><div id='RpoC" + d + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='RpoO" + d + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    $("#RpoC" + d).append("<fieldset style='width: auto'><legend>第" + (d + 1) + "大题原文：</legend>" + data.RpoList[d].Content + "</fieldset>");
                    for (var d1 = 0; d1 < data.RpoList[d].Info.QuestionCount; d1++) {
                        if (data.RpoList[d].Info.AnswerValue[d1] != data.RpoList[d].Info.UserAnswer[d1]) {
                            var div8 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + d1 + 1) + "小题：</legend>";
                        }
                        else {
                            var div8 = "<fieldset style='width: auto'><legend>第" + (Num + d1 + 1) + "小题：</legend>";
                        }
                        div8 = div8 + data.RpoList[d].Info.Problem[d1] + "<br/>";
                        div8 = div8 + "A:<input id='Radio" + (d * 4 + 1) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='A'>" + data.RpoList[d].Choices[(d1 * 4 + 0)] + "</input><br/>";
                        div8 = div8 + "B:<input id='Radio" + (d * 4 + 2) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='B'>" + data.RpoList[d].Choices[(d1 * 4 + 1)] + "</input><br/>";
                        div8 = div8 + "C:<input id='Radio" + (d * 4 + 3) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='C'>" + data.RpoList[d].Choices[(d1 * 4 + 2)] + "</input><br/>";
                        div8 = div8 + "D:<input id='Radio" + (d * 4 + 4) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='D'>" + data.RpoList[d].Choices[(d1 * 4 + 3)] + "</input><br/>";
                        div8 = div8 + "正确答案：<input type='text' name='RpoAnswerValue" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.AnswerValue[d1] + "'/><br/>";
                        div8 = div8 + "</fieldset>";
                        $("#RpoO" + d).append(div8);
                        if (data.RpoList[d].Info.UserAnswer[d1] == "A") {
                            $("input[name=RpoRadio" + d + "_" + d1 + "]").get(0).checked = true;
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "B") {
                            $("input[name=RpoRadio" + d + "_" + d1 + "]").get(1).checked = true;
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "C") {
                            $("input[name=RpoRadio" + d + "_" + d1 + "]").get(2).checked = true;
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "D") {
                            $("input[name=RpoRadio" + d + "_" + d1 + "]").get(3).checked = true;
                        }
                    }
                    Num = Num + data.RpoList[d].Info.QuestionCount;
                }
            }

            //添加完型填空题型
            if (data.CpList.length > 0) {
                var Cp = 0;
                for (var f = 0; f < data.CpList.length; f++) {
                    $("#Cloze").append("<div id='Cp" + f + "'><div id='CpC" + f + "'" + " style='width: 50%; height: auto; overflow: auto;float: left'></div><div id='CpO" + f + "'" + "style='width: 50%; height: auto; overflow: auto;float: left'></div></div><br/>");
                    $("#CpC" + f).append("<fieldset style='width: auto'><legend>第" + (f + 1) + "大题完型填空原文：</legend>" + data.CpList[f].Content + "</fieldset>");
                    for (var f1 = 0; f1 < data.CpList[f].Info.QuestionCount; f1++) {
                        if (data.CpList[f].Info.AnswerValue[f1] != data.CpList[f].Info.UserAnswer[f1]) {
                            var div9 = "<fieldset style='width: auto;color:Red;'><legend>第" + (Num + f1 + 1) + "小题：</legend>";
                        }
                        else {
                            var div9 = "<fieldset style='width: auto'><legend>第" + (Num + f1 + 1) + "小题：</legend>";
                        }
                        div9 = div9 + "A:<input id='Radio" + (f * 4 + 1) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='A'>" + data.CpList[f].Choices[(f1 * 4 + 0)] + "</input><br/>";
                        div9 = div9 + "B:<input id='Radio" + (f * 4 + 2) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='B'>" + data.CpList[f].Choices[(f1 * 4 + 1)] + "</input><br/>";
                        div9 = div9 + "C:<input id='Radio" + (f * 4 + 3) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='C'>" + data.CpList[f].Choices[(f1 * 4 + 2)] + "</input><br/>";
                        div9 = div9 + "D:<input id='Radio" + (f * 4 + 4) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='D'>" + data.CpList[f].Choices[(f1 * 4 + 3)] + "</input><br/>";
                        div9 = div9 + "正确答案：" + data.CpList[f].Info.AnswerValue[f1] + "<br/>";
                        div9 = div9 + "</fieldset>";
                        $("#CpO" + f).append(div9);
                        if (data.CpList[f].Info.UserAnswer[f1] == "A") {
                            $("input[name=CpRadio" + f + "_" + f1 + "]").get(0).checked = true;
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "B") {
                            $("input[name=CpRadio" + f + "_" + f1 + "]").get(1).checked = true;
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "C") {
                            $("input[name=CpRadio" + f + "_" + f1 + "]").get(2).checked = true;
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "D") {
                            $("input[name=CpRadio" + f + "_" + f1 + "]").get(3).checked = true;
                        }
                    }
                    Num = Num + data.CpList[f].Info.QuestionCount;
                }
            }
        });

    });
</script>--%>
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
    <div style="display:none;"> <input type="text" id="TestID" value="<%:ViewData["TestID"] %>" /></div>
</body>
</html>
