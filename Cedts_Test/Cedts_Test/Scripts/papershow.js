/// <reference path="jquery-1.7-vsdoc.js" />
/**
 * jQuery.ScrollTo - Easy element scrolling using jQuery.
 * Copyright (c) 2007-2009 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
 * Dual licensed under MIT and GPL.
 * Date: 5/25/2009
 * @author Ariel Flesler
 * @version 1.4.2
 *
 * http://flesler.blogspot.com/2007/10/jqueryscrollto.html
 */
;(function(d){var k=d.scrollTo=function(a,i,e){d(window).scrollTo(a,i,e)};k.defaults={axis:'xy',duration:parseFloat(d.fn.jquery)>=1.3?0:1};k.window=function(a){return d(window)._scrollable()};d.fn._scrollable=function(){return this.map(function(){var a=this,i=!a.nodeName||d.inArray(a.nodeName.toLowerCase(),['iframe','#document','html','body'])!=-1;if(!i)return a;var e=(a.contentWindow||a).document||a.ownerDocument||a;return d.browser.safari||e.compatMode=='BackCompat'?e.body:e.documentElement})};d.fn.scrollTo=function(n,j,b){if(typeof j=='object'){b=j;j=0}if(typeof b=='function')b={onAfter:b};if(n=='max')n=9e9;b=d.extend({},k.defaults,b);j=j||b.speed||b.duration;b.queue=b.queue&&b.axis.length>1;if(b.queue)j/=2;b.offset=p(b.offset);b.over=p(b.over);return this._scrollable().each(function(){var q=this,r=d(q),f=n,s,g={},u=r.is('html,body');switch(typeof f){case'number':case'string':if(/^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(f)){f=p(f);break}f=d(f,this);case'object':if(f.is||f.style)s=(f=d(f)).offset()}d.each(b.axis.split(''),function(a,i){var e=i=='x'?'Left':'Top',h=e.toLowerCase(),c='scroll'+e,l=q[c],m=k.max(q,i);if(s){g[c]=s[h]+(u?0:l-r.offset()[h]);if(b.margin){g[c]-=parseInt(f.css('margin'+e))||0;g[c]-=parseInt(f.css('border'+e+'Width'))||0}g[c]+=b.offset[h]||0;if(b.over[h])g[c]+=f[i=='x'?'width':'height']()*b.over[h]}else{var o=f[h];g[c]=o.slice&&o.slice(-1)=='%'?parseFloat(o)/100*m:o}if(/^\d+$/.test(g[c]))g[c]=g[c]<=0?0:Math.min(g[c],m);if(!a&&b.queue){if(l!=g[c])t(b.onAfterFirst);delete g[c]}});t(b.onAfter);function t(a){r.animate(g,j,b.easing,a&&function(){a.call(this,n,b)})}}).end()};k.max=function(a,i){var e=i=='x'?'Width':'Height',h='scroll'+e;if(!d(a).is('html,body'))return a[h]-d(a)[e.toLowerCase()]();var c='client'+e,l=a.ownerDocument.documentElement,m=a.ownerDocument.body;return Math.max(l[h],m[h])-Math.min(l[c],m[c])};function p(a){return typeof a=='object'?a:{top:a,left:a}}})(jQuery);

