<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>评阅</title>
</head>
<script src="../../../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="../../../../ckeditor/ckeditor.js" type="text/javascript"></script>
<script type="text/javascript">

    $(function () {
        $.post("/Admin/Remark/GetDate", null, function (data) {
            $("#PaperID").val(data.PaperID);
            Type = data.Type;
            SysSecond = data.Duration * 60; //这里获取倒计时的起始时间  
            $("#PaperTitle").html(data.Title);
            $("#Sspc").val(data.SspcList.length);
            $("#Slpo").val(data.SlpoList.length);
            $("#Llpo").val(data.LlpoList.length);
            $("#Rlpo").val(data.RlpoList.length);
            $("#Lpc").val(data.LpcList.length);
            $("#Rpc").val(data.RpcList.length);
            $("#Rpo").val(data.RpoList.length);
            $("#Cp").val(data.CpList.length);
            var Num = 0;
            //添加快速阅读题型
            if (data.SspcList.length > 0) {

                for (var i = 0; i < data.SspcList.length; i++) {
                    $("#SkimmingAndScanning").append("<div id='Sspc" + i + "'style='display: none;'" + "><div id='SspcC" + i + "'" + " style='width: 400px; height: 600px; overflow: auto;float: left'></div><div id='SspcO" + i + "'" + "style='width: 400px; height: 600px; overflow: auto;float: left'></div></div>");
                    $("#SspcC" + i).append("<fieldset style='width: 350px'><legend>第" + (i + 1) + "大题原文：</legend>" + data.SspcList[i].Content + "</fieldset>");
                    $("#SspcO" + i).append("<input type='text' id='SspAssessmentItemID" + i + "'" + "name='SspAssessmentItemID" + i + "'" + "value='" + data.SspcList[i].Info.ItemID + "'style='display: none;'/><br/>");
                    $("#SspcO" + i).append("<input type='text' id='ChoiceNum" + i + "'" + "name='ChoiceNum" + i + "'" + "value='" + data.SspcList[i].ChoiceNum + "'style='display: none;'/><br/>");
                    $("#SspcO" + i).append("<input type='text' id='TermNum" + i + "'" + "name='TermNum" + i + "'" + "value='" + data.SspcList[i].TermNum + "'style='display: none;'/><br/>");
                    for (var j = 0; j < data.SspcList[i].ChoiceNum; j++) {
                        var div1 = "<fieldset style='width: 350px'><legend>第" + (Num + j + 1) + "小题：</legend>";
                        div1 = div1 + data.SspcList[i].Info.Problem[j] + "<br/>";
                        div1 = div1 + "A:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='A'></input>" + data.SspcList[i].Choices[(j * 4)] + "<br/>";
                        div1 = div1 + "B:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='B'></input>" + data.SspcList[i].Choices[(j * 4 + 1)] + "<br/>";
                        div1 = div1 + "C:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='C'></input>" + data.SspcList[i].Choices[(j * 4 + 2)] + "<br/>";
                        div1 = div1 + "D:<input id='Radio" + i + "_" + j + "'" + "name='SspRadio" + i + "_" + j + "'" + " type='radio' value='D'></input>" + data.SspcList[i].Choices[(j * 4 + 3)] + "<br/>";
                        div1 = div1 + "<span>正确答案：</span><input type='text' id='SspAnswerValue" + i + "_" + j + "'" + "  name='SspAnswerValue" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.AnswerValue[j] + "'/>";
                        div1 = div1 + "<input type='text' name='SspQuestionID" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.QuestionID[j] + "'style='display: none;'/>";
                        div1 = div1 + "<input type='text' name='SspScoreQuestion" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[j] + "'style='display: none;'/>";
                        div1 = div1 + "</fieldset>";
                        $("#SspcO" + i).append(div1);

                        $("input:[name=" + "SspRadio" + i + "_" + j + "]:radio").each(function () {
                            if ($(this).val() == data.SspcList[i].Info.UserAnswer[j]) {
                                $(this).attr("checked", "checked");
                            }
                        })
                        if (data.SspcList[i].Info.AnswerValue[j] != data.SspcList[i].Info.UserAnswer[j]) {
                            $("#SspAnswerValue" + i + "_" + j).css("color", "red");
                            $("#SspAnswerValue" + i + "_" + j).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }
                    }
                    for (var k = 0; k < data.SspcList[i].TermNum; k++) {
                        var div2 = "<fieldset style='width: 350px'><legend>第" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "小题：</legend>";
                        div2 = div2 + data.SspcList[i].Info.Problem[(data.SspcList[i].ChoiceNum + k)] + "<br/>";
                        div2 = div2 + "答案：<input type='text' id='Answer" + i + "_" + k + "'" + "name='SspAnswer" + i + "_" + k + "'" + "value='" + data.SspcList[i].Info.UserAnswer[(data.SspcList[i].ChoiceNum + k)] + "'/></br>";
                        div2 = div2 + "<span>正确答案：</span><input type='text' id='SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + " name='SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                        div2 = div2 + "<input type='text' name='SspQuestionID" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.QuestionID[(data.SspcList[i].ChoiceNum + k)] + "'style='display: none;'/>";
                        div2 = div2 + "<input type='text' name='SspScoreQuestion" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[(data.SspcList[i].ChoiceNum + k)] + "'style='display: none;'/>";
                        div2 = div2 + "</fieldset>";
                        $("#SspcO" + i).append(div2);
                        if (data.SspcList[i].Info.UserAnswer[(data.SspcList[i].ChoiceNum + k)] != data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)]) {
                            $("#SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k)).css("color", "red");
                            $("#SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k)).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }
                    }
                    Num = Num + data.SspcList[i].Info.QuestionCount;
                }
                $("#SspcTotalNum").val(Num);
            }

            //添加短对话听力题型
            if (data.SlpoList.length > 0) {

                for (var m = 0; m < data.SlpoList.length; m++) {
                    $("#ShortConversations").append("<div id='Slpo" + m + "'style='display: none;'" + "></div>");
                    var div3 = "<fieldset style='width: 400px'><legend>第" + (Num + 1) + "小题：</legend>";
                    div3 = div3 + "<img src='../../Images/btn_playaudio.gif' onclick='Short" + m + ".play()'" + "/><br/>";
                    div3 = div3 + "<div style='display:none'><embed id='Short" + m + "'" + " src='../../SoundFile/" + data.SlpoList[m].Info.ItemID + ".mp3' autostart='false' volume='100'></embed></div>";
                    div3 = div3 + "<input type='text' name='SlpAssessmentItemID" + m + "'" + "value='" + data.SlpoList[m].Info.ItemID + "'style='display: none;'/>";
                    div3 = div3 + "A:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='A'>" + data.SlpoList[m].Choices[0] + "</input><br/>";
                    div3 = div3 + "B:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='B'>" + data.SlpoList[m].Choices[1] + "</input><br/>";
                    div3 = div3 + "C:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='C'>" + data.SlpoList[m].Choices[2] + "</input><br/>";
                    div3 = div3 + "D:<input id='Radio2_" + m + "'" + "name='SlpRadio" + m + "'" + " type='radio'  value='D'>" + data.SlpoList[m].Choices[3] + "</input><br/>";
                    div3 = div3 + "<span>正确答案：</span><input type='text' id='SlpAnswerValue" + m + "'" + " name='SlpAnswerValue" + m + "'" + "value='" + data.SlpoList[m].Info.AnswerValue + "'/>";
                    div3 = div3 + "<input type='text' name='SlpQuestionID" + m + "'" + "value='" + data.SlpoList[m].Info.QuestionID + "'style='display: none;'/>";
                    div3 = div3 + "<input type='text' name='SlpScoreQuestion" + m + "'" + "value='" + data.SlpoList[m].Info.ScoreQuestion + "'style='display: none;'/>";
                    div3 = div3 + "</fieldset>";
                    $("#Slpo" + m).append(div3);

                    $("input:[name=" + "SlpRadio" + m + "]:radio").each(function () {
                        if ($(this).val() == data.SlpoList[m].Info.UserAnswer[0]) {
                            $(this).attr("checked", "checked");
                        }
                    })
                    if (data.SlpoList[m].Info.AnswerValue[0] != data.SlpoList[m].Info.UserAnswer[0]) {
                        $("#SlpAnswerValue" + m).css("color", "red");
                        $("#SlpAnswerValue" + m).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                    }
                    Num = Num + data.SlpoList[m].Info.QuestionCount;
                }
                $("#SlpoTotalNum").val(data.SlpoList.length);
            }

            //添加长对话听力题型
            if (data.LlpoList.length > 0) {
                var Llpo = 0;

                for (var l = 0; l < data.LlpoList.length; l++) {
                    $("#LongConversations").append("<div id='Llpo" + l + "'style='display: none;'" + "></div>");
                    $("#Llpo" + l).append("<img src='../../Images/btn_playaudio.gif' onclick='Long" + l + ".play()'" + "/>");
                    $("#Llpo" + l).append("<div style='display:none'><embed id='Long" + l + "'" + " src='../../SoundFile/" + data.LlpoList[l].Info.ItemID + ".mp3' autostart='false' volume='100'></embed></div>");
                    $("#Llpo" + l).append("<input type='text' name='LlpAssessmentItemID" + l + "'" + "value='" + data.LlpoList[l].Info.ItemID + "'style='display: none;'/>");
                    $("#Llpo" + l).append("<input type='text' name='LlpNum" + l + "'" + "value='" + data.LlpoList[l].Info.QuestionCount + "'style='display: none;'/>");
                    for (var n = 0; n < data.LlpoList[l].Info.QuestionCount; n++) {
                        var div4 = "<fieldset style='width: 400px'><legend>第" + (Num + n + 1) + "小题：</legend>";
                        div4 = div4 + "A:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='A'>" + data.LlpoList[l].Choices[(n * 4 + 0)] + "</input><br/>";
                        div4 = div4 + "B:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='B'>" + data.LlpoList[l].Choices[(n * 4 + 1)] + "</input><br/>";
                        div4 = div4 + "C:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='C'>" + data.LlpoList[l].Choices[(n * 4 + 2)] + "</input><br/>";
                        div4 = div4 + "D:<input id='Radio3_" + n + "'" + "name='LlpRadio" + l + "_" + n + "'" + " type='radio'  value='D'>" + data.LlpoList[l].Choices[(n * 4 + 3)] + "</input><br/>";
                        div4 = div4 + "<span>正确答案：</span><input type='text' id='LlpAnswerValue" + l + "_" + n + "'" + " name='LlpAnswerValue" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.AnswerValue[n] + "'/>";
                        div4 = div4 + "<input type='text' name='LlpQuestionID" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.QuestionID[n] + "'style='display: none;'/>";
                        div4 = div4 + "<input type='text' name='LlpScoreQuestion" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.ScoreQuestion[n] + "'style='display: none;'/>";
                        div4 = div4 + "</fieldset>";
                        $("#Llpo" + l).append(div4);

                        $("input:[name=" + "LlpRadio" + l + "_" + n + "]:radio").each(function () {
                            if ($(this).val() == data.LlpoList[l].Info.UserAnswer[n]) {
                                $(this).attr("checked", "checked");
                            }
                        })
                        if (data.LlpoList[l].Info.AnswerValue[n] != data.LlpoList[l].Info.UserAnswer[n]) {
                            $("#LlpAnswerValue" + l + "_" + n).css("color", "red");
                            $("#LlpAnswerValue" + l + "_" + n).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }
                    }
                    Num = Num + data.LlpoList[l].Info.QuestionCount;
                    Llpo += data.LlpoList[l].Info.QuestionCount;
                }
                $("#LlpoTotalNum").val(Llpo);
            }

            //添加短文听力理解题型
            if (data.RlpoList.length > 0) {
                var Rlpo = 0;

                for (var p = 0; p < data.RlpoList.length; p++) {
                    $("#ComprehensionListen").append("<div id='Rlpo" + p + "'style='display: none;'" + "></div>");
                    $("#Rlpo" + p).append("<img src='../../Images/btn_playaudio.gif' onclick='Rlpo" + p + ".play()'" + "/>");
                    $("#Rlpo" + p).append("<div style='display:none'><embed id='RlpoS" + p + "'" + " src='../../SoundFile/" + data.RlpoList[p].Info.ItemID + ".mp3' autostart='false' volume='100'></embed></div>");
                    $("#Rlpo" + p).append("<input type='text' name='RlpNum" + p + "'" + "value='" + data.RlpoList[p].Info.QuestionCount + "'style='display: none;'/>");
                    $("#Rlpo" + p).append("<input type='text' name='RlpAssessmentItemID" + p + "'" + "value='" + data.RlpoList[p].Info.ItemID + "'style='display: none;'/><br/>");
                    for (var p1 = 0; p1 < data.RlpoList[p].Info.QuestionCount; p1++) {
                        var div5 = "<fieldset style='width: 400px'><legend>第" + (Num + p1 + 1) + "小题：</legend>";
                        div5 = div5 + "A:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='A'>" + data.RlpoList[p].Choices[(p1 * 4 + 0)] + "</input><br/>";
                        div5 = div5 + "B:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='B'>" + data.RlpoList[p].Choices[(p1 * 4 + 1)] + "</input><br/>";
                        div5 = div5 + "C:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='C'>" + data.RlpoList[p].Choices[(p1 * 4 + 2)] + "</input><br/>";
                        div5 = div5 + "D:<input id='Radio4_" + p + "'" + "name='RlpRadio" + p + "_" + p1 + "'" + " type='radio'  value='D'>" + data.RlpoList[p].Choices[(p1 * 4 + 3)] + "</input><br/>";
                        div5 = div5 + "<span>正确答案：</span><input type='text' id='RlpAnswerValue" + p + "_" + p1 + "'" + " name='RlpAnswerValue" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.AnswerValue[p1] + "'/><br/>";
                        div5 = div5 + "<input type='text' name='RlpQuestionID" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.QuestionID[p1] + "'style='display: none;'/><br/>";
                        div5 = div5 + "<input type='text' name='RlpScoreQuestion" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.ScoreQuestion[p1] + "'style='display: none;'/><br/>";
                        div5 = div5 + "</fieldset>";
                        $("#Rlpo" + p).append(div5);

                        $("input:[name=" + "RlpRadio" + p + "_" + p1 + "]:radio").each(function () {
                            if ($(this).val() == data.RlpoList[p].Info.UserAnswer[p1]) {
                                $(this).attr("checked", "checked");
                            }
                        })
                        if (data.RlpoList[p].Info.AnswerValue[p1] != data.RlpoList[p].Info.UserAnswer[p1]) {
                            $("#RlpAnswerValue" + p + "_" + p1).css("color", "red");
                            $("#RlpAnswerValue" + p + "_" + p1).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }

                    }
                    Num = Num + data.RlpoList[p].Info.QuestionCount;
                    Rlpo += data.RlpoList[p].Info.QuestionCount;
                }
                $("#RlpoTotalNum").val(Rlpo);
            }

            //添加复合型听力题型
            if (data.LpcList.length > 0) {
                var Lpc = 0;
                for (var t = 0; t < data.LpcList.length; t++) {
                    var sound = "<img src='../../Images/btn_playaudio.gif' onclick='Lpc" + t + ".play()'" + "/><br/><div style='display:none'><embed id='LpcS" + t + "'" + " src=../../SoundFile/" + data.LpcList[t].Info.ItemID + ".mp3' autostart='false' volume='100'></embed></div>";
                    $("#ComplexListen").append("<div id='Lpc" + t + "'style='display: none;'" + ">" + sound + "<div id='LpcC" + t + "'" + " style='width: 400px; height: 600px; overflow: auto;float: left'></div><div id='LpcO" + t + "'" + "style='width: 400px; height: 600px; overflow: auto;float: left'></div></div>");
                    $("#LpcC" + t).append("<fieldset style='width: 350px'><legend>第" + (t + 1) + "大题原文：</legend>" + data.LpcList[t].Script + "</fieldset>");
                    $("#Lpc" + t).append("<input type='text' name='LpcAssessmentItemID" + t + "'" + "value='" + data.LpcList[t].Info.ItemID + "'style='display: none;'/><br/>");
                    $("#LpcC" + t).append("<input type='text' name='LpcNum" + t + "'" + "value='" + data.LpcList[t].Info.QuestionCount + "'style='display: none;'/>");
                    for (var t1 = 0; t1 < data.LpcList[t].Info.QuestionCount; t1++) {
                        var div6 = "<fieldset style='width: 350px'><legend>第" + (Num + t1 + 1) + "小题：</legend>";
                        div6 = div6 + "答案：<input type='text' id='Answer" + (t1 + 1) + "'" + "name='LpcAnswer" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.UserAnswer[t1] + "'/><br/>";
                        div6 = div6 + "<span>正确答案：</span><input type='text' id='LpcAnswerValue" + t + "_" + t1 + "'" + " name='LpcAnswerValue" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.AnswerValue[t1] + "'/><br/>";
                        div6 = div6 + "<input type='text' name='LpcQuestionID" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.QuestionID[t1] + "'style='display: none;'/><br/>";
                        div6 = div6 + "<input type='text' name='LpcScoreQuestion" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.ScoreQuestion[t1] + "'style='display: none;'/><br/>";
                        div6 = div6 + "</fieldset>";
                        $("#LpcO" + t).append(div6);

                        if (data.LpcList[t].Info.UserAnswer[t1] != data.LpcList[t].Info.AnswerValue[t1]) {
                            $("#LpcAnswerValue" + t + "_" + t1).css("color", "red");
                            $("#LpcAnswerValue" + t + "_" + t1).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }

                    }
                    Num = Num + data.LpcList[t].Info.QuestionCount;
                    Lpc += data.LpcList[t].Info.QuestionCount;
                }
                $("#LpcTotalNum").val(Lpc);
            }

            //添加阅读理解-选词填空题型
            if (data.RpcList.length > 0) {
                var Rpc = 0;
                for (var s = 0; s < data.RpcList.length; s++) {
                    $("#BankedCloze").append("<div id='Rpc" + s + "'style='display: none;'" + "><div id='RpcC" + s + "'" + " style='width: 400px; height: 600px; overflow: auto;float: left'></div><div id='RpcO" + s + "'" + "style='width: 400px; height: 600px; overflow: auto;float: left'></div></div>");
                    $("#RpcC" + s).append("<fieldset style='width: 350px'><legend>第" + (s + 1) + "大题原文：</legend>" + data.RpcList[s].Content + "</fieldset>");
                    $("#RpcO" + s).append("<input type='text' name='RpcAssessmentItemID" + s + "'" + "value='" + data.RpcList[s].Info.ItemID + "'style='display: none;'/><br/>");
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
                    $("#RpcO" + s).append("<input type='text' name='RpcNum" + s + "'" + "value='" + data.RpcList[s].Info.QuestionCount + "'style='display: none;'/>");
                    for (var s1 = 0; s1 < data.RpcList[s].Info.QuestionCount; s1++) {
                        var div7 = "<fieldset style='width: 350px'><legend>第" + (Num + s1 + 1) + "小题：</legend>";
                        div7 = div7 + "答案：<input type=text id='Answer" + s1 + "'" + "name='RpcAnswer" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.UserAnswer[s1] + "'/><br/>";
                        div7 = div7 + "<span>正确答案：</span><input type='text' id='RpcAnswerValue" + s + "_" + s1 + "'" + " name='RpcAnswerValue" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.AnswerValue[s1] + "'/><br/>";
                        div7 = div7 + "<input type='text' name='RpcQuestionID" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.QuestionID[s1] + "'style='display: none;'/><br/>";
                        div7 = div7 + "<input type='text' name='RpcScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.ScoreQuestion[s1] + "'style='display: none;'/><br/>";
                        div7 = div7 + "</fieldset>";
                        $("#RpcO" + s).append(div7);

                        if (data.RpcList[s].Info.UserAnswer[s1] != data.RpcList[s].Info.AnswerValue[s1]) {
                            $("#RpcAnswerValue" + s + "_" + s1).css("color", "red");
                            $("#RpcAnswerValue" + s + "_" + s1).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }

                    }
                    Num = Num + data.RpcList[s].Info.QuestionCount;
                    Rpc += data.RpcList[s].Info.QuestionCount;
                }
                $("#RpcTotalNum").val(Rpc);
            }

            //添加阅读理解-选择题型
            if (data.RpoList.length > 0) {
                var Rpo = 0;
                for (var d = 0; d < data.RpoList.length; d++) {
                    $("#MultipleChoice").append("<div id='Rpo" + d + "'style='display: none;'" + "><div id='RpoC" + d + "'" + " style='width: 400px; height: 600px; overflow: auto;float: left'></div><div id='RpoO" + d + "'" + "style='width: 400px; height: 600px; overflow: auto;float: left'></div></div>");
                    $("#RpoO" + d).append("<input type='text' name='RpoAssessmentItemID" + d + "'" + "value='" + data.RpoList[d].Info.ItemID + "'style='display: none;'/><br/>");
                    $("#RpoO" + d).append("<input type='text' name='RpoNum" + d + "'" + "value='" + data.RpoList[d].Info.QuestionCount + "'style='display: none;'/>");
                    $("#RpoC" + d).append("<fieldset style='width: 350px'><legend>第" + (d + 1) + "大题原文：</legend>" + data.RpoList[d].Content + "</fieldset>");
                    for (var d1 = 0; d1 < data.RpoList[d].Info.QuestionCount; d1++) {
                        var div8 = "<fieldset style='width: 350px'><legend>第" + (Num + d1 + 1) + "小题：</legend>";
                        div8 = div8 + data.RpoList[d].Info.Problem[d1] + "<br/>";
                        div8 = div8 + "A:<input id='Radio" + (d * 4 + 1) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='A'>" + data.RpoList[d].Choices[(d1 * 4 + 0)] + "</input><br/>";
                        div8 = div8 + "B:<input id='Radio" + (d * 4 + 2) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='B'>" + data.RpoList[d].Choices[(d1 * 4 + 1)] + "</input><br/>";
                        div8 = div8 + "C:<input id='Radio" + (d * 4 + 3) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='C'>" + data.RpoList[d].Choices[(d1 * 4 + 2)] + "</input><br/>";
                        div8 = div8 + "D:<input id='Radio" + (d * 4 + 4) + "'" + "name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='D'>" + data.RpoList[d].Choices[(d1 * 4 + 3)] + "</input><br/>";
                        div8 = div8 + "<span>正确答案：</span><input type='text' id='RpoAnswerValue" + d + "_" + d1 + "'" + " name='RpoAnswerValue" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.AnswerValue[d1] + "'/><br/>";
                        div8 = div8 + "<input type='text' name='RpoQuestionID" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.QuestionID[d1] + "'style='display: none;'/><br/>";
                        div8 = div8 + "<input type='text' name='RpoScoreQuestion" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.ScoreQuestion[d1] + "'style='display: none;'/><br/>";
                        div8 = div8 + "</fieldset>";
                        $("#RpoO" + d).append(div8);

                        $("input:[name=" + "RpoRadio" + d + "_" + d1 + "]:radio").each(function () {
                            if ($(this).val() == data.RpoList[d].Info.UserAnswer[d1]) {
                                $(this).attr("checked", "checked");
                            }
                        })
                        if (data.RpoList[d].Info.AnswerValue[d1] != data.RpoList[d].Info.UserAnswer[d1]) {
                            $("#RpoAnswerValue" + d + "_" + d1).css("color", "red");
                            $("#RpoAnswerValue" + d + "_" + d1).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }

                    }
                    Num = Num + data.RpoList[d].Info.QuestionCount;
                    Rpo += data.RpoList[d].Info.QuestionCount;
                }
                $("#RpoTotalNum").val(Rpo);
            }

            //添加完型填空题型
            if (data.CpList.length > 0) {
                var Cp = 0;
                for (var f = 0; f < data.CpList.length; f++) {
                    $("#Cloze").append("<div id='Cp" + f + "'style='display: none;'" + "><div id='CpC" + f + "'" + " style='width: 400px; height: 600px; overflow: auto;float: left'></div><div id='CpO" + f + "'" + "style='width: 400px; height: 600px; overflow: auto;float: left'></div></div>");
                    $("#CpC" + f).append("<fieldset style='width: 350px'><legend>第" + (f + 1) + "大题完型填空原文：</legend>" + data.CpList[f].Content + "</fieldset>");
                    $("#CpO" + f).append("<input type='text' name='CpAssessmentItemID" + f + "'" + "value='" + data.CpList[f].Info.ItemID + "'style='display: none;'/><br/>");
                    $("#CpO" + f).append("<input type='text' name='CpNum" + f + "'" + "value='" + data.CpList[f].Info.QuestionCount + "'style='display: none;'/>");
                    for (var f1 = 0; f1 < data.CpList[f].Info.QuestionCount; f1++) {
                        var div9 = "<fieldset style='width: 350px'><legend>第" + (Num + f1 + 1) + "小题：</legend>";
                        div9 = div9 + "A:<input id='Radio" + (f * 4 + 1) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='A'>" + data.CpList[f].Choices[(f1 * 4 + 0)] + "</input><br/>";
                        div9 = div9 + "B:<input id='Radio" + (f * 4 + 2) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='B'>" + data.CpList[f].Choices[(f1 * 4 + 1)] + "</input><br/>";
                        div9 = div9 + "C:<input id='Radio" + (f * 4 + 3) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='C'>" + data.CpList[f].Choices[(f1 * 4 + 2)] + "</input><br/>";
                        div9 = div9 + "D:<input id='Radio" + (f * 4 + 4) + "'" + "name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='D'>" + data.CpList[f].Choices[(f1 * 4 + 3)] + "</input><br/>";
                        div9 = div9 + "<span>正确答案：</span><input type='text' id='CpAnswerValue" + f + "_" + f1 + "'" + " name='CpAnswerValue" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.AnswerValue[f1] + "'/><br/>";
                        div9 = div9 + "<input type='text' name='CpQuestionID" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.QuestionID[f1] + "'style='display: none;'/><br/>";
                        div9 = div9 + "<input type='text' name='CpScoreQuestion" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.ScoreQuestion[f1] + "'style='display: none;'/><br/>";
                        div9 = div9 + "</fieldset>";
                        $("#CpO" + f).append(div9);

                        $("input:[name=" + "CpRadio" + f + "_" + f1 + "]:radio").each(function () {
                            if ($(this).val() == data.CpList[f].Info.UserAnswer[f1]) {
                                $(this).attr("checked", "checked");
                            }
                        })
                        if (data.CpList[f].Info.AnswerValue[f1] != data.CpList[f].Info.UserAnswer[f1]) {
                            $("#CpAnswerValue" + f + "_" + f1).css("color", "red");
                            $("#CpAnswerValue" + f + "_" + f1).parent("fieldset").children("span").html("正确答案应该是：").css("color", "red");
                        }

                    }
                    Num = Num + data.CpList[f].Info.QuestionCount;
                    Cp += data.CpList[f].Info.QuestionCount;
                }
                $("#CpTotalNum").val(Cp);
            }
            //初次加载页面控制题型显示
            if (data.SspcList.length > 0) {
                $("#SkimmingAndScanning").show();
                $("#Sspc0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length > 0) {
                $("#ShortConversations").show();
                $("#Slpo0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length > 0) {
                $("#LongConversations").show();
                $("#Llpo0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length == 0 && data.RlpoList.length > 0) {
                $("#ComprehensionListen").show();
                $("#Rlpo0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length == 0 && data.RlpoList.length == 0 && data.LpcList.length > 0) {
                $("#ComplexListen").show();
                $("#Lpc0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length == 0 && data.RlpoList.length == 0 && data.LpcList.length == 0 && data.RpcList.length > 0) {
                $("#BankedCloze").show();
                $("#Rpc0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length == 0 && data.RlpoList.length == 0 && data.LpcList.length == 0 && data.RpcList.length == 0 && data.RpoList.length > 0) {
                $("#MultipleChoice").show();
                $("#Rpo0").show();
            }
            if (data.SspcList.length == 0 && data.SlpoList.length == 0 && data.LlpoList.length == 0 && data.RlpoList.length == 0 && data.LpcList.length == 0 && data.RpoList.length == 0 && data.RpcList.length == 0 && data.CpList.length > 0) {
                $("#Cloze").show();
                $("#Cp0").show();
            }
        });

    });
</script>
<script type="text/javascript">
    //题型展示控制
    $(function () {
        $("#SkimmingAndScanningNext").click(function () {
            for (var i = 0; i < parseInt($("#Sspc").val()); i++) {
                if ($("#Sspc" + (parseInt($("#Sspc").val()) - 1)).css("display") != "none") {
                    if ($("#Slpo").val() != "0") {
                        $("#ShortConversations").show();
                        $("#Slpo0").show();
                        $("#SkimmingAndScanning").hide();
                        break;
                    }
                    else {
                        if ($("#Llpo").val() != "0") {
                            $("#LongConversations").show();
                            $("#Llpo0").show();
                            $("#SkimmingAndScanning").hide();
                            break;
                        }
                        else {
                            if ($("#Rlpo").val() != "0") {
                                $("#ComprehensionListen").show();
                                $("#Rlpo0").show();
                                $("#SkimmingAndScanning").hide();
                                break;
                            }
                            else {
                                if ($("#Lpc").val() != "0") {
                                    $("#ComplexListen").show();
                                    $("#Lpc0").show();
                                    $("#SkimmingAndScanning").hide();
                                    break;
                                }
                                else {
                                    if ($("#Rpc").val() != "0") {
                                        $("#BankedCloze").show();
                                        $("#Rpc0").show();
                                        $("#SkimmingAndScanning").hide();
                                        break;
                                    }
                                    else {
                                        if ($("#Rpo").val() != "0") {
                                            $("#MultipleChoice").show();
                                            $("#Rpo0").show();
                                            $("#SkimmingAndScanning").hide();
                                            break;
                                        }
                                        else {
                                            if ($("#Cp").val() != "0") {
                                                $("#Cloze").show();
                                                $("#Cp0").show();
                                                $("#SkimmingAndScanning").hide();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Sspc" + i).css("display") != "none") {
                        $("#Sspc" + (i + 1)).show();
                        $("#Sspc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#SkimmingAndScanningOn").click(function () {
            for (var i = 0; i < parseInt($("#Sspc").val()); i++) {
                if ($("#Sspc0").css("display") != "none") {
                    break;
                }
                else {
                    if ($("#Sspc" + i).css("display") != "none") {
                        $("#Sspc" + (i - 1)).show();
                        $("#Sspc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ShortConversationsNext").click(function () {
            for (var i = 0; i < parseInt($("#Slpo").val()); i++) {
                if ($("#Slpo" + (parseInt($("#Slpo").val()) - 1)).css("display") != "none") {
                    if ($("#Llpo").val() != "0") {
                        $("#LongConversations").show();
                        $("#Llpo0").show();
                        $("#ShortConversations").hide();
                        break;
                    }
                    else {
                        if ($("#Rlpo").val() != "0") {
                            $("#ComprehensionListen").show();
                            $("#Rlpo0").show();
                            $("#ShortConversations").hide();
                            break;
                        }
                        else {
                            if ($("#Lpc").val() != "0") {
                                $("#ComplexListen").show();
                                $("#Lpc0").show();
                                $("#ShortConversations").hide();
                                break;
                            }
                            else {
                                if ($("#Rpc").val() != "0") {
                                    $("#BankedCloze").show();
                                    $("#Rpc0").show();
                                    $("#ShortConversations").hide();
                                    break;
                                }
                                else {
                                    if ($("#Rpo").val() != "0") {
                                        $("#MultipleChoice").show();
                                        $("#Rpo0").show();
                                        $("#ShortConversations").hide();
                                        break;
                                    }
                                    else {
                                        if ($("#Cp").val() != "0") {
                                            $("#Cloze").show();
                                            $("#Cp0").show();
                                            $("#ShortConversations").hide();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Slpo" + i).css("display") != "none") {
                        $("#Slpo" + (i + 1)).show();
                        $("#Slpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ShortConversationsOn").click(function () {
            for (var i = 0; i < parseInt($("#Slpo").val()); i++) {
                if ($("#Slpo0").css("display") != "none") {
                    if ($("#Sspc").val() != "0") {
                        $("#SkimmingAndScanning").show();
                        $("#ShortConversations").hide();
                        break;
                    }
                    else {
                        break;
                    }
                }
                else {
                    if ($("#Slpo" + i).css("display") != "none") {
                        $("#Slpo" + (i - 1)).show();
                        $("#Slpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#LongConversationsNext").click(function () {
            for (var i = 0; i < parseInt($("#Llpo").val()); i++) {
                if ($("#Llpo" + (parseInt($("#Llpo").val()) - 1)).css("display") != "none") {
                    if ($("#Rlpo").val() != "0") {
                        $("#ComprehensionListen").show();
                        $("#Rlpo0").show();
                        $("#LongConversations").hide();
                        break;
                    }
                    else {
                        if ($("#Lpc").val() != "0") {
                            $("#ComplexListen").show();
                            $("#Lpc0").show();
                            $("#LongConversations").hide();
                            break;
                        }
                        else {
                            if ($("#Rpc").val() != "0") {
                                $("#BankedCloze").show();
                                $("#Rpc0").show();
                                $("#LongConversations").hide();
                                break;
                            }
                            else {
                                if ($("#Rpo").val() != "0") {
                                    $("#MultipleChoice").show();
                                    $("#Rpo0").show();
                                    $("#LongConversations").hide();
                                    break;
                                }
                                else {
                                    if ($("#Cp").val() != "0") {
                                        $("#Cloze").show();
                                        $("#Cp0").show();
                                        $("#LongConversations").hide();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Llpo" + i).css("display") != "none") {
                        $("#Llpo" + (i + 1)).show();
                        $("#Llpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#LongConversationsOn").click(function () {
            for (var i = 0; i < parseInt($("#Llpo").val()); i++) {
                if ($("#Llpo0").css("display") != "none") {
                    if ($("#Slpo").val() != "0") {
                        $("#ShortConversations").show();
                        $("#LongConversations").hide();
                        break;
                    }
                    else {
                        if ($("#Sspc").val() != "0") {
                            $("#SkimmingAndScanning").show();
                            $("#LongConversations").hide();
                            break;
                        }
                        else {
                            break;
                        }
                    }
                }
                else {
                    if ($("#Llpo" + i).css("display") != "none") {
                        $("#Llpo" + (i - 1)).show();
                        $("#Llpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ComprehensionListenNext").click(function () {
            for (var i = 0; i < parseInt($("#Rlpo").val()); i++) {
                if ($("#Rlpo" + (parseInt($("#Rlpo").val()) - 1)).css("display") != "none") {
                    if ($("#Lpc").val() != "0") {
                        $("#ComplexListen").show();
                        $("#Lpc0").show();
                        $("#ComprehensionListen").hide();
                        break;
                    }
                    else {
                        if ($("#Rpc").val() != "0") {
                            $("#BankedCloze").show();
                            $("#Rpc0").show();
                            $("#ComprehensionListen").hide();
                            break;
                        }
                        else {
                            if ($("#Rpo").val() != "0") {
                                $("#MultipleChoice").show();
                                $("#Rpo0").show();
                                $("#ComprehensionListen").hide();
                                break;
                            }
                            else {
                                if ($("#Cp").val() != "0") {
                                    $("#Cloze").show();
                                    $("#Cp0").show();
                                    $("#ComprehensionListen").hide();
                                    break;
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Rlpo" + i).css("display") != "none") {
                        $("#Rlpo" + (i + 1)).show();
                        $("#Rlpo" + i).hide();
                        break;
                    }
                }
            }
        })
        $("#ComprehensionListenOn").click(function () {
            for (var i = 0; i < parseInt($("#Rlpo").val()); i++) {
                if ($("#Rlpo0").css("display") != "none") {
                    if ($("#Llpo").val() != "0") {
                        $("#LongConversations").show();
                        $("#ComprehensionListen").hide();
                        break;
                    }
                    else {
                        if ($("#Slpo").val() != "0") {
                            $("#ShortConversations").show();
                            $("#ComprehensionListen").hide();
                            break;
                        }
                        else {
                            if ($("#Sspc").val() != "0") {
                                $("#SkimmingAndScanning").show();
                                $("#ComprehensionListen").hide();
                                break;
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
                else {
                    if ($("#Rlpo" + i).css("display") != "none") {
                        $("#Rlpo" + (i - 1)).show();
                        $("#Rlpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ComplexListenNext").click(function () {
            for (var i = 0; i < parseInt($("#Lpc").val()); i++) {
                if ($("#Lpc" + (parseInt($("#Lpc").val()) - 1)).css("display") != "none") {
                    if ($("#Rpc").val() != "0") {
                        $("#BankedCloze").show();
                        $("#Rpc0").show();
                        $("#ComplexListen").hide();
                        break;
                    }
                    else {
                        if ($("#Rpo").val() != "0") {
                            $("#MultipleChoice").show();
                            $("#Rpo0").show();
                            $("#ComplexListen").hide();
                            break;
                        }
                        else {
                            if ($("#Cp").val() != "0") {
                                $("#Cloze").show();
                                $("#Cp0").show();
                                $("#ComplexListen").hide();
                                break;
                            }
                        }
                    }
                }
                else {
                    if ($("#Lpc" + i).css("display") != "none") {
                        $("#Lpc" + (i + 1)).show();
                        $("#Lpc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ComplexListenOn").click(function () {
            for (var i = 0; i < parseInt($("#Lpc").val()); i++) {
                if ($("#Lpc0").css("display") != "none") {
                    if ($("#Rlpo").val() != "0") {
                        $("#ComprehensionListen").show();
                        $("#ComplexListen").hide();
                        break;
                    }
                    else {
                        if ($("#Llpo").val() != "0") {
                            $("#LongConversations").show();
                            $("#ComplexListen").hide();
                            break;
                        }
                        else {
                            if ($("#Slpo").val() != "0") {
                                $("#ShortConversations").show();
                                $("#ComplexListen").hide();
                                break;
                            }
                            else {
                                if ($("#Sspc").val() != "0") {
                                    $("#SkimmingAndScanning").show();
                                    $("#ComplexListen").hide();
                                    break;
                                }
                                else {
                                    break;
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Lpc" + i).css("display") != "none") {
                        $("#Lpc" + (i - 1)).show();
                        $("#Lpc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#BankedClozeNext").click(function () {
            for (var i = 0; i < parseInt($("#Rpc").val()); i++) {
                if ($("#Rpc" + (parseInt($("#Rpc").val()) - 1)).css("display") != "none") {
                    if ($("#Rpo").val() != "0") {
                        $("#MultipleChoice").show();
                        $("#Rpo0").show();
                        $("#BankedCloze").hide();
                        break;
                    }
                    else {
                        if ($("#Cp").val() != "0") {
                            $("#Cloze").show();
                            $("#Cp0").show();
                            $("#BankedCloze").hide();
                            break;
                        }
                    }
                }
                else {
                    if ($("#Rpc" + i).css("display") != "none") {
                        $("#Rpc" + (i + 1)).show();
                        $("#Rpc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#BankedClozeOn").click(function () {
            for (var i = 0; i < parseInt($("#Rpc").val()); i++) {
                if ($("#Rpc0").css("display") != "none") {
                    if ($("#Lpc").val() != "0") {
                        $("#ComplexListen").show();
                        $("#BankedCloze").hide();
                        break;
                    }
                    else {
                        if ($("#Rlpo").val() != "0") {
                            $("#ComprehensionListen").show();
                            $("#BankedCloze").hide();
                            break;
                        }
                        else {
                            if ($("#Llpo").val() != "0") {
                                $("#LongConversations").show();
                                $("#BankedCloze").hide();
                                break;
                            }
                            else {
                                if ($("#Slpo").val() != "0") {
                                    $("#ShortConversations").show();
                                    $("#BankedCloze").hide();
                                    break;
                                }
                                else {
                                    if ($("#Sspc").val() != "0") {
                                        $("#SkimmingAndScanning").show();
                                        $("#BankedCloze").hide();
                                        break;
                                    }
                                    else {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Rpc" + i).css("display") != "none") {
                        $("#Rpc" + (i - 1)).show();
                        $("#Rpc" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#MultipleChoiceNext").click(function () {
            for (var i = 0; i < parseInt($("#Rpo").val()); i++) {
                if ($("#Rpo" + (parseInt($("#Rpo").val()) - 1)).css("display") != "none") {
                    if ($("#Cp").val() != "0") {
                        $("#Cloze").show();
                        $("#Cp0").show();
                        $("#MultipleChoice").hide();
                        break;
                    }
                }
                else {
                    if ($("#Rpo" + i).css("display") != "none") {
                        $("#Rpo" + (i + 1)).show();
                        $("#Rpo" + i).hide();
                        break;
                    }
                }
            }
        })
        $("#MultipleChoiceOn").click(function () {
            for (var i = 0; i < parseInt($("#Rpo").val()); i++) {
                if ($("#Rpo0").css("display") != "none") {
                    if ($("#Rpc").val() != "0") {
                        $("#BankedCloze").show();
                        $("#MultipleChoice").hide();
                        break;
                    }
                    else {
                        if ($("#Lpc").val() != "0") {
                            $("#ComplexListen").show();
                            $("#MultipleChoice").hide();
                            break;
                        }
                        else {
                            if ($("#Rlpo").val() != "0") {
                                $("#ComprehensionListen").show();
                                $("#MultipleChoice").hide();
                                break;
                            }
                            else {
                                if ($("#Llpo").val() != "0") {
                                    $("#LongConversations").show();
                                    $("#MultipleChoice").hide();
                                    break;
                                }
                                else {
                                    if ($("#Slpo").val() != "0") {
                                        $("#ShortConversations").show();
                                        $("#MultipleChoice").hide();
                                        break;
                                    }
                                    else {
                                        if ($("#Sspc").val() != "0") {
                                            $("#SkimmingAndScanning").show();
                                            $("#MultipleChoice").hide();
                                            break;
                                        }
                                        else {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Rpo" + i).css("display") != "none") {
                        $("#Rpo" + (i - 1)).show();
                        $("#Rpo" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ClozeNext").click(function () {
            for (var i = 0; i < parseInt($("#Cp").val()); i++) {
                if ($("#Cp" + (parseInt($("#Cp").val()) - 1)).css("display") != "none") {
                    break;
                }
                else {
                    if ($("#Cp" + i).css("display") != "none") {
                        $("#Cp" + (i + 1)).show();
                        $("#Cp" + i).hide();
                        break;
                    }
                }
            }
        })

        $("#ClozeOn").click(function () {
            for (var i = 0; i < parseInt($("#Cp").val()); i++) {
                if ($("#Cp0").css("display") != "none") {
                    if ($("#Rpo").val() != "0") {
                        $("#MultipleChoice").show();
                        $("#Cloze").hide();
                        break;
                    }
                    if ($("#Rpc").val() != "0") {
                        $("#BankedCloze").show();
                        $("#Cloze").hide();
                        break;
                    }
                    else {
                        if ($("#Lpc").val() != "0") {
                            $("#ComplexListen").show();
                            $("#Cloze").hide();
                            break;
                        }
                        else {
                            if ($("#Rlpo").val() != "0") {
                                $("#ComprehensionListen").show();
                                $("#Cloze").hide();
                                break;
                            }
                            else {
                                if ($("#Llpo").val() != "0") {
                                    $("#LongConversations").show();
                                    $("#Cloze").hide();
                                    break;
                                }
                                else {
                                    if ($("#Slpo").val() != "0") {
                                        $("#ShortConversations").show();
                                        $("#Cloze").hide();
                                        break;
                                    }
                                    else {
                                        if ($("#Sspc").val() != "0") {
                                            $("#SkimmingAndScanning").show();
                                            $("#Cloze").hide();
                                            break;
                                        }
                                        else {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if ($("#Cp" + i).css("display") != "none") {
                        $("#Cp" + (i - 1)).show();
                        $("#Cp" + i).hide();
                        break;
                    }
                }
            }
        })
    })
        
</script>
<body>
    <%using (Html.BeginForm())
      { %>
    <%: Html.ValidationSummary(true)%>
    <div id="edit" style="margin-top: 20px">
        <center>
            <input type="button" value="书写评语" id="btn" />&nbsp;&nbsp;&nbsp;<input type="button"
                id="btnBack" value="返回" />
            <input type="hidden" name="testid" value="<%:ViewData["testID"] %>" />
            <input type="hidden" id="content" name="content" value="<%:TempData["content"] %>" />
        </center>
    </div>
    <div id="contentdiv" style="width: 805px; margin: auto; display: none;">
        <center>
            <textarea id="myEditor" name="myEditor" rows="4" cols="10"></textarea>
            <hr />
            <input type="submit" id="send" value="提  交" />&nbsp;&nbsp;&nbsp;
            <input type="button" id="close" value="取  消" />
        </center>
    </div>
    <br />
    <hr />
    <%} %>
    <script type="text/javascript">
        var editor = CKEDITOR.replace("myEditor");
        editor.setData($("#content").val());
        $("#btnBack").click(function () {
            window.location.href = "/Admin/Remark/Index";
        })

        $("#btn").click(function () {
            $("#contentdiv").show();
            $("#edit").hide();
        })
        $("#close").click(function () {
            $("#contentdiv").hide();
            $("#edit").show();
        })
        $("#send").click(function () {
            var content = editor.document.getBody().getText();
            if ($.trim(content) == "") {
                alert("评语不能为空！");
                return false;
            }
        })

    </script>
    <div style="width: 805px; margin: auto;">
        <input type="hidden" id="PaperID" name="PaperID" />
        <div>
            <center>
                <label id="PaperTitle">
                </label>
            </center>
        </div>
        <div id="SkimmingAndScanning" style="height: 800px; display: none;">
            <div>
                <div style="float: left; height: 36px; width: 700px;">
                    <input type="text" id="Sspc" name="Sspc" style="display: none;" />
                    <input type="text" id="SspcTotalNum" name="SspcTotalNum" style="display: none;" />
                </div>
                <div style="float: right; height: 36px; width: 100px;">
                    <a id="SkimmingAndScanningOn" href="#">上一题</a> <a id="SkimmingAndScanningNext" href="#">
                        下一题</a>
                </div>
            </div>
            <div>
                <h3>
                    Part II Reading Comprehension (Skimming and Scanning) (15 minutes) Directions: In
                    this part, you will have 15 minutes to go over the passage quickly and answer the
                    questions on Answer Sheet 1. For questions 1-7, choose the best answer from the
                    four choices marked A), B), C) and D). For questions 8-10, complete the sentences
                    with the information given in the passage.</h3>
            </div>
        </div>
        <div id="ShortConversations" style="height: 800px; display: none;">
            <div>
                <div style="float: left; height: 36px; width: 600px;">
                    <input type="text" id="Slpo" name="Slpo" style="display: none;" />
                    <input type="text" id="SlpoTotalNum" name="SlpoTotalNum" style="display: none;" />
                </div>
                <div style="float: right; height: 36px; width: 200px;">
                    <a id="ShortConversationsOn" href="#">上一题</a>&nbsp; <a id="ShortConversationsNext"
                        href="#">下一题</a></div>
            </div>
            <div>
                <h3>
                    In this section, you will hear 8 short conversations
                </h3>
            </div>
        </div>
        <div id="LongConversations" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 200px;">
                <input type="text" id="Llpo" name="Llpo" style="display: none;" />
                <input type="text" id="LlpoTotalNum" name="LlpoTotalNum" style="display: none;" />
            </div>
            <div id="LongSound" style="float: left; height: 36px; width: 400px;">
            </div>
            <div style="float: right; height: 36px; width: 200px;">
                <a id="LongConversationsOn" href="#">上一题</a>&nbsp; <a id="LongConversationsNext"
                    href="#">下一题</a></div>
            <div>
                <h3>
                    In this section, you will hear 2 long conversations
                </h3>
            </div>
        </div>
        <div id="ComprehensionListen" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 200px;">
                <input type="text" id="Rlpo" name="Rlpo" style="display: none;" />
                <input type="text" id="RlpoTotalNum" name="RlpoTotalNum" style="display: none;" />
            </div>
            <div id="ComprehensionSound" style="float: left; height: 36px; width: 400px;">
            </div>
            <div style="float: right; height: 36px; width: 200px;">
                <a id="ComprehensionListenOn" href="#">上一题</a>&nbsp; <a id="ComprehensionListenNext"
                    href="#">下一题</a>
            </div>
            <div>
                <h3>
                    In this section, you will hear 3 short passages .At the end of each passage . You
                    will hear some questions. Boss the passage and the questions will be spoken only
                    once. After you hear a question, you must choose the best answer from the four choices
                    marked A),B),C) and D). Then the corresponding letter on Answer Sheet 2 with a single
                    line through the centre.</h3>
            </div>
        </div>
        <div id="ComplexListen" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 200px;">
                <input type="text" id="Lpc" name="Lpc" style="display: none;" />
                <input type="text" id="LpcTotalNum" name="LpcTotalNum" style="display: none;" />
            </div>
            <div id="ComplexSound" style="float: left; height: 36px; width: 400px;">
            </div>
            <div style="float: right; height: 36px; width: 200px;">
                <a id="ComplexListenOn" href="#">上一题</a>&nbsp; <a id="ComplexListenNext" href="#">下一题</a></div>
            <div>
                <h3>
                    In this section , you will hear a passage three times . When the passage is read
                    for the first time, you should listen carefully for its general idea . When the
                    passage is read for the second time,
                </h3>
            </div>
        </div>
        <div id="BankedCloze" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 500px;">
                <input type="text" id="Rpc" name="Rpc" style="display: none;" />
                <input type="text" id="RpcTotalNum" name="RpcTotalNum" style="display: none;" />
            </div>
            <div style="float: right; height: 36px; width: 300px;">
                <a id="BankedClozeOn" href="#">上一题</a>&nbsp; <a id="BankedClozeNext" href="#">下一题</a></div>
            <div>
                <h3>
                    In this section, there is a passage with ten blanks. You are required to select
                    one word for each blank from a list of choices given in a word bank following the
                    passage. Read the passage through carefully before making your choices. Each choice
                    in bank is identified by a letter. Please mark the corresponding letter for each
                    item on Answer Sheet 2 with a single line through the centre. You may not use any
                    of the words in the bank more than once.</h3>
            </div>
        </div>
        <div id="MultipleChoice" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 500px;">
                <input type="text" id="Rpo" name="Rpo" style="display: none;" />
                <input type="text" id="RpoTotalNum" name="RpoTotalNum" style="display: none;" />
            </div>
            <div style="float: right; height: 36px; width: 300px;">
                <a id="MultipleChoiceOn" href="#">上一题</a>&nbsp; <a id="MultipleChoiceNext" href="#">
                    下一题</a></div>
            <div>
                <h3>
                    Directions: There are 2 passages in this section. Each passage is followed by some
                    questions or unfinished statements. For each of them there are four choices marked
                    A), B), C) and D). You should decide on the best choice and mark the corresponding
                    letter on Answer Sheet 2 with a single line through the centre.</h3>
            </div>
        </div>
        <div id="Cloze" style="height: 800px; display: none;">
            <div style="float: left; height: 36px; width: 500px;">
                <input type="text" id="Cp" name="Cp" style="display: none;" />
                <input type="text" id="CpTotalNum" name="CpTotalNum" style="display: none;" />
            </div>
            <div style="float: right; height: 36px; width: 300px;">
                <a id="ClozeOn" href="#">上一题</a> <a id="ClozeNext" href="#">下一题</a>
            </div>
            <div>
                <h3>
                    Directions: There are 20 blanks in the following passage. For each blank there are
                    four choices marked A), B), C) and D) on the right side of the paper. You should
                    choose the ONE that best fits into the passage. Then mark the corresponding letter
                    on Answer Sheet 2 with a single line through the centre.</h3>
            </div>
        </div>
    </div>
</body>
</html>
