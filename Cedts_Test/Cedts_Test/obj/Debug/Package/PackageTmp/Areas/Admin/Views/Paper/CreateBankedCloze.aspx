﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    阅读理解选词填空题型添加
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var myArray = new Array();
        $(function () {
            var LastItem = $("#LastOrder").val();
            if ($("#hid").val() == "1" && LastItem == 0) {
                $("#testbtn").attr({ "disabled": "disabled" });
            }
            $("#testbtn").click(function () {
                if ($("#hid").val() == "1") {
                    var Up = 1;
                    $.post("/admin/paper/Up", { Up: Up, LastItem: LastItem }, function (data) { self.location = '/Admin/Paper/' + data; }, "text");
                }
                else {
                    $.post("/admin/paper/backup", null, function () { history.go(-1); }, "json");
                }
            });

            var selectnum = 0;
            var itemNum = Number($("#hidden").val());
            $("#view").empty();
            for (var i = 1; i <= itemNum; i++) {
                var div_1 = "<h3>第" + i + "大题:</h3><br/>";
                div_1 += "<div>";
                div_1 += "总计分数：<input type='text' id='score" + "_" + i + "'" + " name='score" + "_" + i + "'" + " class='score' /><br />";
                div_1 += "总计估时：<input type='text' id='time" + "_" + i + "'" + " name='time" + "_" + i + "'" + " class='time'/><br />";
                div_1 += "总计难度：<select id='difficult" + "_" + i + "'" + " name='difficult" + "_" + i + "'" + " class='difficult' style='width: 144px'>";
                div_1 += "<option>0.1</option><option>0.2</option><option>0.3</option><option>0.4</option><option>0.5</option><option>0.6</option><option>0.7</option><option>0.8</option><option>0.9</option><option>1</option></select>";
                div_1 += "<div>";
                div_1 += "原&nbsp;&nbsp;文：";
                div_1 += "<br/>";
                div_1 += "<textarea id='textarea" + "_" + i + "'" + " name='textarea" + "_" + i + "'" + " class='textarea' cols='40' rows='8'></textarea>";
                div_1 += "</div>";
                div_1 += "<div>";
                div_1 += "<br/>";
                var str = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O"];
                for (var j = 1; j <= 15; j++) {
                    div_1 += str[(j - 1)] + "<input type='text' id='option" + "_" + i + "_" + j + "'" + "name='option" + "_" + i + "_" + j + "'" + "class='option'/><br/>";
                }
                div_1 += "</div>";
                div_1 += "<br/>";
                for (var k = 1; k <= 10; k++) {
                    div_1 += "<fieldset style='width: 400px'>";
                    div_1 += "<legend>第" + k + "小题</legend>";
                    div_1 += "预设分数：<input type='text' id='scorequestion" + "_" + i + "_" + k + "'" + " name='scorequestion" + "_" + i + "_" + k + "'" + " class='scorequestion' /><br />";
                    div_1 += "答题估时：<input type='text' id='timequestion" + "_" + i + "_" + k + "'" + " name='timequestion" + "_" + i + "_" + k + "'" + " class='timequestion' /><br />";
                    div_1 += "难度：<select name='difficultquestion" + "_" + i + "_" + k + "'" + " id='difficultquestion" + "_" + i + "_" + k + "'" + " class='difficultquestion' style='width: 144px'>";
                    div_1 += "<option>0.1</option><option>0.2</option><option>0.3</option><option>0.4</option><option>0.5</option><option>0.6</option><option>0.7</option><option>0.8</option><option>0.9</option><option>1</option></select><br />";
                    //div_1 += "题目：&nbsp;&nbsp;<input type='text' id='question" + "_" + i + "_" + k + "'" + " name='question" + "_" + i + "_" + k + "'" + " class='question' /><br />";
                    if ($("#checkrole").val() == "Admin") {
                        div_1 += "知识点：<a href='#' class='classa' id='a_a" + "_" + i + "_" + k + "'" + " >请选择知识点</a>";
                        div_1 += "<input type='text' id='txt_a" + "_" + i + "_" + k + "'" + " name='txt_a" + "_" + i + "_" + k + "'" + " class='txt' value='' readonly='readonly' />";
                        div_1 += "<input class='hidden' type='hidden' id='hidden_a" + "_" + i + "_" + k + "'" + " name='hidden_a" + "_" + i + "_" + k + "'" + " /><br />";
                    }
                    div_1 += "答案：<input type='text' id='answer" + "_" + i + "_" + k + "'" + "name='answer" + "_" + i + "_" + k + "'" + "class='answer'/><br/>";
                    div_1 += "答案解析：<input type='text' id='tip" + "_" + i + "_" + k + "'" + " name='tip" + "_" + i + "_" + k + "'" + " class='tip' />";
                    div_1 += "</fieldset>";
                }
                div_1 += "</div>";
                $("#view").append(div_1);

            }
            for (var s = 1; s <= itemNum; s++) {
                myArray[s - 1] = CKEDITOR.replace('textarea_' + s);
            }
            $(".btnNext").live("click", function () {
                if (isTrue()) {
                    opentt();
                    return true;
                }
                else
                    return false;
            });

            $.post("/admin/paper/GetBankedCloze", null, function (data) {
                if (data == "1") { }
                else {
                    for (var i = 1; i <= data.length; i++) {
                        $("#score_" + i).val(data[i - 1].Info.Score);
                        $("#time_" + i).val(data[i - 1].Info.ReplyTime);
                        $("#difficult_" + i).children("option").each(function () {
                            if ($(this).val() == data[i - 1].Info.Diffcult.toString()) {
                                $(this).attr("selected", true);
                            }
                        });
                        myArray[i - 1].setData(data[i - 1].Content);
                        //$("#textarea_" + i).val(data[i - 1].Content);

                        for (var j = 1; j <= 15; j++) {
                            $("#option_" + i + "_" + j).val(data[i - 1].WordList[j - 1]);
                        }

                        for (var k = 1; k <= 10; k++) {
                            $("#scorequestion_" + i + "_" + k).val(data[i - 1].Info.ScoreQuestion[k - 1]);
                            $("#timequestion_" + i + "_" + k).val(data[i - 1].Info.TimeQuestion[k - 1]);
                            $("#difficultquestion_" + i + "_" + k).children("option").each(function () {
                                if ($(this).val() == data[i - 1].Info.DifficultQuestion[k - 1].toString()) {
                                    $(this).attr("selected", true);
                                }
                            });
                            //$("#question_" + i + "_" + k).val(data[i - 1].Info.Problem[k - 1]);
                            if ($("#checkrole").val() == "Admin") {
                                $("#txt_a_" + i + "_" + k).val(data[i - 1].Info.Knowledge[k - 1]);
                                $("#hidden_a_" + i + "_" + k).val(data[i - 1].Info.KnowledgeID[k - 1]);
                            }
                            $("#answer_" + i + "_" + k).val(data[i - 1].Info.AnswerValue[k - 1].toString());
                            $("#tip_" + i + "_" + k).val(data[i - 1].Info.Tip[k - 1]);
                        }
                    }
                }
            }, "json");

            if ($("#IsLast").val() == "1") {
                $("#sunmit").hide();
            }

        })

        function isTrue() {
            var istrue = true;

            $(".score").each(function () {
                if ($(this).val() == "") {
                    alert("预设分数不能为空！");
                    istrue = false;
                    return istrue;
                }
            });
            $(".time").each(function () {
                if ($(this).val() == "") {
                    alert("时间不能为空！");
                    istrue = false;
                    return istrue;
                }
            });

            $(".scorequestion").each(function () {
                if ($(this).val() == "") {
                    alert("小题分数不能为空！");
                    istrue = false;
                    return istrue;
                }
            });
            $(".timequestion").each(function () {
                if ($(this).val() == "") {
                    alert("小题估时不能为空！");
                    istrue = false;
                    return istrue;
                }
            });
            $(".question").each(function () {
                if ($(this).val() == "") {
                    alert("题目不能为空！");
                    istrue = false;
                    return istrue;
                }
            });
            $(".option").each(function () {
                if ($(this).val() == "") {
                    alert("选项不能为空！");
                    istrue = false;
                    return istrue;
                }
            });
            if ($("#checkrole").val() == "Admin") {
                $(".hidden").each(function () {
                    if ($(this).val() == "") {
                        alert("知识点不能为空！");
                        istrue = false;
                        return istrue;
                    }
                });
            }
            $(".answer").each(function () {
                if ($(this).val() == "") {
                    alert("答案不能为空！");
                    istrue = false;
                    return istrue;
                }
            });

            var item = Number($("#hidden").val())
            for (var s = 1; s <= item; s++) {
                var num = 0.0;
                var num1 = 0;
                var num2 = 0.0;
                for (var p = 1; p <= 10; p++) {
                    var score = "#scorequestion_" + s + "_" + p;
                    num = num + Number($.trim($(score).val())) * 10;

                    var time = "#timequestion_" + s + "_" + p;
                    num1 = num1 + Number($.trim($(time).val())) * 10;

                    var difficult = "#difficultquestion_" + s + "_" + p;
                    num2 = num2 + $(difficult).val() * 10;
                }
                if (Number($.trim($("#score_" + s).val())) * 10 != num) {
                    alert("第" + s + "大题，小题分数总和不等于试题分数，请重新设置分数");
                    istrue = false;
                    return istrue;
                }
                if (Number($.trim($("#time_" + s).val())) * 10 != num1) {
                    alert("第" + s + "大题，小题时间总和不等于试题时间，请重新设置时间");
                    istrue = false;
                    return istrue;
                }
                if (Number($.trim($("#difficult_" + s).val())) * 10 * 10 < num2) {
                    alert("第" + s + "大题，小题难度总和不等于试题难度，请重新设置难度");
                    istrue = false;
                    return istrue;
                }
            }
            return istrue;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#sunmit").click(function () {
                $("#sa").val(1);
                $("#btnNext").click();
            });
        })
    </script>
    <div>
        <%using (Html.BeginForm("", "", FormMethod.Post, new { enctype = "multipart/form-data" }))
          { %>
        <%: Html.ValidationSummary(true) %>
        <div id="menu">
            当前位置： =>
            <%:Html.ActionLink("试卷管理首页","Index") %>
            =>
            <%:Html.ActionLink("试卷选词填空题型添加","CreateBankedCloze") %>
        </div>
        <input type="hidden" id="IsLast" name="IsLast" value='<%:ViewData["最后一项"] %>' />
        <input type="hidden" id="LastOrder" name="LastOrder" value='<%:ViewData["前一项"] %>' />
        <input type="hidden" id="checkrole" name="checkrole" value='<%:ViewData["角色"] %>' />
        <input type="hidden" id="hidden" name="hidden" value='<%:ViewData["阅读选词填空"] %>' />
        <input type="hidden" id="hid" name="hid" value='<%:ViewData["ss"] %>' />
        <input type="hidden" id="sa" name="sa" value="2" />
        <div>
            <div id="view">
            </div>
            <div>
                <input type="button" id="testbtn" value="上一步" />
                <input type="submit" id="btnNext" value="下一步" class="btnNext" />
                <input type="button" id="sunmit" value="暂存"  />
            </div>
        </div>
        <%} %>
        <div>
            <%--弹出层Html--%>
            <div id="dd" style="padding: 5px;">
                <div style="width: 350px; float: left;">
                    <h3>
                        待添加知识点</h3>
                    <br />
                    <select style="width: 350px;" size="20" name="listLeft" id="listLeft" class="normal"
                        title="双击可实现右移">
                    </select>
                    <br />
                    <input type="button" id="btnRight" value="添加>>" />
                </div>
                <div style="width: 350px; float: right;">
                    <h3>
                        已添加知识点</h3>
                    <br />
                    <select style="width: 350px;" size="20" name="listRight" id="listRight" class="normal"
                        title="双击可实现左移">
                    </select>
                    <br />
                    <input type="button" id="btnLeft" value="<<去除" />&nbsp;
                    <input type="button" id="btnUp" value="上  移" />&nbsp;
                    <input type="button" id="btnDown" value="下  移" />
                </div>
            </div>
            <%--弹出层和两个listbox操作的JS--%>
            <script type="text/javascript">
                var aid;
                $('#dd').dialog({
                    title: '设置知识点',
                    modal: true,
                    closed: true,
                    //                    width: 580,
                    //                    height: 280,
                    buttons: [{
                        text: '确认',
                        iconCls: 'icon-ok',
                        handler: function () {
                            var count = $("#listRight option").length;
                            var text = "";
                            var val = "";
                            for (var i = 0; i < count; i++) {
                                if (i == count - 1) {
                                    text += $("#listRight").get(0).options[i].text;
                                    val += $("#listRight").get(0).options[i].value;
                                }
                                else {
                                    text += $("#listRight").get(0).options[i].text + ",";
                                    val += $("#listRight").get(0).options[i].value + ",";
                                }
                            }
                            var txt = "#txt_a" + aid.substring(aid.lastIndexOf("a") + 1);
                            var hidden = "#hidden_a" + aid.substring(aid.lastIndexOf("a") + 1);
                            $(txt).attr("readonly", "");
                            $(txt).val("");
                            $(hidden).val("");
                            $(txt).val(text);
                            $(txt).attr("readonly", "readonly");
                            $(hidden).val(val);
                            $("#listRight").empty();
                            $('#dd').dialog('close');
                        }
                    }, {
                        text: '取消',
                        handler: function () {
                            $("#listRight").empty();
                            $('#dd').dialog('close');
                        }
                    }],
                    onBeforeOpen: function () {
                        var partType = 3;
                        $.post("/admin/examination/getpoint", null, function (data) {
                            var vData = eval('(' + data + ')'); //转换为json对象 
                            //bind data
                            var vlist = "";
                            //遍历json数据
                            jQuery.each(vData.datalist, function (i, n) {
                                vlist += "<option value=" + n.data + ">" + n.text + "</option>";
                            });
                            //绑定数据到listLeft
                            $("#listLeft").empty();
                            $("#listLeft").append(vlist);

                        }, "text");

                    },
                    onBeforeClose: function () { }
                });

                $(".classa").live("click", function () {
                    aid = $(this).attr('id');
                    open1();
                    return false;
                });

                function open1() {
                    $('#dd').dialog('open');
                }
                function close1() {
                    $('#dd').dialog('close');
                }

                //right move
                $("#btnRight").click(function () {
                    moveright();
                });
                //double click to move left
                $("#listLeft").dblclick(function () {
                    moveright();
                });

                //left move 
                $("#btnLeft").click(function () {
                    moveleft();
                });

                //double click to move right
                $("#listRight").dblclick(function () {
                    moveleft();
                });

                function moveright() {
                    //数据option选中的数据集合赋值给变量vSelect
                    var vSelect = $("#listLeft option:selected");
                    //克隆数据添加到listRight中
                    vSelect.clone().appendTo("#listRight");
                    //同时移除listRight中的option
                    vSelect.remove();
                }
                function moveleft() {
                    var vSelect = $("#listRight option:selected");
                    vSelect.clone().appendTo("#listLeft");
                    vSelect.remove();
                }

                var $btnUp = $("#btnUp");
                var $btnDown = $("#btnDown");
                var $lbSelLinkMan = $("#listRight")
                $btnUp.click(function () {
                    $lbSelLinkMan.find("option:selected").each(function (index, item) {
                        ListBox_Order("up");
                    });
                    return false;
                });
                $btnDown.click(function () {
                    $lbSelLinkMan.find("option:selected").each(function (index, item) {
                        ListBox_Order("down");
                    });
                    return false;
                });
                function ListBox_Order(action) {
                    var size = $lbSelLinkMan.find("option").size();
                    var selsize = $lbSelLinkMan.find("option:selected").size();

                    if (size > 0 && selsize > 0) {
                        $lbSelLinkMan.find("option:selected").each(function (index, item) {
                            if (action == "up") {
                                $(item).prev().insertAfter($(item));
                                return false;
                            }
                            else if (action == "down")//down时选中多个连靠则操作没效果
                            {
                                $(item).next().insertBefore($(item));
                                return false;
                            }
                        })
                    }
                    return false;
                }
            </script>
        </div>
    </div>
    <div id="tt">
    </div>
    <script type="text/javascript">
        $('#tt').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true,
            onBeforeOpen: function () {
                $("#tt").empty().append("<h1><center>内容正在提交，请稍后~~~！<br /><img src='../../../../Images/06.gif' /></center></h1>");
            },
            onBeforeClose: function () { }
        });
        function opentt() {
            $('#tt').dialog('open');
        }
        function closett() {
            $('#tt').dialog('close');
        }
    </script>
</asp:Content>
