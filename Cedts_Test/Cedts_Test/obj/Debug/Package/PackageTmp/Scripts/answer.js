/// <reference path="jquery-1.7-vsdoc.js" />

(function ($) {
    $(document).ready(function () {
        $("Btn").click(function () {
            window.location.href = '/PaperShow/UserTestInfo';
        })
        var TestID = $("#TestID").val();
        $.post("/PaperShow/GetPaper", { id: TestID }, function (data) {
            Type = data.Type;
            var Num = 0;
            var PaperID = data.PaperID;
            //导航控制
            var wrp = "<ul class='wrp'>";
            if (data.SspcList.length > 0) {
                wrp += "<li id='partSS'>Skimming and Scanning</li>";
            }
            if (data.SlpoList.length > 0 || data.LlpoList.length > 0 || data.RlpoList.length > 0 || data.LpcList.length > 0) {
                wrp += "<li id='partLC'>Listen Comprehension </li>";
            }
            if (data.RpcList.length > 0 || data.RpoList.length > 0 || data.InfMatList.length > 0) {
                wrp += "<li id='partRC'>Reading Comprehension(Reading In Depth)</li>";
            }
            if (data.CpList.length > 0) {
                wrp += "<li id='partCL'>Cloze</li>";
            }
            wrp += "</ul>";
            $("#Navigation").append(wrp);
            //添加快速阅读题型
            if (data.SspcList.length > 0) {
                var direction = "<!--快速阅读--><div class='assessmentItems' id='SkimmingAndScanning' partid='partSS'><!--外部容器--></div>";
                $("#paperBody").append(direction);
                for (var i = 0; i < data.SspcList.length; i++) {
                    $("#SkimmingAndScanning").append("<div id='Sspc" + i + "' class='assessmentItem skimmingAndScanning'><div id='SspcC" + i + "'" + "class='content'></div><div id='SspcO" + i + "'" + "class='questions'></div></div>");
                    $("#SspcC" + i).append(data.SspcList[i].Content);
                    $("#SspcO" + i).append("<div style='display:none'><input type='hidden' id='SspAssessmentItemID" + i + "'" + "name='SspAssessmentItemID" + i + "'" + "value='" + data.SspcList[i].Info.ItemID + "'/><input type='hidden' id='ChoiceNum" + i + "'" + "name='ChoiceNum" + i + "'" + "value='" + data.SspcList[i].ChoiceNum + "'/><input type='hidden' id='TermNum" + i + "'" + "name='TermNum" + i + "'" + "value='" + data.SspcList[i].TermNum + "'/></div>");
                    for (var j = 0; j < data.SspcList[i].ChoiceNum; j++) {
                        if (data.SspcList[i].Info.AnswerValue[j] != data.SspcList[i].Info.UserAnswer[j]) {
                            var div1 = "<div class='question' style='display:red;'>";
                        }
                        else {
                            var div1 = "<div class='question'>";
                        }
                        div1 = div1 + "<div class='qBody'><strong class='qNum'>" + (Num + j + 1) + "</strong>" + data.SspcList[i].Info.Problem[j] + "</div>";
                        div1 = div1 + "<ol class='qChoices'>";
                        div1 = div1 + "<li id='SspRadio" + i + "_" + j + "A'><label><input  type='radio' value='A'/>" + data.SspcList[i].Choices[(j * 4)] + "</label></li>";
                        div1 = div1 + "<li id='SspRadio" + i + "_" + j + "B'><label><input type='radio' value='B'/>" + data.SspcList[i].Choices[(j * 4 + 1)] + "</label></li>";
                        div1 = div1 + "<li id='SspRadio" + i + "_" + j + "C'><label><input type='radio' value='C'/>" + data.SspcList[i].Choices[(j * 4 + 2)] + "</label></li>";
                        if (data.SspcList[i].Choices[(j * 4 + 3)] == "") {
                        }
                        else {
                            div1 = div1 + "<li name='SspRadio" + i + "_" + j + "D'><label><input type='radio' value='D'></input>" + data.SspcList[i].Choices[(j * 4 + 3)] + "</label></li>";
                        }
                        div1 = div1 + "正确答案：" + data.SspcList[i].Info.AnswerValue[j];
                        div1 = div1 + "</ol></div>";
                        //div1 = div1 + "正确答案：" + data.SspcList[i].Info.AnswerValue[j] + "<br/>";
                        $("#SspcO" + i).append(div1);
                        if (data.SspcList[i].Info.UserAnswer[j] == "A") {
                            $("li[id=SspRadio" + i + "_" + j + "A]").addClass('chosen');
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "B") {
                            $("li[id=SspRadio" + i + "_" + j + "B]").addClass('chosen');
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "C") {
                            $("li[id=SspRadio" + i + "_" + j + "C]").addClass('chosen');
                        }
                        if (data.SspcList[i].Info.UserAnswer[j] == "D") {
                            $("li[id=SspRadio" + i + "_" + j + "D]").addClass('chosen');
                        }
                        $("#SspAnswerValue" + i + "_" + j).val(data.SspcList[i].Info.AnswerValue[j]);
                    }
                    for (var k = 0; k < data.SspcList[i].TermNum; k++) {
                        var div2 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "</strong>";
                        var questionline = data.SspcList[i].Info.Problem[(data.SspcList[i].ChoiceNum + k)];
                        var line = "<input type='text' id='Answer" + i + "_" + k + "'" + "name='SspAnswer" + i + "_" + k + "'" + "value='" + data.SspcList[i].Info.UserAnswer[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                        questionline = questionline.replace(/__+/g, line);
                        div2 = div2 + questionline + "</br>";
                        div2 = div2 + "正确答案：" + data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)] + "</div></div>";
                        $("#SspcO" + i).append(div2);
                        $("#SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k)).val(data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)]);
                    }
                    Num = Num + data.SspcList[i].Info.QuestionCount;
                    $("#SspcTotalNum").val(Num);
                }
            }

            //添加短对话听力题型
            if (data.SlpoList.length > 0) {
                var direction = "<!--短对话听力--><div class='assessmentItems' id='ShortConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var m = 0; m < data.SlpoList.length; m++) {
                    $("#ShortConversations").append("<div id='Slpo" + m + "'class='assessmentItem shortConversation'" + "></div>");
                    var div3 = "<div class='questions'><div class='question'><div class='qBody'><strong class='qNum'>" + (Num + 1) + "</strong>";
                    div3 = div3 + "<a class='playAudio' href='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3'>播放音频</a>";
                    div3 = div3 + "</div>";
                    div3 = div3 + "<ol class='qChoices'>"
                    div3 = div3 + "<li id='SlpRadio" + m + "A'><label><input type='radio'  value='A'/>" + data.SlpoList[m].Choices[0] + "</label></li>";
                    div3 = div3 + "<li id='SlpRadio" + m + "B'><label><input type='radio'  value='B'/>" + data.SlpoList[m].Choices[1] + "</label></li>";
                    div3 = div3 + "<li id='SlpRadio" + m + "C'><label><input type='radio'  value='C'/>" + data.SlpoList[m].Choices[2] + "</label></li>";
                    div3 = div3 + "<li id='SlpRadio" + m + "D'><label><input type='radio' value='D'/>" + data.SlpoList[m].Choices[3] + "</label></li>";
                    div3 = div3 + "正确答案：" + data.SlpoList[m].Info.AnswerValue;
                    div3 = div3 + "</ol></div>";
                    //div3 = div3 + "正确答案：" + data.SlpoList[m].Info.AnswerValue;
                    $("#Slpo" + m).append(div3);
                    if (data.SlpoList[m].Info.UserAnswer == "A") {
                        $("li[id=SlpRadio" + m + "A]").addClass('chosen');
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "B") {
                        $("li[id=SlpRadio" + m + "B]").addClass('chosen');
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "C") {
                        $("li[id=SlpRadio" + m + "C]").addClass('chosen');
                    }
                    if (data.SlpoList[m].Info.UserAnswer == "D") {
                        $("li[id=SlpRadio" + m + "D]").addClass('chosen');
                    }
                    $("#SlpAnswerValue" + m).val(data.SlpoList[m].Info.AnswerValue);
                    Num = Num + data.SlpoList[m].Info.QuestionCount;
                }
            }

            //添加长对话听力题型
            if (data.LlpoList.length > 0) {
                var Llpo = 0;
                var direction = "<div class='assessmentItems' id='LongConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var l = 0; l < data.LlpoList.length; l++) {
                    $("#LongConversations").append("<div id='Llpo" + l + "'class='assessmentItem longConversation'" + "><div id='LlpoC" + l + "'" + "class='content'></div><div id='LlpoO" + l + "'" + "class='questions'></div></div>");
                    var content = "<div class='content'><p>Conversation " + (l + 1) + "<a class='playAudio' href='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3'>播放音频</a></p></div>";
                    $("#LlpoC" + l).append(content);
                    for (var n = 0; n < data.LlpoList[l].Info.QuestionCount; n++) {
                        var div4 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + n + 1) + "</strong>";
                        div4 = div4 + "<a class='playAudio' href='../../SoundFile/" + data.LlpoList[l].Info.questionSound[n] + ".mp3'>播放音频</a>";
                        div4 = div4 + "</div>";
                        div4 = div4 + "<ol class='qChoices'>";
                        div4 = div4 + "<li id='LlpRadio" + l + "_" + n + "A'><label><input type='radio' value='A'>" + data.LlpoList[l].Choices[(n * 4 + 0)] + "</label></li>";
                        div4 = div4 + "<li id='LlpRadio" + l + "_" + n + "B'><label><input type='radio' value='B'>" + data.LlpoList[l].Choices[(n * 4 + 1)] + "</label></li>";
                        div4 = div4 + "<li id='LlpRadio" + l + "_" + n + "C'><label><input type='radio' value='C'>" + data.LlpoList[l].Choices[(n * 4 + 2)] + "</label></li>";
                        div4 = div4 + "<li id='LlpRadio" + l + "_" + n + "D'><label><input type='radio' value='D'>" + data.LlpoList[l].Choices[(n * 4 + 3)] + "</label></li>";
                        div4 = div4 + "正确答案：" + data.LlpoList[l].Info.AnswerValue[n];
                        div4 = div4 + "</ol></div>";
                        //div4 = div4 + "正确答案：" + data.LlpoList[l].Info.AnswerValue[n];
                        $("#LlpoO" + l).append(div4);
                        if (data.LlpoList[l].Info.UserAnswer[n] == "A") {
                            $("li[id=LlpRadio" + l + "_" + n + "A]").addClass('chosen');
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "B") {
                            $("li[id=LlpRadio" + l + "_" + n + "B]").addClass('chosen');
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "C") {
                            $("li[id=LlpRadio" + l + "_" + n + "C]").addClass('chosen');
                        }
                        if (data.LlpoList[l].Info.UserAnswer[n] == "D") {
                            $("li[id=LlpRadio" + l + "_" + n + "D]").addClass('chosen');
                        }
                        $("#LlpAnswerValue" + l + "_" + n).val(data.LlpoList[l].Info.AnswerValue[n]);

                    }
                    Num = Num + data.LlpoList[l].Info.QuestionCount;
                    Llpo += data.LlpoList[l].Info.QuestionCount;
                    $("#LlpoTotalNum").val(Llpo);
                }
            }

            //添加短文听力理解题型
            if (data.RlpoList.length > 0) {
                var direction = "<div class='assessmentItems' id='ComprehensionListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                var Rlpo = 0;
                for (var p = 0; p < data.RlpoList.length; p++) {
                    $("#ComprehensionListen").append("<div id='Rlpo" + p + "'class='assessmentItem comprehensionListen'" + "><div id='RlpoC" + p + "'" + "class='content'></div><div id='RlpoO" + p + "'" + "class='questions'></div></div>");
                    var content = "<div class='content'><br/><p>Passage " + (p + 1) + ":<a class='playAudio' href='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3'>播放音频</a></p><br/></div>";
                    $("#RlpoC" + p).append(content);
                    $("#Rlpo" + p).append("<input type='hidden' name='RlpNum" + p + "'" + "value='" + data.RlpoList[p].Info.QuestionCount + "'/>");
                    $("#Rlpo" + p).append("<input type='hidden' name='RlpAssessmentItemID" + p + "'" + "value='" + data.RlpoList[p].Info.ItemID + "'/><br/>");
                    for (var p1 = 0; p1 < data.RlpoList[p].Info.QuestionCount; p1++) {
                        var div5 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + p1 + 1) + "</strong>";
                        div5 = div5 + "<a class='playAudio' href='../../SoundFile/" + data.RlpoList[p].Info.questionSound[p1] + ".mp3'>播放音频</a>";
                        div5 = div5 + "</div>";
                        div5 = div5 + "<ol class='qChoices'>";
                        div5 = div5 + "<li id='RlpoRadio" + p + "_" + p1 + "'A><label><input type='radio' value='A'>" + data.RlpoList[p].Choices[(p1 * 4 + 0)] + "</label></li>";
                        div5 = div5 + "<li id='RlpoRadio" + p + "_" + p1 + "'B><label><input type='radio' value='B'>" + data.RlpoList[p].Choices[(p1 * 4 + 1)] + "</label></li>";
                        div5 = div5 + "<li id='RlpoRadio" + p + "_" + p1 + "'C><label><input type='radio' value='C'>" + data.RlpoList[p].Choices[(p1 * 4 + 2)] + "</label></li>";
                        div5 = div5 + "<li id='RlpoRadio" + p + "_" + p1 + "'D><label><input type='radio' value='D'>" + data.RlpoList[p].Choices[(p1 * 4 + 3)] + "</label></li>";
                        div5 = div5 + "正确答案：" + data.RlpoList[p].Info.AnswerValue[p1];
                        div5 = div5 + "</ol></div>";
                        //div5 = div5 + "正确答案：" + data.RlpoList[p].Info.AnswerValue[p1];
                        $("#RlpoO" + p).append(div5);
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "A") {
                            $("li[id=RlpoRadio" + l + "_" + n + "A]").addClass('chosen');
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "B") {
                            $("li[id=RlpoRadio" + l + "_" + n + "B]").addClass('chosen');
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "C") {
                            $("li[id=RlpoRadio" + l + "_" + n + "C]").addClass('chosen');
                        }
                        if (data.RlpoList[p].Info.UserAnswer[p1] == "D") {
                            $("li[id=RlpoRadio" + l + "_" + n + "D]").addClass('chosen');
                        }
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
                var direction = "<div class='assessmentItems' id='ComplexListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var t = 0; t < data.LpcList.length; t++) {
                    $("#ComplexListen").append("<div id='Lpc" + t + "'class='assessmentItem complexListen'><div id='LpcC" + t + "'" + "class='content'></div><div id='LpcO" + t + "'" + "class='questions'></div></div>")
                    var div = "";
                    div += "<p><a class='playAudio' href='../../SoundFile/" + data.LpcList[t].SoundFile + ".mp3'>播放音频</a></p>";
                    $("#LpcC" + t).append(div);
                    for (var i = 1; i < data.LpcList[t].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";
                        var text = "<input type='text' name='LpcAnswer" + t + "_" + (i - 1) + "' value='" + data.LpcList[t].Info.UserAnswer[(i - 1)] + "'/>(<span style='color:red;'>" + data.LpcList[t].Info.AnswerValue[(i - 1)] + ")</span>(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                        data.LpcList[t].Script = data.LpcList[t].Script.replace(replace, text);
                    }
                    $("#LpcC" + t).append(data.LpcList[t].Script);
                    var div6 = "<div class='question'><div class='qBody'></div>";
                    div6 = div6 + "</div>";
                    $("#LpcO" + t).append(div6);
                    Num = Num + data.LpcList[t].Info.QuestionCount;
                    Lpc += data.LpcList[t].Info.QuestionCount;
                    $("#LpcTotalNum").val(Lpc);
                }
            }

            //添加阅读理解-选词填空题型
            if (data.RpcList.length > 0) {
                var Rpc = 0;
                var direction = "<div class='assessmentItems' id='BankedCloze' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var s = 0; s < data.RpcList.length; s++) {
                    $("#BankedCloze").append("<div id='Rpc" + s + "'class='assessmentItem bankedCloze'" + "><div id='RpcC" + s + "'" + "class='content'></div><div id='RpcO" + s + "'" + "class='choiceBank'></div></div>");
                    for (var i = 1; i < data.RpcList[s].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";
                        var text = "<input type='text' name='RpcAnswer" + s + "_" + (i - 1) + "' value='" + data.RpcList[s].Info.UserAnswer[(i - 1)] + "'/>(<span style='color:red'>" + data.RpcList[s].Info.AnswerValue[(i - 1)] + "</span>)(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
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
                    Num = Num + data.RpcList[s].Info.QuestionCount;
                    Rpc += data.RpcList[s].Info.QuestionCount;
                    $("#RpcTotalNum").val(Rpc);
                }
            }

            //添加阅读理解-选择题型
            if (data.RpoList.length > 0) {
                var Rpo = 0;
                var direction = "<div class='assessmentItems' id='MultipleChoice' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var d = 0; d < data.RpoList.length; d++) {
                    $("#MultipleChoice").append("<div id='Rpo" + d + "'class='assessmentItem multipleChoice'" + "><div id='RpoC" + d + "'" + "class='content'></div><div id='RpoO" + d + "'" + "class='questions'></div></div>");
                    $("#RpoO" + d).append("<input type='hidden' name='RpoAssessmentItemID" + d + "'" + "value='" + data.RpoList[d].Info.ItemID + "'/><br/>");
                    var content = "<br/><p>Passage " + (d + 1) + "</p><br/>" + data.RpoList[d].Content;
                    $("#RpoC" + d).append(content);
                    for (var d1 = 0; d1 < data.RpoList[d].Info.QuestionCount; d1++) {
                        var div8 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + d1 + 1) + "</strong>" + data.RpoList[d].Info.Problem[d1] + "</div>";
                        div8 = div8 + "<ol class='qChoices'>";
                        div8 = div8 + "<li name='RpoRadio" + d + "_" + d1 + "A'><label><input type='radio'  value='A'>" + data.RpoList[d].Choices[(d1 * 4 + 0)] + "</label></li>";
                        div8 = div8 + "<li name='RpoRadio" + d + "_" + d1 + "B'><label><input type='radio'  value='B'>" + data.RpoList[d].Choices[(d1 * 4 + 1)] + "</label></li>";
                        div8 = div8 + "<li name='RpoRadio" + d + "_" + d1 + "C'><label><input type='radio'  value='C'>" + data.RpoList[d].Choices[(d1 * 4 + 2)] + "</label></li>";
                        div8 = div8 + "<li name='RpoRadio" + d + "_" + d1 + "D'><label><input type='radio'  value='D'>" + data.RpoList[d].Choices[(d1 * 4 + 3)] + "</label></li>";
                        div8 = div8 + "正确答案：" + data.RpoList[d].Info.AnswerValue[d1];
                        div8 = div8 + "</ol></div>"
                        //div8 = div8 + "正确答案：" + data.RpoList[d].Info.AnswerValue[d1];
                        $("#RpoO" + d).append(div8);
                        if (data.RpoList[d].Info.UserAnswer[d1] == "A") {
                            $("li[id=RpoRadio" + d + "_" + d1 + "A]").addClass('chosen');
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "B") {
                            $("li[id=RpoRadio" + d + "_" + d1 + "B]").addClass('chosen');
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "C") {
                            $("li[id=RpoRadio" + d + "_" + d1 + "C]").addClass('chosen');
                        }
                        if (data.RpoList[d].Info.UserAnswer[d1] == "D") {
                            $("li[id=RpoRadio" + d + "_" + d1 + "D]").addClass('chosen');
                        }
                        $("#RpoAnswerValue" + d + "_" + d1).val(data.RpoList[d].Info.AnswerValue[d1]);
                    }
                    Num = Num + data.RpoList[d].Info.QuestionCount;
                    Rpo += data.RpoList[d].Info.QuestionCount;
                    $("#RpoTotalNum").val(Rpo);
                }
            }

            //添加阅读理解-信息匹配题型
            if (data.InfMatList.length > 0) {
                var InfoMat = 0;
                var direction = "<div class='assessmentItems' id='InfoMatch' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var s = 0; s < data.InfMatList.length; s++) {
                    $("#InfoMatch").append("<div id='InfoMat" + s + "'class='assessmentItem infoMatch'" + "><div id='InfoMatC" + s + "'" + "class='content'></div><div id='InfoMatO" + s + "'" + "class='choiceBank'></div></div>");
                    for (var i = 1; i < data.InfMatList[s].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";
                        var text = "<input type='text' name='InfoMatAnswer" + s + "_" + (i - 1) + "' value='" + data.InfMatList[s].Info.UserAnswer[(i - 1)] + "'/>(<span style='color:red'>" + data.InfMatList[s].Info.AnswerValue[(i - 1)] + "</span>)(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                        data.InfMatList[s].Content = data.InfMatList[s].Content.replace(replace, text);
                    }
                    $("#InfoMatC" + s).append(data.InfMatList[s].Content);
                    var div7 = "<ol>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[0] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[1] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[2] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[3] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[4] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[5] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[6] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[7] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[8] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[9] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[10] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[11] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[12] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[13] + "</li>";
                    div7 = div7 + "<li>" + data.InfMatList[s].WordList[14] + "</li>";
                    div7 = div7 + "</ol>";
                    div7 = div7 + "<input type='text' name='InfoMatNum" + s + "'" + "value='" + data.InfMatList[s].Info.QuestionCount + "'style='display: none;'/>";
                    $("#InfoMatO" + s).append(div7);
                    Num = Num + data.InfMatList[s].Info.QuestionCount;
                    InfoMat += data.InfMatList[s].Info.QuestionCount;
                    $("#InfoMatTotalNum").val(InfoMat);
                }
            }

            //添加完型填空题型
            if (data.CpList.length > 0) {
                var Cp = 0;
                var direction = "<div class='assessmentItems' id='Cloze' partid='partCL'></div>";
                $("#paperBody").append(direction);
                for (var f = 0; f < data.CpList.length; f++) {
                    $("#Cloze").append("<div id='Cp" + f + "'class='assessmentItem cloze'><div id='CpC" + f + "'" + " class='content'></div><div id='CpO" + f + "'" + "class='questions'></div></div>");
                    for (var i = 1; i < data.CpList[f].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";
                        var text = "<span class='blank'>&nbsp;</span>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                        data.CpList[f].Content = data.CpList[f].Content.replace(replace, text);
                    }
                    $("#CpC" + f).append(data.CpList[f].Content);
                    for (var f1 = 0; f1 < data.CpList[f].Info.QuestionCount; f1++) {
                        var div9 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + f1 + 1) + "</strong></div>";
                        div9 = div9 + "<ol class='qChoices'>"
                        div9 = div9 + "<li name='CpRadio" + f + "_" + f1 + "A'><label><input type='radio'  value='A'>" + data.CpList[f].Choices[(f1 * 4 + 0)] + "</label></li>";
                        div9 = div9 + "<li name='CpRadio" + f + "_" + f1 + "B'><label><input type='radio'  value='B'>" + data.CpList[f].Choices[(f1 * 4 + 1)] + "</label></li>";
                        div9 = div9 + "<li name='CpRadio" + f + "_" + f1 + "C'><label><input type='radio'  value='C'>" + data.CpList[f].Choices[(f1 * 4 + 2)] + "</label></li>";
                        div9 = div9 + "<li name='CpRadio" + f + "_" + f1 + "D'><label><input type='radio'  value='D'>" + data.CpList[f].Choices[(f1 * 4 + 3)] + "</label></li>";
                        div9 = div9 + "正确答案：" + data.CpList[f].Info.AnswerValue[f1];
                        div9 = div9 + "</ol></div>"
                        //div9 = div9 + "正确答案：" + data.CpList[f].Info.AnswerValue[f1];
                        $("#CpO" + f).append(div9);
                        if (data.CpList[f].Info.UserAnswer[f1] == "A") {
                            $("li[id=CpRadio" + f + "_" + f1 + "A]").addClass('chosen');
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "B") {
                            $("li[id=CpRadio" + f + "_" + f1 + "B]").addClass('chosen');
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "C") {
                            $("li[id=CpRadio" + f + "_" + f1 + "C]").addClass('chosen');
                        }
                        if (data.CpList[f].Info.UserAnswer[f1] == "D") {
                            $("li[id=CpRadio" + f + "_" + f1 + "D]").addClass('chosen');
                        }
                        $("#CpAnswerValue" + f + "_" + f1).val(data.CpList[f].Info.AnswerValue[f1]);
                    }
                    Num = Num + data.CpList[f].Info.QuestionCount;
                    Cp += data.CpList[f].Info.QuestionCount;
                    $("#CpTotalNum").val(Cp);
                }
            }
            var $assessmentItems = $('.assessmentItems');
            var currentPart, currentItem, itemsCount = $assessmentItems.length;
            var $btnPrev = $('.btnPrev'), $btnNext = $('.btnNext'), $btnSubmit = $('.btnSubmit'), $btnQuit = $('.btnQuit');
            var parts = ['SkimmingAndScanning', 'Listen Comprehension', 'Reading Comprehension (Reading In Depth)', 'Cloze'];
            window.globalPlayer = null;
            window.isGlobalPlaying = false;
            swfobject.embedSWF("../../Images/globalplayer.swf", "GlobalPlayer", "1", "1", "10", "", {}, {}, {}, function (e) { globalPlayer = e.ref; });
            swfobject.embedSWF("../../Images/control.swf", "LcControl", "1", "1", "10");
            /*
            window.soundStop = function(){ //callback for GlobalPlayer on soundComplete
            isGlobalPlaying = false;
            $('.singleplayer').each(function(){
            this.enable();
            });
            };
            */

            (function ($paperNav) { //导航设置
                var $navContainer = $paperNav.parent();
                var indexMax = 99; //最高 z-index
                var navCurrentWidth = $paperNav.width(); //ul 宽度，960
                var navTotalWidth = $paperNav.get(0).scrollWidth; //ul内部宽度
                var $first = $paperNav.find('li:first');
                var leftOffset = 0;
                var navHeight = $navContainer.innerHeight();
                var topOffset = $navContainer.offset().top;

                $paperNav.find('li').each(function (i) {
                    $(this).css('z-index', indexMax - i); //调整li顺序
                });
                var leftMask = $('<span class="leftMask"/>').appendTo($paperNav);
                var rightMask = $('<span class="rightMask"/>').appendTo($paperNav);

                var repositionHeader = function () {
                    if ($(window).scrollTop() > topOffset) {
                        $navContainer.addClass('fixed');
                        $('.paperTitle').css({ 'margin-bottom': navHeight });
                    } else {
                        $navContainer.removeClass('fixed');
                        $('.paperTitle').css({ 'margin-bottom': 0 });
                    }
                }

                $('body').bind('partChangeEvent', function (e, pid) { //partChangeEvent handle
                    var $li = $paperNav.find('#' + pid);
                    if ($li.position().left + $li.innerWidth() > navCurrentWidth - 50) { //当前part右侧超出
                        var newleft = (navCurrentWidth - $li.innerWidth()) / 3 * 2 - $li.position().left;
                        leftOffset += newleft;
                        $first.animate({ 'margin-left': leftOffset }, 200);
                    }
                    if ($li.position().left < 50) { //左侧超出
                        var newleft = (navCurrentWidth - $li.innerWidth()) / 3 * 1 - $li.position().left;
                        leftOffset += newleft;
                        if (leftOffset > 0) leftOffset = 0;
                        $first.animate({ 'margin-left': leftOffset }, 200);
                    }
                    if ($li.is($first)) { //第一个part，似乎position().left有bug，取不到
                        $first.animate({ 'margin-left': 0 }, 200);
                    }
                    if (navTotalWidth + leftOffset > navCurrentWidth) {
                        rightMask.show();
                    } else {
                        rightMask.hide();
                    }
                    if (leftOffset == 0) {
                        leftMask.hide();
                    } else {
                        leftMask.show();
                    }
                    $paperNav.find('li').removeClass('current');
                    $li.addClass('current');
                    if ($li.attr('autostart') == '1') {
                        playAudio($li.attr('href'));
                        isGlobalPlaying = true;
                    }
                }).bind('pageSizeChange', function (e) { //pageSizeChange handle
                    navCurrentWidth = $paperNav.width();
                    navTotalWidth = $paperNav.get(0).scrollWidth;
                    repositionHeader();
                })


                $(window).scroll(repositionHeader);

            })($('.paperNav > ul'));


            (function ($qChoices) {//选择题设置
                if ($qChoices == null) return;
                var $inputs = $qChoices.find('input');
                var $li = $qChoices.find('li');
                $('<span class="mark"/>').appendTo($li);
                $qChoices.each(function () {
                    $(this).find('li').each(function (idx) {
                        var str = String.fromCharCode(65 + idx);
                        $('<span class="ABC">' + str + '.</span>p').prependTo($(this));
                    });
                })
                //                $li.bind('click', function (e) {
                //                    e.stopPropagation();
                //                    $(this).find('input').attr('checked', 'checked');
                //                    $(this).siblings().removeClass('chosen');
                //                    $(this).addClass('chosen');
                //                });
            })($('.qChoices'));

            var iniAudio = function ($btnAudio) { //音频播放按钮
                $btnAudio.each(function () {
                    var seed = String(new Date());
                    var p;
                    $(this).attr('id', seed);
                    if ($(this).attr('progressbar')) {
                        p = 1;
                    } else {
                        p = 0;
                    }
                    swfobject.embedSWF("../../Images/singleplayer.swf", seed, p == 1 ? "210" : "24", "24", "10", "", { soundFile: $(this).attr('href'), expand: p }, { wmode: 'opaque' }, { bgcolor: "#EEECDE", id: 'a' + Math.floor(Math.random() * 100), 'class': 'singleplayer' });
                });
            };

            (function ($list) {//双栏
                $.each($list, function (i, item) {
                    var $parent = $($list[i].container);
                    var selector = $list[i].inner;
                    $parent.each(function () {
                        $(this).find(selector).filter(':even').addClass('even');
                    });
                });
            })([{ container: '#ShortConversations', inner: '.shortConversation' },
			{ container: '.longConversation .questions', inner: '.question' },
			{ container: '.comprehensionListen .questions', inner: '.question' }
			]);

            (function ($banked) { //bankedCloze
                if ($banked == null) return;
                $banked.each(function () {
                    var seed = "uid" + Number(new Date()) + Math.round(Math.random() * 100000);
                    var $this = $(this);
                    var $items = $this.find('.choiceBank li');
                    $items.each(function (i) {
                        $(this).attr('index', String.fromCharCode(65 + i));
                    });
                    $this.find('input[type="text"]').each(function () {
                        var blank = $('<span class="blank">&nbsp;</span>').insertAfter($(this));
                        blank.data('inputRef', $(this));
                    });

                    $items.draggable({
                        helper: 'clone',
                        opacity: '0.8',
                        scope: seed
                    });
                    $this.find('.blank').droppable({
                        drop: function (event, ui) {
                            $(this).data('inputRef').val(ui.draggable.attr('index'));
                            $(this).text(ui.draggable.text());
                            $(this).addClass('dragDone');
                        },
                        hoverClass: "hover",
                        scope: seed
                    });
                });
            })($('.bankedCloze'));


            (function ($equal) { //等高题型的处理
                if ($equal == null) return;
                $equal.each(function () {
                    var $content = $(this).find('.content');
                    var $questions = $(this).find('.questions');
                    if ($questions.height() > $content.outerHeight() + 50) {
                        $questions.height($content.outerHeight());
                        $questions.css('overflow', 'auto');
                    }
                });
            })($('.cloze, .multipleChoice'));

            var gotoItem = function (id) {
                if (id == currentItem || id < 0 || id >= itemsCount) return;
                $assessmentItems.hide();
                $assessmentItems.eq(id).show();

                var pid = $assessmentItems.eq(id).attr('partid');
                if (pid != currentPart) {
                    $('body').trigger('partChangeEvent', pid);
                    currentPart = pid;
                };
                currentItem = id;
                if (currentItem == 0) { //第一题
                    $btnPrev.hide();
                    if (currentItem == itemsCount - 1) { //只有一题
                        //$btnSubmit.show();
                        $btnNext.hide();
                    } else {
                        $btnNext.show();
                        //$btnSubmit.hide();
                    }
                } else if (currentItem == itemsCount - 1) {//最后一题
                    $btnPrev.show();
                    $btnNext.hide();
                } else { //中间的题
                    $btnPrev.show();
                    $btnNext.show();
                }
                if (itemsCount == 1) {

                }
                if ($assessmentItems.eq(id).find('[autostart=1]').length > 0) {//播放directions，仅播放第一个，不允许有多个
                    playAudio($assessmentItems.eq(id).find('[autostart=1]').eq(0).attr('href'));
                    isGlobalPlaying = true;
                }

                iniAudio($assessmentItems.eq(id).find('.playAudio'));

                $('body').trigger('pageSizeChange');
            }

            var nextItem = function () { gotoItem(currentItem + 1); }
            var prevItem = function () { gotoItem(currentItem - 1); }
            var doSubmit = function () {
                if (confirm('是否提交？')) {
                    $("#Submin").click();
                }
            }

            var doQuit = function () {
                if (confirm('是否返回？')) {
                    window.history.back();
                   // window.location.href = '../../PaperShow/UserTestInfo';
                }
            }

            // button handles
            $btnNext.click(function (e) {
                e.preventDefault();
                nextItem();
            })
            $btnPrev.click(function (e) {
                e.preventDefault();
                prevItem();
            })
            $btnSubmit.click(function (e) {
                e.preventDefault();
                doSubmit();
            })
            $btnQuit.click(function () {
                doQuit();
            })

            gotoItem(0);
        });

        var playAudio = function (url, channel) {
            if (channel == undefined) channel = 1;
            if (url != null) {
                globalPlayer.loadSound(url, channel, 1);
            }
        }
    });
})(jQuery);