(function ($) {
    $(document).ready(function () {
        var SysSecond;
        var InterValObj;
        var Hours;
        var timer;
        var Type;
        var PaperID;
        var Sspctime = 0; //快速阅读计时
        var Slpotime = 0; //短对话计时
        var Llpotime = 0; //长对话
        var Rlpotime = 0; //短文听力理解
        var Lpctime = 0; //复合型听力
        var Rpctime = 0; //阅读理解-选词填空
        var Rpotime = 0; //阅读理解-选择题型
        var InfoMatime = 0;//阅读理解-信息匹配
        var Cptime = 0; //完型填空
        var LlpoArray = new Array();
        var RlpoArray = new Array();
        var myDate = new Date();
        var starttime = myDate.toLocaleTimeString();     //获取开始答卷时间
        $("#StartTime").val(starttime);

        $.post("/PaperShow/GetJosnData", null, function (data) {

        if(data.timeList.length>=1){
        $("#Continues").val(data.TestID);
        $("#temptime1").val(data.timeList[0]);
        $("#temptime2").val(data.timeList[1]);
        $("#temptime3").val(data.timeList[2]);
        $("#temptime4").val(data.timeList[3]);
        $("#temptime5").val(data.timeList[4]);
        $("#temptime6").val(data.timeList[5]);
        $("#temptime7").val(data.timeList[6]);
        $("#temptime8").val(data.timeList[7]);
        $("#temptime9").val(data.timeList[8]);
         PaperID = data.paperTotal.PaperID;
            $("#PaperID").val(data.paperTotal.PaperID);
            Type = data.paperTotal.Type;
            SysSecond = data.paperTotal.Duration * 60; //这里获取倒计时的起始时间  
            $(".paperTitle ").html('<h1>' + data.paperTotal.Title + '</h1>');
            $("#Sspc").val(data.paperTotal.SspcList.length);
            $("#Slpo").val(data.paperTotal.SlpoList.length);
            $("#Llpo").val(data.paperTotal.LlpoList.length);
            $("#Rlpo").val(data.paperTotal.RlpoList.length);
            $("#Lpc").val(data.paperTotal.LpcList.length);
            $("#Rpc").val(data.paperTotal.RpcList.length);
            $("#Rpo").val(data.paperTotal.RpoList.length);
            $("#Cp").val(data.paperTotal.CpList.length);
            $("#InfoMat").val(data.paperTotal.InfMatList.length);
            var Num = 0;
            //导航控制
            var wrp = "<ul class='wrp'>";
            if (data.paperTotal.SspcList.length > 0) {
                wrp += "<li id='partSS'>Skimming and Scanning</li>";
            }
            if (Type == "大学四级试题" || Type == "大学六级试题") {
                wrp += "<li id='partLC' href='../../SoundFile/" + PaperID + ".mp3' autostart='1'> Listen Comprehension</li>";
            }
            else {
                if (data.paperTotal.SlpoList.length > 0 || data.paperTotal.LlpoList.length > 0 || data.paperTotal.RlpoList.length > 0 || data.paperTotal.LpcList.length > 0) {
                    wrp += "<li id='partLC'>Listen Comprehension </li>";
                }
            }
            if (data.paperTotal.RpcList.length > 0 || data.paperTotal.RpoList.length > 0 || data.paperTotal.InfMatList.length > 0) {
                wrp += "<li id='partRC'>Reading Comprehension(Reading In Depth)</li>";
            }
            if (data.paperTotal.CpList.length > 0) {
                wrp += "<li id='partCL'>Cloze</li>";
            }
            wrp += "</ul>";
            $("#Navigation").append(wrp);
            //添加快速阅读题型
            if (data.paperTotal.SspcList.length > 0) {
                var direction = "<!--快速阅读--><div class='assessmentItems' id='SkimmingAndScanning' partid='partSS'><!--外部容器--></div>";
                $("#paperBody").append(direction);
                for (var i = 0; i < data.paperTotal.SspcList.length; i++) {
                    $("#SkimmingAndScanning").append("<div id='Sspc" + i + "' class='assessmentItem skimmingAndScanning'><div id='SspcC" + i + "'" + "class='content'></div><div id='SspcO" + i + "'" + "class='questions'></div></div><hr/>");
                    $("#SspcC" + i).append(data.paperTotal.SspcList[i].Content);
                    $("#SspcO" + i).append("<div style='display:none'><input type='hidden' id='SspAssessmentItemID" + i + "'" + "name='SspAssessmentItemID" + i + "'" + "value='" + data.paperTotal.SspcList[i].Info.ItemID + "'/><input type='hidden' id='ChoiceNum" + i + "'" + "name='ChoiceNum" + i + "'" + "value='" + data.paperTotal.SspcList[i].ChoiceNum + "'/><input type='hidden' id='TermNum" + i + "'" + "name='TermNum" + i + "'" + "value='" + data.paperTotal.SspcList[i].TermNum + "'/></div>");
                    for (var j = 0; j < data.paperTotal.SspcList[i].ChoiceNum; j++) {
                        var div1 = "<div class='question'>";
                        div1 = div1 + "<div class='qBody'><strong class='qNum'>" + (Num + j + 1) + "</strong>" + data.paperTotal.SspcList[i].Info.Problem[j] + "</div>";
                        div1 = div1 + "<ol class='qChoices'>";
                        if(data.userData[j]=="A")
                        {
                        div1 = div1 + "<li class='chosen'><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='A' checked='checked'/>" + data.paperTotal.SspcList[i].Choices[(j * 4)] + "</label></li>";
                        } 
                        else
                        {
                            div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='A'/>" + data.paperTotal.SspcList[i].Choices[(j * 4)] + "</label></li>";
                        }
                         if(data.userData[j]=="B")
                        {
                             div1 = div1 + "<li class='chosen'><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='B' checked='checked'/>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 1)] + "</label></li>";
                        }
                        else
                        {
                            div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='B'/>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 1)] + "</label></li>";
                        }
                        if(data.userData[j]=="C")
                        {
                            div1 = div1 + "<li class='chosen'><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='C' checked='checked'/>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 2)] + "</label></li>";
                        }
                        else
                        {
                            div1 = div1 + "<li><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='C'/>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 2)] + "</label></li>";
                        }
                        
                        if (data.paperTotal.SspcList[i].Choices[(j * 4 + 3)] == "") {
                        }
                        else {
                        if(data.userData[j]=="D")
                        {
                            div1 = div1 + "<li class='chosen'><span class='ABC'>D.</span><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='D' checked='checked'></input>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 3)] + "</label><span class='mark'></span></li>";
                        }
                        else
                        {
                            div1 = div1 + "<li><span class='ABC'>D.</span><label><input name='SspRadio" + i + "_" + j + "'" + " type='radio' value='D'></input>" + data.paperTotal.SspcList[i].Choices[(j * 4 + 3)] + "</label><span class='mark'></span></li>";
                        }
                        }
                        div1 = div1 + "</ol></div><div style='display:none'>"
                        div1 = div1 + "<input type='hidden' name='SspAnswerValue" + i + "_" + j + "'" + "value='" + data.paperTotal.SspcList[i].Info.AnswerValue[j] + "'/>";
                        div1 = div1 + "<input type='hidden' name='SspQuestionID" + i + "_" + j + "'" + "value='" + data.paperTotal.SspcList[i].Info.QuestionID[j] + "'/>";
                        div1 = div1 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + j + "'" + "value='" + data.paperTotal.SspcList[i].Info.ScoreQuestion[j] + "'/><div>";
                        $("#SspcO" + i).append(div1);
                        $("#SspAnswerValue" + i + "_" + j).val(data.paperTotal.SspcList[i].Info.AnswerValue[j]);
                    }
                    for (var k = 0; k < data.paperTotal.SspcList[i].TermNum; k++) {
                        var div2 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + data.paperTotal.SspcList[i].ChoiceNum + k + 1) + "</strong>";
                        var questionline = data.paperTotal.SspcList[i].Info.Problem[(data.paperTotal.SspcList[i].ChoiceNum + k)];
                        var line = "<input type='text' id='Answer" + i + "_" + k + "'" + "name='SspAnswer" + i + "_" + k + "' value='"+data.userData[data.paperTotal.SspcList[i].ChoiceNum + k]+"'/>";
                        questionline = questionline.replace(/__+/g, line);
                        div2 = div2 + questionline + "</div></div>";
                        div2 = div2 + "<div style='display:none'><input type='hidden' name='SspAnswerValue" + i + "_" + (data.paperTotal.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.paperTotal.SspcList[i].Info.AnswerValue[(data.paperTotal.SspcList[i].ChoiceNum + k)] + "'/>";
                        div2 = div2 + "<input type='hidden' name='SspQuestionID" + i + "_" + (data.paperTotal.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.paperTotal.SspcList[i].Info.QuestionID[(data.paperTotal.SspcList[i].ChoiceNum + k)] + "'/>";
                        div2 = div2 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + (data.paperTotal.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.paperTotal.SspcList[i].Info.ScoreQuestion[(data.paperTotal.SspcList[i].ChoiceNum + k)] + "'/></div>";
                        $("#SspcO" + i).append(div2);
                        $("#SspAnswerValue" + i + "_" + (data.paperTotal.SspcList[i].ChoiceNum + k)).val(data.paperTotal.SspcList[i].Info.AnswerValue[(data.paperTotal.SspcList[i].ChoiceNum + k)]);
                    }
                    Num = Num + data.paperTotal.SspcList[i].Info.QuestionCount;
                    $("#SspcTotalNum").val(Num);
                }
            }

            //添加短对话听力题型
            if (data.paperTotal.SlpoList.length > 0) {
                var direction = "<!--短对话听力--><div class='assessmentItems' id='ShortConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var m = 0; m < data.paperTotal.SlpoList.length; m++) {
                    $("#ShortConversations").append("<div id='Slpo" + m + "'class='assessmentItem shortConversation'" + "></div>");
                    var div3 = "<div class='questions'><div class='question'><div class='qBody'><strong class='qNum'>" + (Num + 1) + "</strong>";
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        div3 = div3 + "<a class='playAudio' href='../../SoundFile/" + data.paperTotal.SlpoList[m].SoundFile + ".mp3'>播放音频</a>";
                    }
                    div3 = div3 + "</div>";
                    div3 = div3 + "<ol class='qChoices'>"
                    if(data.userData[Num]=="A")
                    {
                         div3 = div3 + "<li class='chosen'><label><input type='radio' name='SlpRadio" + m + "' value='A' checked='checked'/>" + data.paperTotal.SlpoList[m].Choices[0] + "</label></li>";
                    }
                    else
                    {
                         div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='A'/>" + data.paperTotal.SlpoList[m].Choices[0] + "</label></li>";
                    }
                     if(data.userData[Num]=="B")
                    {
                         div3 = div3 + "<li class='chosen'><label><input type='radio' name='SlpRadio" + m + "' value='B' checked='checked'/>" + data.paperTotal.SlpoList[m].Choices[1] + "</label></li>";
                    }
                    else
                    {
                         div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='B'/>" + data.paperTotal.SlpoList[m].Choices[1] + "</label></li>";
                    }
                     if(data.userData[Num]=="C")
                    {
                         div3 = div3 + "<li class='chosen'><label><input type='radio' name='SlpRadio" + m + "' value='C' checked='checked'/>" + data.paperTotal.SlpoList[m].Choices[2] + "</label></li>";
                    }
                    else
                    {
                         div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='C'/>" + data.paperTotal.SlpoList[m].Choices[2] + "</label></li>";
                    }
                     if(data.userData[Num]=="D")
                    {
                        div3 = div3 + "<li class='chosen'><label><input type='radio' name='SlpRadio" + m + "' value='D' checked='checked'/>" + data.paperTotal.SlpoList[m].Choices[3] + "</label></li>";
                    }
                    else
                    {
                        div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='D'/>" + data.paperTotal.SlpoList[m].Choices[3] + "</label></li>";
                    }
                    div3 = div3 + "</ol>";
                    div3 = div3 + "</div></div><div style='display:none'>";
                    div3 = div3 + "<input type='hidden' name='SlpAssessmentItemID" + m + "'" + "value='" + data.paperTotal.SlpoList[m].Info.ItemID + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpAnswerValue" + m + "'" + "value='" + data.paperTotal.SlpoList[m].Info.AnswerValue + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpQuestionID" + m + "'" + "value='" + data.paperTotal.SlpoList[m].Info.QuestionID + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpScoreQuestion" + m + "'" + "value='" + data.paperTotal.SlpoList[m].Info.ScoreQuestion + "'/></div>";
                    $("#Slpo" + m).append(div3);
                    $("#SlpAnswerValue" + m).val(data.paperTotal.SlpoList[m].Info.AnswerValue);
                    Num = Num + data.paperTotal.SlpoList[m].Info.QuestionCount;
                    $("#SlpoTotalNum").val(data.paperTotal.SlpoList.length);
                }
            }

            //添加长对话听力题型
            if (data.paperTotal.LlpoList.length > 0) {
                var Llpo = 0;
                var direction = "<div class='assessmentItems' id='LongConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var l = 0; l < data.paperTotal.LlpoList.length; l++) {
                    LlpoArray.push(data.paperTotal.LlpoList[l].Info.QuestionCount);
                    $("#LongConversations").append("<div id='Llpo" + l + "'class='assessmentItem longConversation'" + "><div id='LlpoC" + l + "'" + "class='content'></div><div id='LlpoO" + l + "'" + "class='questions'></div></div>");
                    $("#Llpo" + l).append("<input type='hidden' name='LlpAssessmentItemID" + l + "'" + "value='" + data.paperTotal.LlpoList[l].Info.ItemID + "'/>");
                    $("#Llpo" + l).append("<input type='hidden' name='LlpNum" + l + "'" + "value='" + data.paperTotal.LlpoList[l].Info.QuestionCount + "'/>");
                    if (Type == "大学四级试题" || Type == "大学六级试题")
                    { }
                    else {
                        var content = "<div class='content'><p>Conversation " + (l + 1) + "<a class='playAudio' href='../../SoundFile/" + data.paperTotal.LlpoList[l].SoundFile + ".mp3'>播放音频</a></p></div>";
                        $("#LlpoC" + l).append(content);
                    }

                    for (var n = 0; n < data.paperTotal.LlpoList[l].Info.QuestionCount; n++) {
                        var div4 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + n + 1) + "</strong>";
                        if (Type == "大学四级试题" || Type == "大学六级试题")
                        { }
                        else {
                            div4 = div4 + "<a class='playAudio' href='../../SoundFile/" + data.paperTotal.LlpoList[l].Info.questionSound[n] + ".mp3'>播放音频</a>";
                        }
                        div4 = div4 + "</div>";
                        div4 = div4 + "<ol class='qChoices'>";
                         if(data.userData[Num + n]=="A")
                    {
                            div4 = div4 + "<li class='chosen'><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='A' checked='checked'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 0)] + "</label></li>";
                    }
                    else
                    {
                           div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='A'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 0)] + "</label></li>";
                    }
                     if(data.userData[Num + n]=="B")
                    {
                           div4 = div4 + "<li class='chosen'><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='B' checked='checked'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 1)] + "</label></li>";
                    }
                    else
                    {
                           div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='B'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 1)] + "</label></li>";
                    }
                     if(data.userData[Num + n]=="C")
                    {
                           div4 = div4 + "<li class='chosen'><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='C' checked='checked'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 2)] + "</label></li>";
                    }
                    else
                    {
                           div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='C'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 2)] + "</label></li>";
                    }
                     if(data.userData[Num + n]=="D")
                    {
                           div4 = div4 + "<li class='chosen'><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='D' checked='checked'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 3)] + "</label></li>";
                    }
                    else
                    {
                           div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='D'>" + data.paperTotal.LlpoList[l].Choices[(n * 4 + 3)] + "</label></li>";
                    }
                        div4 = div4 + "</ol><div style='display:none'>";
                        div4 = div4 + "<input type='hidden' name='LlpAnswerValue" + l + "_" + n + "'" + "value='" + data.paperTotal.LlpoList[l].Info.AnswerValue[n] + "'/>";
                        div4 = div4 + "<input type='hidden' name='LlpQuestionID" + l + "_" + n + "'" + "value='" + data.paperTotal.LlpoList[l].Info.QuestionID[n] + "'/>";
                        div4 = div4 + "<input type='hidden' name='LlpScoreQuestion" + l + "_" + n + "'" + "value='" + data.paperTotal.LlpoList[l].Info.ScoreQuestion[n] + "'/>";
                        div4 = div4 + "</div></div>";
                        $("#LlpoO" + l).append(div4);
                        $("#LlpAnswerValue" + l + "_" + n).val(data.paperTotal.LlpoList[l].Info.AnswerValue[n]);

                    }
                    Num = Num + data.paperTotal.LlpoList[l].Info.QuestionCount;
                    Llpo += data.paperTotal.LlpoList[l].Info.QuestionCount;
                    $("#LlpoTotalNum").val(Llpo);
                }
            }

            //添加短文听力理解题型
            if (data.paperTotal.RlpoList.length > 0) {
                var direction = "<div class='assessmentItems' id='ComprehensionListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                var Rlpo = 0;
                for (var p = 0; p < data.paperTotal.RlpoList.length; p++) {
                    RlpoArray.push(data.paperTotal.RlpoList[p].Info.QuestionCount);
                    $("#ComprehensionListen").append("<div id='Rlpo" + p + "'class='assessmentItem comprehensionListen'" + "><div id='RlpoC" + p + "'" + "class='content'></div><div id='RlpoO" + p + "'" + "class='questions'></div></div>");
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        var content = "<div class='content'><br/><p>Passage " + (p + 1) + ":<a class='playAudio' href='../../SoundFile/" + data.paperTotal.RlpoList[p].SoundFile + ".mp3'>播放音频</a></p><br/></div>";
                        $("#RlpoC" + p).append(content);
                    }
                    $("#Rlpo" + p).append("<input type='hidden' name='RlpNum" + p + "'" + "value='" + data.paperTotal.RlpoList[p].Info.QuestionCount + "'/>");
                    $("#Rlpo" + p).append("<input type='hidden' name='RlpAssessmentItemID" + p + "'" + "value='" + data.paperTotal.RlpoList[p].Info.ItemID + "'/><br/>");
                    for (var p1 = 0; p1 < data.paperTotal.RlpoList[p].Info.QuestionCount; p1++) {
                        var div5 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + p1 + 1) + "</strong>";
                        if (Type == "大学四级试题" || Type == "大学六级试题") {
                        }
                        else {
                            div5 = div5 + "<a class='playAudio' href='../../SoundFile/" + data.paperTotal.RlpoList[p].Info.questionSound[p1] + ".mp3'>播放音频</a>";
                        }
                        div5 = div5 + "</div>";
                        div5 = div5 + "<ol class='qChoices'>";
                         if(data.userData[Num + p1]=="A")
                    {
                             div5 = div5 + "<li class='chosen'><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='A' checked='checked'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 0)] + "</label></li>";
                    }
                    else
                    {
                             div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='A'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 0)] + "</label></li>";
                    }
                     if(data.userData[Num + p1]=="B")
                    {
                             div5 = div5 + "<li class='chosen'><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='B' checked='checked'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 1)] + "</label></li>";
                    }
                    else
                    {
                             div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='B'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 1)] + "</label></li>";
                    }
                     if(data.userData[Num + p1]=="C")
                    {
                             div5 = div5 + "<li class='chosen'><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='C' checked='checked'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 2)] + "</label></li>";
                    }
                    else
                    {
                             div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='C'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 2)] + "</label></li>";
                    }
                     if(data.userData[Num + p1]=="D")
                    {
                             div5 = div5 + "<li class='chosen'><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='D' checked='checked'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 3)] + "</label></li>";
                    }
                    else
                    {
                             div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='D'>" + data.paperTotal.RlpoList[p].Choices[(p1 * 4 + 3)] + "</label></li>";
                    }
                        div5 = div5 + "</ol><div style='display:none'>";
                        div5 = div5 + "<input type='hidden' name='RlpAnswerValue" + p + "_" + p1 + "'" + "value='" + data.paperTotal.RlpoList[p].Info.AnswerValue[p1] + "'/><br/>";
                        div5 = div5 + "<input type='hidden' name='RlpQuestionID" + p + "_" + p1 + "'" + "value='" + data.paperTotal.RlpoList[p].Info.QuestionID[p1] + "'/><br/>";
                        div5 = div5 + "<input type='hidden' name='RlpScoreQuestion" + p + "_" + p1 + "'" + "value='" + data.paperTotal.RlpoList[p].Info.ScoreQuestion[p1] + "'/><br/>";
                        div5 = div5 + "</div></div>";
                        $("#RlpoO" + p).append(div5);
                        $("#RlpAnswerValue" + p + "_" + p1).val(data.paperTotal.RlpoList[p].Info.AnswerValue[p1]);

                    }
                    Num = Num + data.paperTotal.RlpoList[p].Info.QuestionCount;
                    Rlpo += data.paperTotal.RlpoList[p].Info.QuestionCount;
                    $("#RlpoTotalNum").val(Rlpo);
                }
            }
            //添加复合型听力题型
            if (data.paperTotal.LpcList.length > 0) {
                var Lpc = 0;
                var direction = "<div class='assessmentItems' id='ComplexListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var t = 0; t < data.paperTotal.LpcList.length; t++) {
                    $("#ComplexListen").append("<div id='Lpc" + t + "'class='assessmentItem complexListen'><div id='LpcC" + t + "'" + "class='content'></div><div id='LpcO" + t + "'" + "class='questions'></div></div>")
                    var div = "";
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        div += "<p><a class='playAudio' href='../../SoundFile/" + data.paperTotal.LpcList[t].SoundFile + ".mp3'>播放音频</a></p>";
                    }
                    $("#LpcC" + t).append(div);
                    for (var i = 1; i < data.paperTotal.LpcList[t].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";

                        var text = "<input type='text' name='LpcAnswer" + t + "_" + (i - 1) + "' value='"+data.userData[Num+i-1]+"'/>" + "(" + "<strong class='qNum'>" + (Num + i) + "</strong>" + ")";
                        data.paperTotal.LpcList[t].Script = data.paperTotal.LpcList[t].Script.replace(replace, text);
                    }
                    $("#LpcC" + t).append(data.paperTotal.LpcList[t].Script);
                    $("#Lpc" + t).append("<input type='hidden' name='LpcAssessmentItemID" + t + "'" + "value='" + data.paperTotal.LpcList[t].Info.ItemID + "'/><br/>");
                    $("#LpcC" + t).append("<input type='hidden' name='LpcNum" + t + "'" + "value='" + data.paperTotal.LpcList[t].Info.QuestionCount + "'/>");
                    var div6 = "<div class='question'><div class='qBody'></div>";
                    for (var t1 = 0; t1 < data.paperTotal.LpcList[t].Info.QuestionCount; t1++) {

                        div6 = div6 + "<div style='display:none'><input type='hidden' name='LpcAnswerValue" + t + "_" + t1 + "'" + "value='" + data.paperTotal.LpcList[t].Info.AnswerValue[t1] + "'/><br/>";
                        div6 = div6 + "<input type='hidden' name='LpcQuestionID" + t + "_" + t1 + "'" + "value='" + data.paperTotal.LpcList[t].Info.QuestionID[t1] + "'/><br/>";
                        div6 = div6 + "<input type='hidden' name='LpcScoreQuestion" + t + "_" + t1 + "'" + "value='" + data.paperTotal.LpcList[t].Info.ScoreQuestion[t1] + "'/><br/>";
                        div6 = div6 + "</div>";

                        $("LpcAnswerValue" + t + "_" + t1).val(data.paperTotal.LpcList[t].Info.AnswerValue[t1]);
                    }
                    div6 = div6 + "</div>";
                    $("#LpcO" + t).append(div6);
                    Num = Num + data.paperTotal.LpcList[t].Info.QuestionCount;
                    Lpc += data.paperTotal.LpcList[t].Info.QuestionCount;
                    $("#LpcTotalNum").val(Lpc);
                }
            }

            //添加阅读理解-选词填空题型
            if (data.paperTotal.RpcList.length > 0) {
                var Rpc = 0;
                var direction = "<div class='assessmentItems' id='BankedCloze' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var s = 0; s < data.paperTotal.RpcList.length; s++) {
                    $("#BankedCloze").append("<div id='Rpc" + s + "'class='assessmentItem bankedCloze'" + "><div id='RpcC" + s + "'" + "class='content'></div><div id='RpcO" + s + "'" + "class='choiceBank'></div></div>");
                    var num1=1;
                    for (var i = 1; i < 11; i++) {
                        var replace = "(_" + i + "_)";
                        var find=data.paperTotal.RpcList[s].Content.indexOf(replace);  
                        if(find!=-1)
                        {
                         
                         var value=data.userData[Num+num1-1];
                         var value1;
                         if(value=="A")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[0];
                         }
                         if(value=="B")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[1];
                         }
                         if(value=="C")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[2];
                         }
                         if(value=="D")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[3];
                         }
                         if(value=="E")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[4];
                         }
                         if(value=="F")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[5];
                         }
                         if(value=="G")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[6];
                         }
                         if(value=="H")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[7];
                         }
                         if(value=="I")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[8];
                         }
                         if(value=="J")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[9];
                          }
                         if(value=="K")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[10];
                          }
                         if(value=="L")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[11];
                          }
                         if(value=="M")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[12];
                          }
                         if(value=="N")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[13];
                         }
                         if(value=="O")
                         {
                          value1=data.paperTotal.RpcList[s].WordList[14];
                         }

                        var text= "<input type='text' name='RpcAnswer" + s + "_" + (num1 - 1) + "'value='"+data.userData[Num+num1-1]+"'itemid='"+value1+"'/>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";     

                        data.paperTotal.RpcList[s].Content = data.paperTotal.RpcList[s].Content.replace(replace, text);
                        num1+=1;
                        }
                        
                    }
                    $("#RpcC" + s).append(data.paperTotal.RpcList[s].Content);
                    var div7 = "<ol>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[0] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[1] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[2] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[3] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[4] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[5] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[6] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[7] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[8] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[9] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[10] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[11] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[12] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[13] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.RpcList[s].WordList[14] + "</li>";
                    div7 = div7 + "</ol>";
                    div7 = div7 + "<input type='hidden' name='RpcAssessmentItemID" + s + "'" + "value='" + data.paperTotal.RpcList[s].Info.ItemID + "'/><br/>";
                    div7 = div7 + "<input type='hidden' name='RpcNum" + s + "'" + "value='" + data.paperTotal.RpcList[s].Info.QuestionCount + "'/>";
                    $("#RpcO" + s).append(div7);
                    for (var s1 = 0; s1 < data.paperTotal.RpcList[s].Info.QuestionCount; s1++) {
                        var div = "<div style='display:none'><input type='hidden' name='RpcAnswerValue" + s + "_" + s1 + "'" + "value='" + data.paperTotal.RpcList[s].Info.AnswerValue[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='RpcQuestionID" + s + "_" + s1 + "'" + "value='" + data.paperTotal.RpcList[s].Info.QuestionID[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='RpcScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.paperTotal.RpcList[s].Info.ScoreQuestion[s1] + "'/><br/>";
                        div = div + "</div>";
                        $("#RpcO" + s).append(div);
                        $("#RpcAnswerValue" + s + "_" + s1).val(data.paperTotal.RpcList[s].Info.AnswerValue[s1]);
                    }

                    Num = Num + data.paperTotal.RpcList[s].Info.QuestionCount;
                    Rpc += data.paperTotal.RpcList[s].Info.QuestionCount;
                    $("#RpcTotalNum").val(Rpc);
                }
            }

            //添加阅读理解-选择题型
            if (data.paperTotal.RpoList.length > 0) {
                var Rpo = 0;
                var direction = "<div class='assessmentItems' id='MultipleChoice' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var d = 0; d < data.paperTotal.RpoList.length; d++) {
                    $("#MultipleChoice").append("<div id='Rpo" + d + "'class='assessmentItem multipleChoice'" + "><div id='RpoC" + d + "'" + "class='content'></div><div id='RpoO" + d + "'" + "class='questions'></div></div>");
                    $("#RpoO" + d).append("<input type='hidden' name='RpoAssessmentItemID" + d + "'" + "value='" + data.paperTotal.RpoList[d].Info.ItemID + "'/><br/>");
                    var content = "<br/><p>Passage " + (d + 1) + "</p><br/>" + data.paperTotal.RpoList[d].Content;
                    $("#RpoO" + d).append("<input type='hidden' name='RpoNum" + d + "'" + "value='" + data.paperTotal.RpoList[d].Info.QuestionCount + "'/>");
                    $("#RpoC" + d).append(content);
                    for (var d1 = 0; d1 < data.paperTotal.RpoList[d].Info.QuestionCount; d1++) {
                        var div8 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + d1 + 1) + "</strong>" + data.paperTotal.RpoList[d].Info.Problem[d1] + "</div>";
                        div8 = div8 + "<ol class='qChoices'>";
                        if(data.userData[Num + d1]=="A")
                    {
                            div8 = div8 + "<li class='chosen'><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='A' checked='checked'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 0)] + "</label></li>";
                    }
                    else
                    {
                            div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='A'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 0)] + "</label></li>";
                    }
                    if(data.userData[Num + d1]=="B")
                    {
                            div8 = div8 + "<li class='chosen'><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='B' checked='checked'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 1)] + "</label></li>";
                    }
                    else
                    {
                            div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='B'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 1)] + "</label></li>";
                    }
                    if(data.userData[Num + d1]=="C")
                    {
                            div8 = div8 + "<li class='chosen'><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='C' checked='checked'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 2)] + "</label></li>";
                    }
                    else
                    {
                            div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='C'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 2)] + "</label></li>";
                    }
                    if(data.userData[Num + d1]=="D")
                    {
                            div8 = div8 + "<li class='chosen'><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='D' checked='checked'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 3)] + "</label></li>";
                    }
                    else
                    {
                            div8 = div8 + "<li><label><input name='RpoRadio" + d + "_" + d1 + "'" + " type='radio'  value='D'>" + data.paperTotal.RpoList[d].Choices[(d1 * 4 + 3)] + "</label></li>";
                    }
                        div8 = div8 + "</ol><div style='display:none'>"
                        div8 = div8 + "<input type='hidden' name='RpoAnswerValue" + d + "_" + d1 + "'" + "value='" + data.paperTotal.RpoList[d].Info.AnswerValue[d1] + "'/><br/>";
                        div8 = div8 + "<input type='hidden' name='RpoQuestionID" + d + "_" + d1 + "'" + "value='" + data.paperTotal.RpoList[d].Info.QuestionID[d1] + "'/><br/>";
                        div8 = div8 + "<input type='hidden' name='RpoScoreQuestion" + d + "_" + d1 + "'" + "value='" + data.paperTotal.RpoList[d].Info.ScoreQuestion[d1] + "'/><br/>";
                        div8 = div8 + "</div></div>";
                        $("#RpoO" + d).append(div8);
                        $("#RpoAnswerValue" + d + "_" + d1).val(data.paperTotal.RpoList[d].Info.AnswerValue[d1]);
                    }
                    Num = Num + data.paperTotal.RpoList[d].Info.QuestionCount;
                    Rpo += data.paperTotal.RpoList[d].Info.QuestionCount;
                    $("#RpoTotalNum").val(Rpo);
                }
            }

            //添加阅读理解-信息匹配题型
            if (data.paperTotal.InfMatList.length > 0) {
                var InfoMat = 0;
                var direction = "<div class='assessmentItems' id='InfoMatch' partid='partRC'></div>";
                $("#paperBody").append(direction);
                for (var s = 0; s < data.paperTotal.InfMatList.length; s++) {
                    $("#InfoMatch").append("<div id='InfoMat" + s + "'class='assessmentItem infoMatch'" + "><div id='InfoMatC" + s + "'" + "class='content'></div><div id='InfoMatO" + s + "'" + "class='choiceBank'></div></div>");
                    var num1=1;
                    for (var i = 1; i < 11; i++) {
                        var replace = "(_" + i + "_)";
                        var find=data.paperTotal.InfMatList[s].Content.indexOf(replace);  
                        if(find!=-1)
                        {
                         
                         var value=data.userData[Num+num1-1];
                         var value1;
                         if(value=="A")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[0];
                         }
                         if(value=="B")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[1];
                         }
                         if(value=="C")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[2];
                         }
                         if(value=="D")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[3];
                         }
                         if(value=="E")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[4];
                         }
                         if(value=="F")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[5];
                         }
                         if(value=="G")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[6];
                         }
                         if(value=="H")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[7];
                         }
                         if(value=="I")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[8];
                         }
                         if(value=="J")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[9];
                          }
                         if(value=="K")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[10];
                          }
                         if(value=="L")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[11];
                          }
                         if(value=="M")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[12];
                          }
                         if(value=="N")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[13];
                         }
                         if(value=="O")
                         {
                          value1=data.paperTotal.InfMatList[s].WordList[14];
                         }
                        var text= "<input type='text' name='InfoMatAnswer" + s + "_" + (num1 - 1) + "' value='"+data.userData[Num+num1-1]+"' itemid='"+value1+"'/>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";     

                        data.paperTotal.InfMatList[s].Content = data.paperTotal.InfMatList[s].Content.replace(replace, text);
                        num1+=1;
                        }
                        
                    }
                    $("#InfoMatC" + s).append(data.paperTotal.InfMatList[s].Content);
                    var div7 = "<ol>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[0] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[1] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[2] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[3] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[4] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[5] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[6] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[7] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[8] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[9] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[10] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[11] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[12] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[13] + "</li>";
                    div7 = div7 + "<li>" + data.paperTotal.InfMatList[s].WordList[14] + "</li>";
                    div7 = div7 + "</ol>";
                    div7 = div7 + "<input type='hidden' name='InfoMatAssessmentItemID" + s + "'" + "value='" + data.paperTotal.InfMatList[s].Info.ItemID + "'/><br/>";
                    div7 = div7 + "<input type='hidden' name='InfoMatNum" + s + "'" + "value='" + data.paperTotal.InfMatList[s].Info.QuestionCount + "'/>";
                    $("#InfoMatO" + s).append(div7);
                    for (var s1 = 0; s1 < data.paperTotal.InfMatList[s].Info.QuestionCount; s1++) {
                        var div = "<div style='display:none'><input type='hidden' name='InfoMatAnswerValue" + s + "_" + s1 + "'" + "value='" + data.paperTotal.InfMatList[s].Info.AnswerValue[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='InfoMatQuestionID" + s + "_" + s1 + "'" + "value='" + data.paperTotal.InfMatList[s].Info.QuestionID[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='InfoMatScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.paperTotal.InfMatList[s].Info.ScoreQuestion[s1] + "'/><br/>";
                        div = div + "</div>";
                        $("#InfoMatO" + s).append(div);
                        $("#InfoMatAnswerValue" + s + "_" + s1).val(data.paperTotal.InfMatList[s].Info.AnswerValue[s1]);
                    }

                    Num = Num + data.paperTotal.InfMatList[s].Info.QuestionCount;
                    InfoMat += data.paperTotal.InfMatList[s].Info.QuestionCount;
                    $("#InfoMatTotalNum").val(InfoMat);
                }
            }

            //添加完型填空题型
            if (data.paperTotal.CpList.length > 0) {
                var Cp = 0;
                var direction = "<div class='assessmentItems' id='Cloze' partid='partCL'></div>";
                $("#paperBody").append(direction);
                for (var f = 0; f < data.paperTotal.CpList.length; f++) {
                    $("#Cloze").append("<div id='Cp" + f + "'class='assessmentItem cloze'><div id='CpC" + f + "'" + " class='content'></div><div id='CpO" + f + "'" + "class='questions'></div></div>");
                    var num1=1;
                     for (var i = 1; i < 21; i++) {
                        var replace = "(_" + i + "_)";
                       var find=data.paperTotal.CpList[f].Content.indexOf(replace);
                       if(find!=-1)
                       {
                        var text = "<span class='blank'>&nbsp;</span>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";
                        num1+=1;
                        data.paperTotal.CpList[f].Content = data.paperTotal.CpList[f].Content.replace(replace, text);
                        }
                    }
                    $("#CpC" + f).append(data.paperTotal.CpList[f].Content);
                    $("#CpO" + f).append("<input type='hidden' name='CpAssessmentItemID" + f + "'" + "value='" + data.paperTotal.CpList[f].Info.ItemID + "'/><br/>");
                    $("#CpO" + f).append("<input type='hidden' name='CpNum" + f + "'" + "value='" + data.paperTotal.CpList[f].Info.QuestionCount + "'/>");
                    for (var f1 = 0; f1 < data.paperTotal.CpList[f].Info.QuestionCount; f1++) {
                        var div9 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + f1 + 1) + "</strong></div>";
                        div9 = div9 + "<ol class='qChoices'>"
                        if(data.userData[Num + f1]=="A")
                    {
                            div9 = div9 + "<li class='chosen'><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='A' checked='checked'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 0)] + "</label></li>";
                    }
                    else
                    {
                            div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='A'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 0)] + "</label></li>";
                    }
                    if(data.userData[Num + f1]=="B")
                    {
                            div9 = div9 + "<li class='chosen'><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='B' checked='checked'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 1)] + "</label></li>";
                    }
                    else
                    {
                            div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='B'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 1)] + "</label></li>";
                    }
                    if(data.userData[Num + f1]=="C")
                    {
                            div9 = div9 + "<li class='chosen'><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='C' checked='checked'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 2)] + "</label></li>";
                    }
                    else
                    {
                            div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='C'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 2)] + "</label></li>";
                    }
                    if(data.userData[Num + f1]=="D")
                    {
                           div9 = div9 + "<li class='chosen'><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='D' checked='checked'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 3)] + "</label></li></ol>";
                    }
                    else
                    {
                           div9 = div9 + "<li><label><input name='CpRadio" + f + "_" + f1 + "'" + " type='radio'  value='D'>" + data.paperTotal.CpList[f].Choices[(f1 * 4 + 3)] + "</label></li></ol>";
                    }
                        div9 = div9 + "<div style='display:none'>"
                        div9 = div9 + "<input type='hidden' name='CpAnswerValue" + f + "_" + f1 + "'" + "value='" + data.paperTotal.CpList[f].Info.AnswerValue[f1] + "'/><br/>";
                        div9 = div9 + "<input type='hidden' name='CpQuestionID" + f + "_" + f1 + "'" + "value='" + data.paperTotal.CpList[f].Info.QuestionID[f1] + "'/><br/>";
                        div9 = div9 + "<input type='hidden' name='CpScoreQuestion" + f + "_" + f1 + "'" + "value='" + data.paperTotal.CpList[f].Info.ScoreQuestion[f1] + "'/><br/>";
                        div9 = div9 + "</div></div>";
                        $("#CpO" + f).append(div9);
                        $("#CpAnswerValue" + f + "_" + f1).val(data.paperTotal.CpList[f].Info.AnswerValue[f1]);
                    }
                    Num = Num + data.paperTotal.CpList[f].Info.QuestionCount;
                    Cp += data.paperTotal.CpList[f].Info.QuestionCount;
                    $("#CpTotalNum").val(Cp);
                }

                }
        }
        else{
            PaperID = data.PaperID;
            $("#PaperID").val(data.PaperID);
            Type = data.Type;
            SysSecond = data.Duration * 60; //这里获取倒计时的起始时间  
            $(".paperTitle ").html('<h1>' + data.Title + '</h1>');
            $("#Sspc").val(data.SspcList.length);
            $("#Slpo").val(data.SlpoList.length);
            $("#Llpo").val(data.LlpoList.length);
            $("#Rlpo").val(data.RlpoList.length);
            $("#Lpc").val(data.LpcList.length);
            $("#Rpc").val(data.RpcList.length);
            $("#Rpo").val(data.RpoList.length);
            $("#InfoMat").val(data.InfMatList.length);
            $("#Cp").val(data.CpList.length);
            var Num = 0;
            //导航控制
            var wrp = "<ul class='wrp'>";
            if (data.SspcList.length > 0) {
                wrp += "<li id='partSS'>Skimming and Scanning</li>";
            }
            if (Type == "大学四级试题" || Type == "大学六级试题") {
                wrp += "<li id='partLC' href='../../SoundFile/" + PaperID + ".mp3' autostart='1'> Listen Comprehension</li>";
            }
            else {
                if (data.SlpoList.length > 0 || data.LlpoList.length > 0 || data.RlpoList.length > 0 || data.LpcList.length > 0) {
                    wrp += "<li id='partLC'>Listen Comprehension </li>";
                }
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
                    $("#SkimmingAndScanning").append("<div id='Sspc" + i + "' class='assessmentItem skimmingAndScanning'><div id='SspcC" + i + "'" + "class='content'></div><div id='SspcO" + i + "'" + "class='questions'></div></div><hr/>");
                    $("#SspcC" + i).append(data.SspcList[i].Content);
                    $("#SspcO" + i).append("<div style='display:none'><input type='hidden' id='SspAssessmentItemID" + i + "'" + "name='SspAssessmentItemID" + i + "'" + "value='" + data.SspcList[i].Info.ItemID + "'/><input type='hidden' id='ChoiceNum" + i + "'" + "name='ChoiceNum" + i + "'" + "value='" + data.SspcList[i].ChoiceNum + "'/><input type='hidden' id='TermNum" + i + "'" + "name='TermNum" + i + "'" + "value='" + data.SspcList[i].TermNum + "'/></div>");
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
                        div1 = div1 + "</ol></div><div style='display:none'>"
                        div1 = div1 + "<input type='hidden' name='SspAnswerValue" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.AnswerValue[j] + "'/>";
                        div1 = div1 + "<input type='hidden' name='SspQuestionID" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.QuestionID[j] + "'/>";
                        div1 = div1 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + j + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[j] + "'/><div>";
                        $("#SspcO" + i).append(div1);
                        $("#SspAnswerValue" + i + "_" + j).val(data.SspcList[i].Info.AnswerValue[j]);
                    }
                    for (var k = 0; k < data.SspcList[i].TermNum; k++) {
                        var div2 = "<div class='question'><div class='qBody'><strong class='qNum'>" + (Num + data.SspcList[i].ChoiceNum + k + 1) + "</strong>";
                        var questionline = data.SspcList[i].Info.Problem[(data.SspcList[i].ChoiceNum + k)];
                        var line = "<input type='text' id='Answer" + i + "_" + k + "'" + "name='SspAnswer" + i + "_" + k + "'/>";
                        questionline = questionline.replace(/__+/g, line);
                        div2 = div2 + questionline + "</div></div>";
                        div2 = div2 + "<div style='display:none'><input type='hidden' name='SspAnswerValue" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.AnswerValue[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                        div2 = div2 + "<input type='hidden' name='SspQuestionID" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.QuestionID[(data.SspcList[i].ChoiceNum + k)] + "'/>";
                        div2 = div2 + "<input type='hidden' name='SspScoreQuestion" + i + "_" + (data.SspcList[i].ChoiceNum + k) + "'" + "value='" + data.SspcList[i].Info.ScoreQuestion[(data.SspcList[i].ChoiceNum + k)] + "'/></div>";
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
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        div3 = div3 + "<a class='playAudio' href='../../SoundFile/" + data.SlpoList[m].SoundFile + ".mp3'>播放音频</a>";
                    }
                    div3 = div3 + "</div>";
                    div3 = div3 + "<ol class='qChoices'>"
                    div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='A'/>" + data.SlpoList[m].Choices[0] + "</label></li>";
                    div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='B'/>" + data.SlpoList[m].Choices[1] + "</label></li>";
                    div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='C'/>" + data.SlpoList[m].Choices[2] + "</label></li>";
                    div3 = div3 + "<li><label><input type='radio' name='SlpRadio" + m + "' value='D'/>" + data.SlpoList[m].Choices[3] + "</label></li>";
                    div3 = div3 + "</ol>";
                    div3 = div3 + "</div></div><div style='display:none'>";
                    div3 = div3 + "<input type='hidden' name='SlpAssessmentItemID" + m + "'" + "value='" + data.SlpoList[m].Info.ItemID + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpAnswerValue" + m + "'" + "value='" + data.SlpoList[m].Info.AnswerValue + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpQuestionID" + m + "'" + "value='" + data.SlpoList[m].Info.QuestionID + "'/>";
                    div3 = div3 + "<input type='hidden' name='SlpScoreQuestion" + m + "'" + "value='" + data.SlpoList[m].Info.ScoreQuestion + "'/></div>";
                    $("#Slpo" + m).append(div3);
                    $("#SlpAnswerValue" + m).val(data.SlpoList[m].Info.AnswerValue);
                    Num = Num + data.SlpoList[m].Info.QuestionCount;
                    $("#SlpoTotalNum").val(data.SlpoList.length);
                }
            }

            //添加长对话听力题型
            if (data.LlpoList.length > 0) {
                var Llpo = 0;
                var direction = "<div class='assessmentItems' id='LongConversations' partid='partLC'></div>";
                $("#paperBody").append(direction);
                for (var l = 0; l < data.LlpoList.length; l++) {
                    LlpoArray.push(data.LlpoList[l].Info.QuestionCount);
                    $("#LongConversations").append("<div id='Llpo" + l + "'class='assessmentItem longConversation'" + "><div id='LlpoC" + l + "'" + "class='content'></div><div id='LlpoO" + l + "'" + "class='questions'></div></div>");
                    $("#Llpo" + l).append("<input type='hidden' name='LlpAssessmentItemID" + l + "'" + "value='" + data.LlpoList[l].Info.ItemID + "'/>");
                    $("#Llpo" + l).append("<input type='hidden' name='LlpNum" + l + "'" + "value='" + data.LlpoList[l].Info.QuestionCount + "'/>");
                    if (Type == "大学四级试题" || Type == "大学六级试题")
                    { }
                    else {
                        var content = "<div class='content'><p>Conversation " + (l + 1) + "<a class='playAudio' href='../../SoundFile/" + data.LlpoList[l].SoundFile + ".mp3'>播放音频</a></p></div>";
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
                        div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='A'>" + data.LlpoList[l].Choices[(n * 4 + 0)] + "</label></li>";
                        div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='B'>" + data.LlpoList[l].Choices[(n * 4 + 1)] + "</label></li>";
                        div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='C'>" + data.LlpoList[l].Choices[(n * 4 + 2)] + "</label></li>";
                        div4 = div4 + "<li><label><input type='radio' name='LlpRadio" + l + "_" + n + "' value='D'>" + data.LlpoList[l].Choices[(n * 4 + 3)] + "</label></li>";
                        div4 = div4 + "</ol><div style='display:none'>";
                        div4 = div4 + "<input type='hidden' name='LlpAnswerValue" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.AnswerValue[n] + "'/>";
                        div4 = div4 + "<input type='hidden' name='LlpQuestionID" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.QuestionID[n] + "'/>";
                        div4 = div4 + "<input type='hidden' name='LlpScoreQuestion" + l + "_" + n + "'" + "value='" + data.LlpoList[l].Info.ScoreQuestion[n] + "'/>";
                        div4 = div4 + "</div></div>";
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
                var direction = "<div class='assessmentItems' id='ComprehensionListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
                var Rlpo = 0;
                for (var p = 0; p < data.RlpoList.length; p++) {
                    RlpoArray.push(data.RlpoList[p].Info.QuestionCount);
                    $("#ComprehensionListen").append("<div id='Rlpo" + p + "'class='assessmentItem comprehensionListen'" + "><div id='RlpoC" + p + "'" + "class='content'></div><div id='RlpoO" + p + "'" + "class='questions'></div></div>");
                    if (Type == "大学四级试题" || Type == "大学六级试题") {
                    }
                    else {
                        var content = "<div class='content'><br/><p>Passage " + (p + 1) + ":<a class='playAudio' href='../../SoundFile/" + data.RlpoList[p].SoundFile + ".mp3'>播放音频</a></p><br/></div>";
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
                        div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='A'>" + data.RlpoList[p].Choices[(p1 * 4 + 0)] + "</label></li>";
                        div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='B'>" + data.RlpoList[p].Choices[(p1 * 4 + 1)] + "</label></li>";
                        div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='C'>" + data.RlpoList[p].Choices[(p1 * 4 + 2)] + "</label></li>";
                        div5 = div5 + "<li><label><input type='radio' name='RlpoRadio" + p + "_" + p1 + "' value='D'>" + data.RlpoList[p].Choices[(p1 * 4 + 3)] + "</label></li>";
                        div5 = div5 + "</ol><div style='display:none'>";
                        div5 = div5 + "<input type='hidden' name='RlpAnswerValue" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.AnswerValue[p1] + "'/><br/>";
                        div5 = div5 + "<input type='hidden' name='RlpQuestionID" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.QuestionID[p1] + "'/><br/>";
                        div5 = div5 + "<input type='hidden' name='RlpScoreQuestion" + p + "_" + p1 + "'" + "value='" + data.RlpoList[p].Info.ScoreQuestion[p1] + "'/><br/>";
                        div5 = div5 + "</div></div>";
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
                var direction = "<div class='assessmentItems' id='ComplexListen' partid='partLC'></div>";
                $("#paperBody").append(direction);
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
                    var div6 = "<div class='question'><div class='qBody'></div>";
                    for (var t1 = 0; t1 < data.LpcList[t].Info.QuestionCount; t1++) {

                        div6 = div6 + "<div style='display:none'><input type='hidden' name='LpcAnswerValue" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.AnswerValue[t1] + "'/><br/>";
                        div6 = div6 + "<input type='hidden' name='LpcQuestionID" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.QuestionID[t1] + "'/><br/>";
                        div6 = div6 + "<input type='hidden' name='LpcScoreQuestion" + t + "_" + t1 + "'" + "value='" + data.LpcList[t].Info.ScoreQuestion[t1] + "'/><br/>";
                        div6 = div6 + "</div>";

                        $("LpcAnswerValue" + t + "_" + t1).val(data.LpcList[t].Info.AnswerValue[t1]);
                    }
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
                    var num1=1;
                    for (var i = 1; i < 11; i++) {
                        var replace = "(_" + i + "_)";
                        var find=data.RpcList[s].Content.indexOf(replace);  
                        if(find!=-1)
                        {
                        var text = "<input type='text' name='RpcAnswer" + s + "_" + (num1 - 1) + "'/>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";
                       data.RpcList[s].Content = data.RpcList[s].Content.replace(replace, text);
                        num1+=1;
                        }
                        
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
                    div7 = div7 + "<input type='hidden' name='RpcAssessmentItemID" + s + "'" + "value='" + data.RpcList[s].Info.ItemID + "'/><br/>";
                    div7 = div7 + "<input type='hidden' name='RpcNum" + s + "'" + "value='" + data.RpcList[s].Info.QuestionCount + "'/>";
                    $("#RpcO" + s).append(div7);
                    for (var s1 = 0; s1 < data.RpcList[s].Info.QuestionCount; s1++) {
                        var div = "<div style='display:none'><input type='hidden' name='RpcAnswerValue" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.AnswerValue[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='RpcQuestionID" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.QuestionID[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='RpcScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.RpcList[s].Info.ScoreQuestion[s1] + "'/><br/>";
                        div = div + "</div>";
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
                var direction = "<div class='assessmentItems' id='MultipleChoice' partid='partRC'></div>";
                $("#paperBody").append(direction);
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
                        div8 = div8 + "</ol><div style='display:none'>"
                        div8 = div8 + "<input type='hidden' name='RpoAnswerValue" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.AnswerValue[d1] + "'/><br/>";
                        div8 = div8 + "<input type='hidden' name='RpoQuestionID" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.QuestionID[d1] + "'/><br/>";
                        div8 = div8 + "<input type='hidden' name='RpoScoreQuestion" + d + "_" + d1 + "'" + "value='" + data.RpoList[d].Info.ScoreQuestion[d1] + "'/><br/>";
                        div8 = div8 + "</div></div>";
                        $("#RpoO" + d).append(div8);
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
                    var num1=1;
                    for (var i = 1; i < data.InfMatList[s].Info.QuestionCount + 1; i++) {
                        var replace = "(_" + i + "_)";
                        var find=data.InfMatList[s].Content.indexOf(replace);  
                        if(find!=-1)
                        {
                        var text = "<input type='text' name='InfoMatAnswer" + s + "_" + (num1 - 1) + "'/>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";
                       data.InfMatList[s].Content = data.InfMatList[s].Content.replace(replace, text);
                        num1+=1;
                        }
                        
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
                    div7 = div7 + "<input type='hidden' name='InfoMatAssessmentItemID" + s + "'" + "value='" + data.InfMatList[s].Info.ItemID + "'/><br/>";
                    div7 = div7 + "<input type='hidden' name='InfoMatNum" + s + "'" + "value='" + data.InfMatList[s].Info.QuestionCount + "'/>";
                    $("#InfoMatO" + s).append(div7);
                    for (var s1 = 0; s1 < data.InfMatList[s].Info.QuestionCount; s1++) {
                        var div = "<div style='display:none'><input type='hidden' name='InfoMatAnswerValue" + s + "_" + s1 + "'" + "value='" + data.InfMatList[s].Info.AnswerValue[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='InfoMatQuestionID" + s + "_" + s1 + "'" + "value='" + data.InfMatList[s].Info.QuestionID[s1] + "'/><br/>";
                        div = div + "<input type='hidden' name='InfoMatScoreQuestion" + s + "_" + s1 + "'" + "value='" + data.InfMatList[s].Info.ScoreQuestion[s1] + "'/><br/>";
                        div = div + "</div>";
                        $("#InfoMatO" + s).append(div);
                        $("#InfoMatAnswerValue" + s + "_" + s1).val(data.InfMatList[s].Info.AnswerValue[s1]);
                    }

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
                    var num1=1;
                     for (var i = 1; i < 21; i++) {
                        var replace = "(_" + i + "_)";
                       var find=data.CpList[f].Content.indexOf(replace);
                       if(find!=-1)
                       {
                        var text = "<span class='blank'>&nbsp;</span>" + "(" + "<strong class='qNum'>" + (Num + num1) + "</strong>" + ")";
                        num1+=1;
                        data.CpList[f].Content = data.CpList[f].Content.replace(replace, text);
                        }
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
                        div9 = div9 + "<div style='display:none'>"
                        div9 = div9 + "<input type='hidden' name='CpAnswerValue" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.AnswerValue[f1] + "'/><br/>";
                        div9 = div9 + "<input type='hidden' name='CpQuestionID" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.QuestionID[f1] + "'/><br/>";
                        div9 = div9 + "<input type='hidden' name='CpScoreQuestion" + f + "_" + f1 + "'" + "value='" + data.CpList[f].Info.ScoreQuestion[f1] + "'/><br/>";
                        div9 = div9 + "</div></div>";
                        $("#CpO" + f).append(div9);
                        $("#CpAnswerValue" + f + "_" + f1).val(data.CpList[f].Info.AnswerValue[f1]);
                    }
                    Num = Num + data.CpList[f].Info.QuestionCount;
                    Cp += data.CpList[f].Info.QuestionCount;
                    $("#CpTotalNum").val(Cp);
                }
            }
           }
            if (Type == "大学四级试题" || Type == "大学六级试题") {
                InterValObj = window.setInterval(SetRemainTime, 1000); //倒计时
            }
            startclock();
           
            //Hours = window.setInterval(SetUsedTime, 1000); //计时器
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
                $li.bind('click', function (e) {
                    e.stopPropagation();
                    $(this).find('input').attr('checked', 'checked');
                    $(this).siblings().removeClass('chosen');
                    $(this).addClass('chosen');
                });
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
                    var $instructions = $('<div class="instructions"><b>操作提示：</b>请使用鼠标拖动右侧单词或词组到横线上完成答题。如需更改，直接拖动新的单词覆盖即可。</div>');
					$this.prepend($instructions);
                    var $items = $this.find('.choiceBank li');
                    $items.each(function (i) {
                        $(this).attr('index', String.fromCharCode(65 + i));
                    });
                    $this.find('input[type="text"]').each(function () {
                    var value = $(this).attr("value");
                    if(value!="")
                    {
                        var itemid = $(this).attr("itemid");
                        var blank = $('<span class="blank" class="blank ui-droppable dragDon">'+itemid+'</span>').insertAfter($(this));
                       
                    }
                    else
                    {
                            var blank = $('<span class="blank">&nbsp;</span>').insertAfter($(this));
                     }
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
                    var $instructions = $('<div class="instructions"><b>操作提示：</b>请使用鼠标拖动右侧单词或词组到横线上完成答题。如需更改，直接拖动新的单词覆盖即可。</div>');
					$this.prepend($instructions);
                    var $items = $this.find('.choiceBank li');
                    $items.each(function (i) {
                        $(this).attr('index', String.fromCharCode(65 + i));
                    });
                    $this.find('input[type="text"]').each(function () {
                        var value = $(this).attr("value");
                    if(value!="")
                    {
                        var itemid = $(this).attr("itemid");
                        var blank = $('<span class="blank" class="blank ui-droppable dragDon">'+itemid+'</span>').insertAfter($(this));
                       
                    }
                    else
                    {
                            var blank = $('<span class="blank">&nbsp;</span>').insertAfter($(this));
                     }
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
                $.scrollTo(0);

                var pid = $assessmentItems.eq(id).attr('partid');
                if (pid != currentPart) {
                    $('body').trigger('partChangeEvent', pid);
                    currentPart = pid;
                };
                currentItem = id;
                if (currentItem == 0) { //第一题
                    $btnPrev.hide();
                    if (currentItem == itemsCount - 1) { //只有一题
                        $btnSubmit.show();
                        $btnNext.hide();
                    } else {
                        $btnNext.show();
                        $btnSubmit.hide();
                    }
                    stopclock2();
                    stopclock3();
                    stopclock4();
                    stopclock5();
                    stopclock6();
                    stopclock7();
                    stopclock8();
                    stopclock9();
                    startclock1();
                } else if (currentItem == itemsCount - 1) {//最后一题
                    $btnPrev.show();
                    $btnNext.hide();
                    $btnSubmit.show();
                    stopclock1();
                    stopclock2();
                    stopclock3();
                    stopclock4();
                    stopclock5();
                    stopclock6();
                    stopclock7();
                    stopclock8();
                    startclock9();
                } else { //中间的题
                    $btnPrev.show();
                    $btnNext.show();
                    $btnSubmit.hide();
                    //题型时间控制
                    switch (currentItem) {
                        case 1:
                            stopclock1();
                            stopclock3();
                            stopclock4();
                            stopclock5();
                            stopclock6();
                            stopclock7();
                            stopclock8();
                            stopclock9();
                            startclock2();
                            break;
                        case 2:
                            stopclock1();
                            stopclock2();
                            stopclock4();
                            stopclock5();
                            stopclock6();
                            stopclock7();
                            stopclock8();
                            stopclock9();
                            startclock3();
                            break;
                        case 3:
                            stopclock1();
                            stopclock2();
                            stopclock3();
                            stopclock5();
                            stopclock6();
                            stopclock7();
                            stopclock8();
                            stopclock9();
                            startclock4();
                            break;
                        case 4:
                            stopclock1();
                            stopclock2();
                            stopclock3();
                            stopclock4();
                            stopclock6();
                            stopclock7();
                            stopclock8();
                            stopclock9();
                            startclock5();
                            break;
                        case 5:
                            stopclock1();
                            stopclock2();
                            stopclock3();
                            stopclock4();
                            stopclock5();
                            stopclock7();
                            stopclock8();
                            stopclock9();
                            startclock6();
                            break;
                        case 6:
                            stopclock1();
                            stopclock2();
                            stopclock3();
                            stopclock4();
                            stopclock5();
                            stopclock6();
                            stopclock8();
                            stopclock9();
                            startclock7();
                            break;
                        default:
                            stopclock1();
                            stopclock2();
                            stopclock3();
                            stopclock4();
                            stopclock5();
                            stopclock6();
                            stopclock8();
                            stopclock9();
                            startclock8();
                            break;
                    }
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
                    stopclock8();
                }
            }

            var doQuit = function () {
                if (confirm('是否保存当前练习？')) {
                $.post("/PaperShow/selectFirstpaper",null,function(data){
                if(data=="1")
                {
                alert("第一次练习不能暂存！");
                }
                else{
                $("#Temp").val("2");
                    $("#Submin").click();
                    stopclock1();
                    stopclock2();
                    stopclock3();
                    stopclock4();
                    stopclock5();
                    stopclock6();
                    stopclock7();
                    stopclock8();
                    stopclock9();
                }
                })
                   
                }
                else
                {
                  window.location.href = '/PaperShow/UserTestInfo';
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

        //计时器
        var h = 0, m = 0, s = 0; //h--小时，m--分钟 ，s--秒
        function second() {
            if (s > 0 && (s % 60) == 0) { m += 1; s = 0; }
            if (m > 0 && (m % 60) == 0) { h += 1; m = 0; }
            t = h + ":" + m + ":" + s;
            $(".timeUsed").html(t);
            s += 1;
        }
        function startclock() { se = setInterval(second, 1000); }

        //快速阅读题型计时器
        var s1 = 0;
        var se1;
        function second1() {
            s1 += 1;
            $("#time1").val(s1);
        }
        function startclock1() { se1 = setInterval(second1, 1000); }
        function stopclock1() { clearInterval(se1); }
        //短对话题型计时器
        var s2 = 0;
        var se2;
        function second2() {
            s2 += 1;
            $("#time2").val(s2);
        }
        function startclock2() { se2 = setInterval(second2, 1000); }
        function stopclock2() { clearInterval(se2); }
        //长对话计时器
        var s3 = 0;
        var se3;
        function second3() {
            s3 += 1;
            $("#time3").val(s3);
        }
        function startclock3() { se3 = setInterval(second3, 1000); }
        function stopclock3() { clearInterval(se3); }
        //短文听力理解计时器
        var s4 = 0;
        var se4;
        function second4() {
            s4 += 1;
            $("#time4").val(s4);
        }
        function startclock4() { se4 = setInterval(second4, 1000); }
        function stopclock4() { clearInterval(se4); }
        //复合型听力计时器
        var s5 = 0;
        var se5;
        function second5() {
            s5 += 1;
            $("#time5").val(s5);
        }
        function startclock5() { se5 = setInterval(second5, 1000); }
        function stopclock5() { clearInterval(se5); }
        //阅读理解-选词填空计时器
        var s6 = 0;
        var se6;
        function second6() {
            s6 += 1;
            $("#time6").val(s6);
        }
        function startclock6() { se6 = setInterval(second6, 1000); }
        function stopclock6() { clearInterval(se6); }
        //阅读理解-选择题型计时器
        var s7 = 0;
        var se7;
        function second7() {
            s7 += 1;
            $("#time7").val(s7);
        }
        function startclock7() { se7 = setInterval(second7, 1000); }
        function stopclock7() { clearInterval(se7); }
        //信息匹配计时器
        var s8 = 0;
        var se8;
        function second8() {
            s8 += 1;
            $("#time8").val(s8);
        }
        function startclock8() { se8 = setInterval(second8, 1000); }
        function stopclock8() { clearInterval(se8); }
        //完形填空计时器
        var s9 = 0;
        var se9;
        function second9() {
            s9 += 1;
            $("#time9").val(s9);
        }
        function startclock9() { se9 = setInterval(second9, 1000); }
        function stopclock9() { clearInterval(se9); }
        //倒计时

        //将时间减去1秒，计算分、秒  
        function SetRemainTime() {
            if (SysSecond > 0) {
                SysSecond = SysSecond - 1;
                var second = Math.floor(SysSecond % 60);             // 计算秒      
                var minite = Math.floor(SysSecond / 60);      //计算分  

                $(".timeRemaind").html(minite + "分" + second + "秒");
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
    });
})(jQuery);