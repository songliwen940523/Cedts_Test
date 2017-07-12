(function ($) {
    $(document).ready(function () {
        $("Btn").click(function () {
            window.location.href = '/AssignHomework/CreateByTypeFive';
        })
        var AssessmentID = $("#AssessmentItemID").val();
        $.post("/AssignHomework/GetAssessmentItem", { id: AssessmentID }, function (data) {
            var Num = 0;
            //导航控制
            var wrp = "<ul class='wrp'>";
            if (data.Sspc != null) {
                wrp += "<li id='partSS'>Skimming and Scanning</li>";
            }
            if (data.Slpo != null || data.Llpo != null || data.Rlpo != null || data.Lpc != null) {
                wrp += "<li id='partLC'>Listen Comprehension </li>";
            }
            if (data.Rpc != null || data.Rpo != null || data.InfMat != null) {
                wrp += "<li id='partRC'>Reading Comprehension(Reading In Depth)</li>";
            }
            if (data.Cp != null) {
                wrp += "<li id='partCL'>Cloze</li>";
            }
            wrp += "</ul>";
            $("#Navigation").append(wrp);
            //添加快速阅读题型
            if (data.ItemType == 1) {
                var direction = "<!--快速阅读--><div class='assessmentItems' id='SkimmingAndScanning' partid='partSS'><!--外部容器--></div>";
                $("#paperBody").append(direction);
                $("#SkimmingAndScanning").append("<div id='Sspc' class='assessmentItem skimmingAndScanning'><div id='SspcC' class='content'></div><div id='SspcO' class='questions'></div></div>");
                $("#SspcC").append(data.Sspc.Content);
                $("#SspcO").append("<div style='display:none'><input type='hidden' id='SspAssessmentItemID' name='SspAssessmentItemID' value='" + data.Sspc.Info.ItemID + "'/><input type='hidden' id='ChoiceNum' name='ChoiceNum' value='" + data.Sspc.ChoiceNum + "'/><input type='hidden' id='TermNum' name='TermNum' value='" + data.Sspc.TermNum + "'/></div>");
                for (var j = 0; j < data.Sspc.ChoiceNum; j++) {
                    var div1 = "<div class='question'>";
                    div1 = div1 + "<div class='qBody'><strong class='qNum'>" + (Num + j + 1) + "</strong>" + data.Sspc.Info.Problem[j] + "</div>";
                    div1 = div1 + "<ol class='qChoices'>";
                    div1 = div1 + "<li id='SspRadio" + j + "A'><label><input  type='radio' value='A'/>" + data.Sspc.Choices[(j * 4)] + "</label></li>";
                    div1 = div1 + "<li id='SspRadio" + j + "B'><label><input type='radio' value='B'/>" + data.Sspc.Choices[(j * 4 + 1)] + "</label></li>";
                    div1 = div1 + "<li id='SspRadio" + j + "C'><label><input type='radio' value='C'/>" + data.Sspc.Choices[(j * 4 + 2)] + "</label></li>";
                    if (data.Sspc.Choices[(j * 4 + 3)] == "") {
                    }
                    else {
                        div1 = div1 + "<li name='SspRadio" + j + "D'><label><input type='radio' value='D'></input>" + data.Sspc.Choices[(j * 4 + 3)] + "</label></li>";
                    }
                    div1 = div1 + "正确答案：" + data.Sspc.Info.AnswerValue[j];
                    div1 = div1 + "</ol></div>";
                    //div1 = div1 + "正确答案：" + data.SspcList[i].Info.AnswerValue[j] + "<br/>";
                    $("#SspcO").append(div1);
                    //$("#SspAnswerValue" + j).val(data.Sspc.Info.AnswerValue[j]);
                }
                for (var k = 0; k < data.Sspc.TermNum; k++) {
                    var div2 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + data.Sspc.ChoiceNum + k + 1) + "</strong>";
                    var questionline = data.Sspc.Info.Problem[(data.Sspc.ChoiceNum + k)];
                    var line = "<input type='text' id='Answer" + k + "'" + "name='SspAnswer" + k + "'" + "value=''/>";
                    questionline = questionline.replace(/__+/g, line);
                    div2 = div2 + questionline + "</br>";
                    div2 = div2 + "正确答案：" + data.Sspc.Info.AnswerValue[(data.Sspc.ChoiceNum + k)] + "</div></div>";
                    $("#SspcO").append(div2);
                    //$("#SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k)).val(data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)]);
                }
                Num = Num + data.Sspc.Info.QuestionCount;
                $("#SspcTotalNum").val(Num);
            }

            //添加短对话听力题型
            if (data.ItemType == 2) {
                var direction = "<!--短对话听力--><div class='assessmentItems' id='ShortConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                $("#ShortConversations").append("<div id='Slpo'class='assessmentItem shortConversation'></div>");
                var div3 = "<div class='questions'><div class='question'><div class='qBody'><strong class='qNum'>" + (Num + 1) + "</strong>";
                div3 = div3 + "<a class='playAudio' href='../../SoundFile/" + data.Slpo.SoundFile + ".mp3'>播放音频</a>";
                div3 = div3 + "</div>";
                div3 = div3 + "<ol class='qChoices'>"
                div3 = div3 + "<li id='SlpRadioA'><label><input type='radio'  value='A'/>" + data.Slpo.Choices[0] + "</label></li>";
                div3 = div3 + "<li id='SlpRadioB'><label><input type='radio'  value='B'/>" + data.Slpo.Choices[1] + "</label></li>";
                div3 = div3 + "<li id='SlpRadioC'><label><input type='radio'  value='C'/>" + data.Slpo.Choices[2] + "</label></li>";
                div3 = div3 + "<li id='SlpRadioD'><label><input type='radio' value='D'/>" + data.Slpo.Choices[3] + "</label></li>";
                div3 = div3 + "正确答案：" + data.Slpo.Info.AnswerValue;
                div3 = div3 + "</ol></div>";
                //div3 = div3 + "正确答案：" + data.SlpoList[m].Info.AnswerValue;
                $("#Slpo").append(div3);
                //$("#SlpAnswerValue" + m).val(data.SlpoList[m].Info.AnswerValue);
                Num = Num + data.Slpo.Info.QuestionCount;
            }

            //添加长对话听力题型
            if (data.ItemType == 3) {
                var Llpo = 0;
                var direction = "<div class='assessmentItems' id='LongConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                $("#LongConversations").append("<div id='Llpo'class='assessmentItem longConversation'><div id='LlpoC' class='content'></div><div id='LlpoO' class='questions'></div></div>");
                var content = "<div class='content'><p>Conversation :<a class='playAudio' href='../../SoundFile/" + data.Llpo.SoundFile + ".mp3'>播放音频</a></p></div>";
                $("#LlpoC").append(content);
                for (var n = 0; n < data.Llpo.Info.QuestionCount; n++) {
                    var div4 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + n + 1) + "</strong>";
                    div4 = div4 + "<a class='playAudio' href='../../SoundFile/" + data.Llpo.Info.questionSound[n] + ".mp3'>播放音频</a>";
                    div4 = div4 + "</div>";
                    div4 = div4 + "<ol class='qChoices'>";
                    div4 = div4 + "<li id='LlpRadio" + n + "A'><label><input type='radio' value='A'>" + data.Llpo.Choices[(n * 4 + 0)] + "</label></li>";
                    div4 = div4 + "<li id='LlpRadio" + n + "B'><label><input type='radio' value='B'>" + data.Llpo.Choices[(n * 4 + 1)] + "</label></li>";
                    div4 = div4 + "<li id='LlpRadio" + n + "C'><label><input type='radio' value='C'>" + data.Llpo.Choices[(n * 4 + 2)] + "</label></li>";
                    div4 = div4 + "<li id='LlpRadio" + n + "D'><label><input type='radio' value='D'>" + data.Llpo.Choices[(n * 4 + 3)] + "</label></li>";
                    div4 = div4 + "正确答案：" + data.Llpo.Info.AnswerValue[n];
                    div4 = div4 + "</ol></div>";
                    //div4 = div4 + "正确答案：" + data.LlpoList[l].Info.AnswerValue[n];
                    $("#LlpoO").append(div4);
                    //$("#LlpAnswerValue" + l + "_" + n).val(data.LlpoList[l].Info.AnswerValue[n]);

                }
                Num = Num + data.Llpo.Info.QuestionCount;
                Llpo += data.Llpo.Info.QuestionCount;
                $("#LlpoTotalNum").val(Llpo);
            }

            //添加短文听力理解题型
            if (data.ItemType == 4) {
                var direction = "<div class='assessmentItems' id='ComprehensionListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                var Rlpo = 0;
                $("#ComprehensionListen").append("<div id='Rlpo' class='assessmentItem comprehensionListen'><div id='RlpoC' class='content'></div><div id='RlpoO' class='questions'></div></div>");
                var content = "<div class='content'><br/><p>Passage :<a class='playAudio' href='../../SoundFile/" + data.Rlpo.SoundFile + ".mp3'>播放音频</a></p><br/></div>";
                $("#RlpoC").append(content);
                $("#Rlpo").append("<input type='hidden' name='RlpNum' value='" + data.Rlpo.Info.QuestionCount + "'/>");
                $("#Rlpo").append("<input type='hidden' name='RlpAssessmentItemID' value='" + data.Rlpo.Info.ItemID + "'/><br/>");
                for (var p1 = 0; p1 < data.Rlpo.Info.QuestionCount; p1++) {
                    var div5 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + p1 + 1) + "</strong>";
                    div5 = div5 + "<a class='playAudio' href='../../SoundFile/" + data.Rlpo.Info.questionSound[p1] + ".mp3'>播放音频</a>";
                    div5 = div5 + "</div>";
                    div5 = div5 + "<ol class='qChoices'>";
                    div5 = div5 + "<li id='RlpoRadio" + p1 + "'A><label><input type='radio' value='A'>" + data.Rlpo.Choices[(p1 * 4 + 0)] + "</label></li>";
                    div5 = div5 + "<li id='RlpoRadio" + p1 + "'B><label><input type='radio' value='B'>" + data.Rlpo.Choices[(p1 * 4 + 1)] + "</label></li>";
                    div5 = div5 + "<li id='RlpoRadio" + p1 + "'C><label><input type='radio' value='C'>" + data.Rlpo.Choices[(p1 * 4 + 2)] + "</label></li>";
                    div5 = div5 + "<li id='RlpoRadio" + p1 + "'D><label><input type='radio' value='D'>" + data.Rlpo.Choices[(p1 * 4 + 3)] + "</label></li>";
                    div5 = div5 + "正确答案：" + data.Rlpo.Info.AnswerValue[p1];
                    div5 = div5 + "</ol></div>";
                    //div5 = div5 + "正确答案：" + data.RlpoList[p].Info.AnswerValue[p1];
                    $("#RlpoO").append(div5);
                    //$("#RlpAnswerValue" + p + "_" + p1).val(data.RlpoList[p].Info.AnswerValue[p1]);

                }
                Num = Num + data.Rlpo.Info.QuestionCount;
                Rlpo += data.Rlpo.Info.QuestionCount;
                $("#RlpoTotalNum").val(Rlpo);
            }
            //添加复合型听力题型
            if (data.ItemType == 5) {
                var Lpc = 0;
                var direction = "<div class='assessmentItems' id='ComplexListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                $("#ComplexListen").append("<div id='Lpc'class='assessmentItem complexListen'><div id='LpcC' class='content'></div><div id='LpcO' class='questions'></div></div>")
                var div = "";
                div += "<p><a class='playAudio' href='../../SoundFile/" + data.Lpc.SoundFile + ".mp3'>播放音频</a></p>";
                $("#LpcC").append(div);
                for (var i = 1; i < data.Lpc.Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";
                    var text = "<input type='text' name='LpcAnswer" + (i - 1) + "' value=''/><span style='color:red;'>(" + data.Lpc.Info.AnswerValue[(i - 1)] + ")</span>(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.Lpc.Script = data.Lpc.Script.replace(replace, text);
                }
                $("#LpcC").append(data.Lpc.Script);
                var div6 = "<div class='question'><div class='qBody'></div>";
                div6 = div6 + "</div>";
                $("#LpcO").append(div6);
                Num = Num + data.Lpc.Info.QuestionCount;
                Lpc += data.Lpc.Info.QuestionCount;
                $("#LpcTotalNum").val(Lpc);
            }

            //添加阅读理解-选词填空题型
            if (data.ItemType == 6) {
                var Rpc = 0;
                var direction = "<div class='assessmentItems' id='BankedCloze' partid='partRC'></div>";
                $("#paperBody").append(direction);
                $("#BankedCloze").append("<div id='Rpc'class='assessmentItem bankedCloze'><div id='RpcC' class='content'></div><div id='RpcO' class='choiceBank'></div></div>");
                for (var i = 1; i < data.Rpc.Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";
                    var text = "<input type='text' name='RpcAnswer" + (i - 1) + "' value=''/>(<span style='color:red'>" + data.Rpc.Info.AnswerValue[(i - 1)] + "</span>)(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.Rpc.Content = data.Rpc.Content.replace(replace, text);
                }
                $("#RpcC").append(data.Rpc.Content);
                var div7 = "<ol>";
                div7 = div7 + "<li>" + data.Rpc.WordList[0] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[1] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[2] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[3] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[4] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[5] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[6] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[7] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[8] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[9] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[10] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[11] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[12] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[13] + "</li>";
                div7 = div7 + "<li>" + data.Rpc.WordList[14] + "</li>";
                div7 = div7 + "</ol>";
                //div7 = div7 + "<input type='text' name='RpcNum' value='" + data.Rpc.Info.QuestionCount + "'style='display: none;'/>";
                $("#RpcO").append(div7);
                Num = Num + data.Rpc.Info.QuestionCount;
                Rpc += data.Rpc.Info.QuestionCount;
                $("#RpcTotalNum").val(Rpc);
            }

            //添加阅读理解-选择题型
            if (data.ItemType == 7) {
                var Rpo = 0;
                var direction = "<div class='assessmentItems' id='MultipleChoice' partid='partRC'></div>";
                $("#paperBody").append(direction);
                $("#MultipleChoice").append("<div id='Rpo' class='assessmentItem multipleChoice'" + "><div id='RpoC' class='content'></div><div id='RpoO' class='questions'></div></div>");
                $("#RpoO").append("<input type='hidden' name='RpoAssessmentItemID' value='" + data.Rpo.Info.ItemID + "'/><br/>");
                var content = "<br/><p>Passage</p><br/>" + data.Rpo.Content;
                $("#RpoC").append(content);
                for (var d1 = 0; d1 < data.Rpo.Info.QuestionCount; d1++) {
                    var div8 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + d1 + 1) + "</strong>" + data.Rpo.Info.Problem[d1] + "</div>";
                    div8 = div8 + "<ol class='qChoices'>";
                    div8 = div8 + "<li name='RpoRadio" + d1 + "A'><label><input type='radio'  value='A'>" + data.Rpo.Choices[(d1 * 4 + 0)] + "</label></li>";
                    div8 = div8 + "<li name='RpoRadio" + d1 + "B'><label><input type='radio'  value='B'>" + data.Rpo.Choices[(d1 * 4 + 1)] + "</label></li>";
                    div8 = div8 + "<li name='RpoRadio" + d1 + "C'><label><input type='radio'  value='C'>" + data.Rpo.Choices[(d1 * 4 + 2)] + "</label></li>";
                    div8 = div8 + "<li name='RpoRadio" + d1 + "D'><label><input type='radio'  value='D'>" + data.Rpo.Choices[(d1 * 4 + 3)] + "</label></li>";
                    div8 = div8 + "正确答案：" + data.Rpo.Info.AnswerValue[d1];
                    div8 = div8 + "</ol></div>"
                    //div8 = div8 + "正确答案：" + data.RpoList[d].Info.AnswerValue[d1];
                    $("#RpoO").append(div8);
                    //$("#RpoAnswerValue" + d + "_" + d1).val(data.RpoList[d].Info.AnswerValue[d1]);
                }
                Num = Num + data.Rpo.Info.QuestionCount;
                Rpo += data.Rpo.Info.QuestionCount;
                $("#RpoTotalNum").val(Rpo);
            }

            //添加阅读理解-信息匹配题型
            if (data.ItemType == 9) {
                var InfoMat = 0;
                var direction = "<div class='assessmentItems' id='InfoMatch' partid='partRC'></div>";
                $("#paperBody").append(direction);
                $("#InfoMatch").append("<div id='Imat'class='assessmentItem infoMatch'><div id='ImatC' class='content'></div><div id='ImatO' class='choiceBank'></div></div>");
                for (var i = 1; i < data.InfMat.Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";
                    var text = "<input type='text' name='ImatAnswer" + (i - 1) + "' value=''/>(<span style='color:red'>" + data.InfMat.Info.AnswerValue[(i - 1)] + "</span>)(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.InfMat.Content = data.InfMat.Content.replace(replace, text);
                }
                $("#ImatC").append(data.InfMat.Content);
                var div7 = "<ol>";
                div7 = div7 + "<li>" + data.InfMat.WordList[0] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[1] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[2] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[3] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[4] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[5] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[6] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[7] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[8] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[9] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[10] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[11] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[12] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[13] + "</li>";
                div7 = div7 + "<li>" + data.InfMat.WordList[14] + "</li>";
                div7 = div7 + "</ol>";
                //div7 = div7 + "<input type='text' name='RpcNum' value='" + data.Rpc.Info.QuestionCount + "'style='display: none;'/>";
                $("#ImatO").append(div7);
                Num = Num + data.InfMat.Info.QuestionCount;
                InfoMat += data.InfMat.Info.QuestionCount;
                $("#ImatTotalNum").val(InfoMat);
            }

            //添加完型填空题型
            if (data.ItemType == 8) {
                var Cp = 0;
                var direction = "<div class='assessmentItems' id='Cloze' partid='partCL'></div>";
                $("#paperBody").append(direction);
                $("#Cloze").append("<div id='Cp' class='assessmentItem cloze'><div id='CpC' class='content'></div><div id='CpO' class='questions'></div></div>");
                for (var i = 1; i < data.Cp.Info.QuestionCount + 1; i++) {
                    var replace = "(_" + i + "_)";
                    var text = "<span class='blank'>&nbsp;</span>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                    data.Cp.Content = data.Cp.Content.replace(replace, text);
                }
                $("#CpC").append(data.Cp.Content);
                for (var f1 = 0; f1 < data.Cp.Info.QuestionCount; f1++) {
                    var div9 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + f1 + 1) + "</strong></div>";
                    div9 = div9 + "<ol class='qChoices'>"
                    div9 = div9 + "<li name='CpRadio" + f1 + "A'><label><input type='radio'  value='A'>" + data.Cp.Choices[(f1 * 4 + 0)] + "</label></li>";
                    div9 = div9 + "<li name='CpRadio" + f1 + "B'><label><input type='radio'  value='B'>" + data.Cp.Choices[(f1 * 4 + 1)] + "</label></li>";
                    div9 = div9 + "<li name='CpRadio" + f1 + "C'><label><input type='radio'  value='C'>" + data.Cp.Choices[(f1 * 4 + 2)] + "</label></li>";
                    div9 = div9 + "<li name='CpRadio" + f1 + "D'><label><input type='radio'  value='D'>" + data.Cp.Choices[(f1 * 4 + 3)] + "</label></li>";
                    div9 = div9 + "正确答案：" + data.Cp.Info.AnswerValue[f1];
                    div9 = div9 + "</ol></div>"
                    //div9 = div9 + "正确答案：" + data.CpList[f].Info.AnswerValue[f1];
                    $("#CpO").append(div9);
                    //$("#CpAnswerValue" + f + "_" + f1).val(data.CpList[f].Info.AnswerValue[f1]);
                }
                Num = Num + data.Cp.Info.QuestionCount;
                Cp += data.Cp.Info.QuestionCount;
                $("#CpTotalNum").val(Cp);
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
            })($('.infoMatch'));

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