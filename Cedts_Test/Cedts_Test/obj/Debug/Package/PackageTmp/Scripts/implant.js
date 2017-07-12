var SysSecond;
var InterValObj;
var Closes;
var timer;
var Type;
var PaperID;
var LlpoArray = new Array();
var RlpoArray = new Array();
debugger;
    var myDate = new Date();
    var starttime = myDate.toLocaleTimeString();     //获取开始答卷时间
    $("#StartTime").val(starttime);
    $.post("/PaperShow/GetJosnData", null, function (data) {
        PaperID = data.PaperID;
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
                $("#SkimmingAndScanning").append("<div id='Sspc" + i + "' class='assessmentItem skimmingAndScanning'><div id='SspcC" + i + "'" + "class='content'></div><div id='SspcO" + i + "'" + "class='questions'></div></div>");
                $("#SspcC" + i).append(data.SspcList[i].Content);
                $("#SspcO" + i).append("<input type='hidden' id='SspAssessmentItemID" + i + "'" + "name='SspAssessmentItemID" + i + "'" + "value='" + data.SspcList[i].Info.ItemID + "'/><br/>");
                $("#SspcO" + i).append("<input type='hidden' id='ChoiceNum" + i + "'" + "name='ChoiceNum" + i + "'" + "value='" + data.SspcList[i].ChoiceNum + "'/><br/>");
                $("#SspcO" + i).append("<input type='hidden' id='TermNum" + i + "'" + "name='TermNum" + i + "'" + "value='" + data.SspcList[i].TermNum + "'/><br/>");
                for (var j = 0; j < data.SspcList[i].ChoiceNum; j++) {
                    var div1 = "<div class='question'>";
                    div1 = div1 + "<div class='qBody'><strong class='qNum'>" + (Num + j + 1) + "</strong>" + data.SspcList[i].Info.Problem[j] + "</div>";
                    div1 = div1 + "<ol class='qChoices'>";
                    div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='A'/>" + data.SspcList[i].Choices[(j * 4)] + "</label></li>";
                    div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='B'/>" + data.SspcList[i].Choices[(j * 4 + 1)] + "</label></li>";
                    div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='C'/>" + data.SspcList[i].Choices[(j * 4 + 2)] + "</label></li>";
                    if (data.SspcList[i].Choices[(j * 4 + 3)] == "") {
                    }
                    else {
                        div1 = div1 + "<li><span class='ABC'>D.</span><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='D'></input>" + data.SspcList[i].Choices[(j * 4 + 3)] + "</label><span class='mark'></span></li>";
                    }
                    div1 = div1 + "</ol></div>"
                    div1 = div1 + "<input type='hidden' name='SspAnswerValue" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.AnswerValue[j] + "'/>";
                    div1 = div1 + "<input type='hidden' name='SspQuestionID" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.QuestionID[j] + "'/>";
                    div1 = div1 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[j] + "'/>";
                    $("#SspcO" + i).append(div1);
                    $("#SspAnswerValue" + i + "_" + j).val(data.SspcList[i].Info.AnswerValue[j]);
                }
                for (var k = 0; k < data.SspcList[i].TermNum; k++) {
                    var div2 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "</strong>";
                    var questionline = data.SspcList[i].Info.Problem[(data.SspcList[i].ChoiceNum + k)];
                    var line = "<input type='text' id='Answer" + i + "_" + k + "'" + "name='SspAnswer" + i + "_" + k + "'/>";
                    questionline = questionline.replace("__", line);
                    div2 = div2 + questionline + "</div>.</div>";
                    div2 = div2 + "<input type='hidden' name='SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                    div2 = div2 + "<input type='hidden' name='SspQuestionID" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.QuestionID[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                    div2 = div2 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                    $("#SspcO" + i).append(div2);
                    $("#SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k)).val(data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)]);
                }
                Num = Num + data.SspcList[i].Info.QuestionCount;
                $("#SspcTotalNum").val(Num);
            }
        }

        //添加短对话听力题型
        if (data.SlpoList.length > 0) {
            for (var m = 0; m < data.SlpoList.length; m++) {
                $("#ShortConversations").append("<div id='Slpo" + m + "'class='assessmentItem shortConversation'" + "></div>");
                var div3 = "<div class='questions'><div class='question'><div class='qBody'><strong class='qNum'>" + (Num + 1) + "</strong>";
                if (Type == "大学四级试题" || Type == "大学六级试题") {
                }
                else {
                    div3 = div3 + "<a class='playAudio href='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3>播放音频</a>";
                }
                div3 = div3 + "</div>";
                div3 = div3 + "<ol class='qChoices'>"
                div3 = div3 + "<li><label><input type='radio'  value='A'/>" + data.SlpoList[m].Choices[0] + "</label></li>";
                div3 = div3 + "<li><label><input type='radio'  value='B'/>" + data.SlpoList[m].Choices[1] + "</label></li>";
                div3 = div3 + "<li><label><input type='radio'  value='C'/>" + data.SlpoList[m].Choices[2] + "</label></li>";
                div3 = div3 + "<li><label><input type='radio'  value='D'/>" + data.SlpoList[m].Choices[3] + "</label></li>";
                div3 = div3 + "</ol>";
                div3 = div3 + "</div></div>";
                div3 = div3 + "<input type='hidden' name='SlpAssessmentItemID" + m + "'" + "value='" + data.SlpoList[m].Info.ItemID + "'/>";
                div3 = div3 + "<input type='hidden' name='SlpAnswerValue" + m + "'" + "value='" + data.SlpoList[m].Info.AnswerValue + "'/>";
                div3 = div3 + "<input type='hidden' name='SlpQuestionID" + m + "'" + "value='" + data.SlpoList[m].Info.QuestionID + "'/>";
                div3 = div3 + "<input type='hidden' name='SlpScoreQuestion" + m + "'" + "value='" + data.SlpoList[m].Info.ScoreQuestion + "'/>";
                $("#Slpo" + m).append(div3);
                $("#SlpAnswerValue" + m).val(data.SlpoList[m].Info.AnswerValue);
                Num = Num + data.SlpoList[m].Info.QuestionCount;
                $("#SlpoTotalNum").val(data.SlpoList.length);
            }
        }

        //添加长对话听力题型
        if (data.LlpoList.length > 0) {
            var Llpo = 0;
            for (var l = 0; l < data.LlpoList.length; l++) {
                LlpoArray.push(data.LlpoList[l].Info.QuestionCount);
                $("#LongConversations").append("<div id='Llpo" + l + "'class='assessmentItem longConversation'" + "><div id='LlpoC" + l + "'" + "class='content'></div><div id='LlpoO" + l + "'" + "class='questions'></div></div>");
                $("#Llpo" + l).append("<input type='hidden' name='LlpAssessmentItemID" + l + "'" + "value='" + data.LlpoList[l].Info.ItemID + "'/>");
                $("#Llpo" + l).append("<input type='hidden' name='LlpNum" + l + "'" + "value='" + data.LlpoList[l].Info.QuestionCount + "'/>");
                if (Type == "大学四级试题" || Type == "大学六级试题")
                { }
                else {
                    var content = "<div class='content'><p>Conversation " + (l + 1) + "<a class='playAudio href='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3>播放音频</a></p></div>";
                    $("#LlpoC" + l).append(content);
                }

                for (var n = 0; n < data.LlpoList[l].Info.QuestionCount; n++) {
                    var div4 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + n + 1) + "</strong>";
                    if (Type == "大学四级试题" || Type == "大学六级试题")
                    { }
                    else {
                        div4 = div4 + "<a class='playAudio' href='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3'>播放音频</a>";
                    }
                    div4 = div4 + "</div>";
                    div4 = div4 + "<ol class='qChoices'>";
                    div4 = div4 + "<li><label><input type='radio'  value='A'>" + data.LlpoList[l].Choices[(n * 4 + 0)] + "</label></li>";
                    div4 = div4 + "<li><label><input type='radio'  value='B'>" + data.LlpoList[l].Choices[(n * 4 + 1)] + "</label></li>";
                    div4 = div4 + "<li><label><input type='radio'  value='C'>" + data.LlpoList[l].Choices[(n * 4 + 2)] + "</label></li>";
                    div4 = div4 + "<li><label><input type='radio'  value='D'>" + data.LlpoList[l].Choices[(n * 4 + 3)] + "</label></li>";
                    div4 = div4 + '</ol>';
                    div4 = div4 + "<input type='hidden' name='LlpAnswerValue" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.AnswerValue[n] + "'/>";
                    div4 = div4 + "<input type='hidden' name='LlpQuestionID" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.QuestionID[n] + "'/>";
                    div4 = div4 + "<input type='hidden' name='LlpScoreQuestion" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.ScoreQuestion[n] + "'/>";
                    div4 = div4 + "</div>";
                    $("#LlpoO" + l).append(div4);
                    $("#LlpAnswerValue" + l + "_" + n).val(data.LlpoList[l].Info.AnswerValue[n]);

                }
                Num = Num + data.LlpoList[l].Info.QuestionCount;
                Llpo += data.LlpoList[l].Info.QuestionCount;
                $("#LlpoTotalNum").val(Llpo);
            }
        }

        //添加短文听力理解题型
        if (data.RlpoList.length > 0) {
            var Rlpo = 0;
            for (var p = 0; p < data.RlpoList.length; p++) {
                RlpoArray.push(data.RlpoList[p].Info.QuestionCount);
                $("#ComprehensionListen").append("<div id='Rlpo" + p + "'class='assessmentItem comprehensionListen'" + "><div id='RlpoC" + p + "'" + "class='content'></div><div id='RlpoO" + p + "'" + "class='questions'></div></div>");
                if (Type == "大学四级试题" || Type == "大学六级试题") {
                }
                else {
                    var content = "<div class='content'><br/><p>Passage " + (p + 1) + ":<a class='playAudio href='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3>播放音频</a></p><br/></div>";
                    $("#RlpoC" + p).append(content);
                }
                $("#Rlpo" + p).append("<input type='hidden' name='RlpNum" + p + "'" + "value='" + data.RlpoList[p].Info.QuestionCount + "'/>");
                $("#Rlpo" + p).append("<input type='hidden' name='RlpAssessmentItemID" + p + "'" + "value='" + data.RlpoList[p].Info.ItemID + "'/><br/>");
                for (var p1 = 0; p1 < data.RlpoList[p].Info.QuestionCount; p1++) {
                    var div5 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + p1 + 1) + "</strong>";
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        div5 = div5 + "<a class='playAudio' href='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3'>播放音频</a>";
                    }
                    div5 = div5 + "</div>";
                    div5 = div5 + "<ol class='qChoices'>";
                    div5 = div5 + "<li><label><input type='radio'  value='A'>" + data.RlpoList[p].Choices[(p1 * 4 + 0)] + "</label></li>";
                    div5 = div5 + "<li><label><input type='radio'  value='B'>" + data.RlpoList[p].Choices[(p1 * 4 + 1)] + "</label></li>";
                    div5 = div5 + "<li><label><input type='radio'  value='C'>" + data.RlpoList[p].Choices[(p1 * 4 + 2)] + "</label></li>";
                    div5 = div5 + "<li><label><input type='radio'  value='D'>" + data.RlpoList[p].Choices[(p1 * 4 + 3)] + "</label></li>";
                    div5 = div5 + "</ol>";
                    div5 = div5 + "<input type='hidden' name='RlpAnswerValue" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.AnswerValue[p1] + "'/><br/>";
                    div5 = div5 + "<input type='hidden' name='RlpQuestionID" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.QuestionID[p1] + "'/><br/>";
                    div5 = div5 + "<input type='hidden' name='RlpScoreQuestion" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.ScoreQuestion[p1] + "'/><br/>";
                    div5 = div5 + "</div>";
                    $("#RlpoO" + p).append(div5);
                    $("#RlpAnswerValue" + p + "_" + p1).val(data.RlpoList[p].Info.AnswerValue[p1]);

                }
                Num = Num + data.RlpoList[p].Info.QuestionCount;
                Rlpo += data.RlpoList[p].Info.QuestionCount;
                $("#RlpoTotalNum").val(Rlpo);
            }
        }
        //添加复合型听力题型
        if (data.LpcList.length > 0) {
            var Lpc = 0;
            for (var t = 0; t < data.LpcList.length; t++) {
                $("#ComplexListen").append("<div id='Lpc" + t + "'class='assessmentItem complexListen'><div id='LpcC" + t + "'" + "class='content'></div><div id='LpcO" + t + "'" + "class='questions'></div></div>")
                var div = "";
                if (Type == "大学四级试题" || Type == "大学六级试题") {
                }
                else {
                    div += "<p><a class='playAudio' href='../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3'>播放音频</a></p>";
                }
                $("#LpcC" + t).append(div);
                for (var i = 1; i < data.LpcList[t].Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";

                    var text = "<input type='text' name='LpcAnswer" + t + "_" + (i - 1) + "'/>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.LpcList[t].Script = data.LpcList[t].Script.replace(replace, text);
                }
                $("#LpcC" + t).append(data.LpcList[t].Script);
                $("#Lpc" + t).append("<input type='hidden' name='LpcAssessmentItemID" + t + "'" + "value='" + data.LpcList[t].Info.ItemID + "'/><br/>");
                $("#LpcC" + t).append("<input type='hidden' name='LpcNum" + t + "'" + "value='" + data.LpcList[t].Info.QuestionCount + "'/>");
                for (var t1 = 0; t1 < data.LpcList[t].Info.QuestionCount; t1++) {
                    var div6 = "<div class='question'><div class='qBody'></div>"
                    div6 = div6 + "<input type='hidden' name='LpcAnswerValue" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.AnswerValue[t1] + "'/><br/>";
                    div6 = div6 + "<input type='hidden' name='LpcQuestionID" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.QuestionID[t1] + "'/><br/>";
                    div6 = div6 + "<input type='hidden' name='LpcScoreQuestion" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.ScoreQuestion[t1] + "'/><br/>";
                    div6 = div6 + "</div></div>";
                    $("#LpcO" + t).append(div6);
                    $("LpcAnswerValue" + t + "_" + t1).val(data.LpcList[t].Info.AnswerValue[t1]);
                }
                Num = Num + data.LpcList[t].Info.QuestionCount;
                Lpc += data.LpcList[t].Info.QuestionCount;
                $("#LpcTotalNum").val(Lpc);
            }
        }

        //添加阅读理解-选词填空题型
        if (data.RpcList.length > 0) {
            var Rpc = 0;
            for (var s = 0; s < data.RpcList.length; s++) {
                $("#BankedCloze").append("<div id='Rpc" + s + "'class='assessmentItem multipleChoice'" + "><div id='RpcC" + s + "'" + "class='content'></div><div id='RpcO" + s + "'" + "class='choiceBank'></div></div>");
                for (var i = 1; i < data.RpcList[s].Info.QuestionCount + 1; i++) {

                    var replace = "(_" + i + "_)";
                    var text = "<input type='text' name='RpcAnswer" + s + "_" + (i - 1) + "'/>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.RpcList[s].Content = data.RpcList[s].Content.replace(replace, text);
                }
                $("#RpcC" + s).append(data.RpcList[s].Content);
                var div7 = "<ol>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[0] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[1] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[2] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[3] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[4] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[5] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[6] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[7] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[8] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[9] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[10] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[11] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[12] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[13] + "</li>";
                div7 = div7 + "<li>" + data.RpcList[s].WordList[14] + "</li>";
                div7 = div7 + "</ol>";
                div7 = div7 + "<input type='text' name='RpcNum" + s + "'" + "value='" + data.RpcList[s].Info.QuestionCount + "'style='display: none;'/>";
                $("#RpcO" + s).append(div7);
                for (var s1 = 0; s1 < data.RpcList[s].Info.QuestionCount; s1++) {
                    var div = "<input type='hidden' name='RpcAnswerValue" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.AnswerValue[s1] + "'/><br/>";
                    div = div + "<input type='hidden' name='RpcQuestionID" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.QuestionID[s1] + "'/><br/>";
                    div = div + "<input type='hidden' name='RpcScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.ScoreQuestion[s1] + "'/><br/>";
                    div = div + "";
                    $("#RpcO" + s).append(div);
                    $("#RpcAnswerValue" + s + "_" + s1).val(data.RpcList[s].Info.AnswerValue[s1]);
                }
                Num = Num + data.RpcList[s].Info.QuestionCount;
                Rpc += data.RpcList[s].Info.QuestionCount;
                $("#RpcTotalNum").val(Rpc);
            }
        }

        //添加阅读理解-选择题型
        if (data.RpoList.length > 0) {
            var Rpo = 0;
            for (var d = 0; d < data.RpoList.length; d++) {
                $("#MultipleChoice").append("<div id='Rpo" + d + "'class='assessmentItem multipleChoice'" + "><div id='RpoC" + d + "'" + "class='content'></div><div id='RpoO" + d + "'" + "class='questions'></div></div>");
                $("#RpoO" + d).append("<input type='hidden' name='RpoAssessmentItemID" + d + "'" + "value='" + data.RpoList[d].Info.ItemID + "'/><br/>");
                var content = "<br/><p>Passage " + (d + 1) + "</p><br/>" + data.RpoList[d].Content;
                $("#RpoO" + d).append("<input type='hidden' name='RpoNum" + d + "'" + "value='" + data.RpoList[d].Info.QuestionCount + "'/>");
                $("#RpoC" + d).append(content);
                for (var d1 = 0; d1 < data.RpoList[d].Info.QuestionCount; d1++) {
                    var div8 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + d1 + 1) + "</strong>" + data.RpoList[d].Info.Problem[d1] + "</div>";
                    div8 = div8 + "<ol class='qChoices'>";
                    div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='A'>" + data.RpoList[d].Choices[(d1 * 4 + 0)] + "</label></li>";
                    div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='B'>" + data.RpoList[d].Choices[(d1 * 4 + 1)] + "</label></li>";
                    div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='C'>" + data.RpoList[d].Choices[(d1 * 4 + 2)] + "</label></li>";
                    div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='D'>" + data.RpoList[d].Choices[(d1 * 4 + 3)] + "</label></li>";
                    div8 = div8 + "</ol>"
                    div8 = div8 + "<input type='hidden' name='RpoAnswerValue" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.AnswerValue[d1] + "'/><br/>";
                    div8 = div8 + "<input type='hidden' name='RpoQuestionID" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.QuestionID[d1] + "'/><br/>";
                    div8 = div8 + "<input type='hidden' name='RpoScoreQuestion" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.ScoreQuestion[d1] + "'/><br/>";
                    div8 = div8 + "</div>";
                    $("#RpoO" + d).append(div8);
                    $("#RpoAnswerValue" + d + "_" + d1).val(data.RpoList[d].Info.AnswerValue[d1]);
                }
                Num = Num + data.RpoList[d].Info.QuestionCount;
                Rpo += data.RpoList[d].Info.QuestionCount;
                $("#RpoTotalNum").val(Rpo);
            }
        }

        //添加完型填空题型
        if (data.CpList.length > 0) {
            var Cp = 0;
            for (var f = 0; f < data.CpList.length; f++) {
                $("#Cloze").append("<div id='Cp" + f + "'class='assessmentItem cloze'><div id='CpC" + f + "'" + " class='content'></div><div id='CpO" + f + "'" + "class='questions'></div></div>");
                for (var i = 1; i < data.CpList[f].Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";
                    var text = "<span class='blank'>&nbsp;</span>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.CpList[f].Content = data.CpList[f].Content.replace(replace, text);
                }
                $("#CpC" + f).append(data.CpList[f].Content);
                $("#CpO" + f).append("<input type='hidden' name='CpAssessmentItemID" + f + "'" + "value='" + data.CpList[f].Info.ItemID + "'/><br/>");
                $("#CpO" + f).append("<input type='hidden' name='CpNum" + f + "'" + "value='" + data.CpList[f].Info.QuestionCount + "'/>");
                for (var f1 = 0; f1 < data.CpList[f].Info.QuestionCount; f1++) {
                    var div9 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + f1 + 1) + "</strong></div>";
                    div9 = div9 + "<ol class='qChoices'>"
                    div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='A'>" + data.CpList[f].Choices[(f1 * 4 + 0)] + "</label></li>";
                    div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='B'>" + data.CpList[f].Choices[(f1 * 4 + 1)] + "</label></li>";
                    div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='C'>" + data.CpList[f].Choices[(f1 * 4 + 2)] + "</label></li>";
                    div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='D'>" + data.CpList[f].Choices[(f1 * 4 + 3)] + "</label></li></ol>";
                    div9 = div9 + "<input type='hidden' name='CpAnswerValue" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.AnswerValue[f1] + "'/><br/>";
                    div9 = div9 + "<input type='hidden' name='CpQuestionID" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.QuestionID[f1] + "'/><br/>";
                    div9 = div9 + "<input type='hidden' name='CpScoreQuestion" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.ScoreQuestion[f1] + "'/><br/>";
                    div9 = div9 + "</div>";
                    $("#CpO" + f).append(div9);
                    $("#CpAnswerValue" + f + "_" + f1).val(data.CpList[f].Info.AnswerValue[f1]);
                }
                Num = Num + data.CpList[f].Info.QuestionCount;
                Cp += data.CpList[f].Info.QuestionCount;
                $("#CpTotalNum").val(Cp);
            }
        }
});

//倒计时
//将时间减去1秒，计算分、秒  
function SetRemainTime() {
    if (SysSecond > 0) {
        SysSecond = SysSecond - 1;
        var second = Math.floor(SysSecond % 60);             // 计算秒      
        var minite = Math.floor(SysSecond / 60);      //计算分  

        $("#remainTime").html(minite + "分" + second + "秒");
    } else {//剩余时间小于或等于0的时候，就停止间隔函数  
        window.clearInterval(InterValObj);
        //这里可以添加倒计时时间为0后需要执行的事件
        alert("考试时间结束！");
        $("#Submin").click();
    }
    if (SysSecond == 300) {
        alert('注意，离考试结束还有5分钟!');
    }
}