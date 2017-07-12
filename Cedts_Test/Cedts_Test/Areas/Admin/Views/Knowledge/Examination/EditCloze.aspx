<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    修改完型填空
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../../../Scripts/EditValidate.js" type="text/javascript"></script>
    <%using (Html.BeginForm())
      {%>
    <%: Html.ValidationSummary(true)%>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试题管理首页","Index") %>
        =>
        <%:Html.ActionLink("修改完型填空","EditCloze") %>
    </div>
    <div>
        <div>
            完型填空原文:
            <div>
                <textarea cols="40" rows="8" id="textarea" name="textarea"></textarea>
            </div>
            预设分数：<input type="text" id="score" name="score" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
            &nbsp;答题估时：
            <input type="text" id="time" name="time" /><br />
            难度：&nbsp;&nbsp;
            <select name="difficult" id="difficult" style="width: 144px">
                <option>0.1</option>
                <option>0.2</option>
                <option>0.3</option>
                <option>0.4</option>
                <option>0.5</option>
                <option>0.6</option>
                <option>0.7</option>
                <option>0.8</option>
                <option>0.9</option>
                <option>1</option>
            </select>
            <br />
        </div>
        <div id="divi">
        </div>
        <div>
            <div style="display: none">
                <input type="text" id="AssessID" name="AssessID" value="<%=ViewData["ID"]%>" />
                <input type="text" id="ItemType" name="ItemType" />
                <input type="text" id="ItemType_CN" name="ItemType_CN" />
                <input type="text" id="PartType" name="PartType" />
                <input type="text" id="QuestionCount" name="QuestionCount" />
                <input type="text" id="QuestionID" name="QuestionID" />
                <input type="text" id="QkID" name="QkID" />
                <input type="text" id="Count" name="Count" />
            </div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="submit" id="submit" value="确定" onclick="return check();"/>
        </div>
        <div>
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
        </div>
    </div>
    <%} %>
    <script type="text/javascript">
        var id = $("#AssessID").val();
        var editor = CKEDITOR.replace('textarea');
        $.ajax({
            type: "POST",
            url: "/Admin/Examination/GetCloze",
            data: { id: id },
            datatype: "json",
            success: function (data) {
                $("#divi").html("");
                $("#ItemType").val(data.Info.ItemType);
                $("#ItemType_CN").val(data.Info.ItemType_CN);
                $("#PartType").val(data.Info.PartType);
                $("#Count").val(data.Info.Count);
                $("#QkID").val(data.Info.QuestionKnowledgeID);
                $("#QuestionCount").val(data.Info.QuestionCount);
                editor.setData(data.Content);
                //$("#textarea").val(data.Content);
                $("#score").val(data.Info.Score);
                $("#time").val(data.Info.ReplyTime);
                var m = $("#difficult").find("option");
                var cnum = Math.ceil(data.Info.Diffcult * 10) / 10;
                m.each(function (i) {
                    if ($(this).text() == cnum) {
                        $(this).attr("selected", true);
                    }
                })
                var num = data.Info.QuestionCount;
                var questionids = "";
                for (var i = 0; i < num; i++) {
                    if (num > 0) {
                        if (i != num - 1) {
                            questionids += data.Info.QuestionID[i].toString() + ",";
                        }
                        else {
                            questionids += data.Info.QuestionID[i].toString();
                        }
                    }
                    else {
                        questionids += data.Info.QuestionID[i].toString() + ",";
                    }
                    var div = "<br/>"
                    div = div + "<fieldset style='width: 400px'><legend>第" + (i + 1) + "小题：</legend>";
                    div = div + "<br/>";
                    div = div + "预设分数：<input type='text' class='scorequestion' id='scorequestion" + (i + 1) + "'" + "name='scorequestion" + (i + 1) + "'" + "value='" + data.Info.ScoreQuestion[i] + "'" + "/>";
                    div = div + "<br />";
                    div = div + "答题估时：<input type='text' class='timequestion' id='timequestion" + (i + 1) + "'" + " name='timequestion" + (i + 1) + "'" + "value='" + data.Info.TimeQuestion[i] + "'/>";
                    div = div + "<br />";
                    div = div + "难度：<select class='difficultquestion' name='difficultquestion" + (i + 1) + "'" + "id='difficultquestion" + (i + 1) + "'" + "style='width: 144px'>";
                    div = div + "<option>0.1</option>";
                    div = div + "<option>0.2</option>";
                    div = div + "<option>0.3</option>";
                    div = div + "<option>0.4</option>";
                    div = div + "<option>0.5</option>";
                    div = div + "<option>0.6</option>";
                    div = div + "<option>0.7</option>";
                    div = div + "<option>0.8</option>";
                    div = div + "<option>0.9</option>";
                    div = div + "<option>1</option>";
                    div = div + "</select>";
                    div = div + "<br/>";
                    div = div + "选项:" + "&nbsp;" + "A";
                    div = div + "<input class='option' type='text'" + "id='" + "Option" + (i * 4 + 1) + "'" + "name='Option" + (i * 4 + 1) + "'" + "value=" + "'" + data.Choices[i * 4] + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; B ";
                    div = div + "<input class='option' type='text'" + "id='" + "Option" + (i * 4 + 2) + "'" + "name='Option" + (i * 4 + 2) + "'" + "value=" + "'" + data.Choices[(i * 4 + 1)] + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; C ";
                    div = div + "<input class='option' type='text'" + "id='" + "Option" + (i * 4 + 3) + "'" + "name='Option" + (i * 4 + 3) + "'" + "value=" + "'" + data.Choices[(i * 4 + 2)] + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; D ";
                    div = div + "<input class='option' type='text'" + "id='" + "Option" + (i * 4 + 4) + "'" + "name='Option" + (i * 4 + 4) + "'" + "value=" + "'" + data.Choices[(i * 4 + 3)] + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "<a href='#' class='classa' id='a" + (i + 1) + "'" + " >请选择知识点</a><span id='span" + (i + 1) + "'" + "'" + "class='" + "span" + "'" + ">" + data.Info.Knowledge[i] + "</span><input class='hidden' type='hidden' id='hidden" + (i + 1) + "'" + " name='hidden" + (i + 1) + "'" + "value=" + "'" + data.Info.KnowledgeID[i] + "'" + " />";
                    div = div + "<br/>";
                    div = div + "&nbsp;答案：&nbsp;";
                    div = div + "<select style='width: 144px'" + "id='D" + (i + 1) + "'" + "name='D" + (i + 1) + "'" + ">";
                    div = div + "<option>A</option>";
                    div = div + "<option>B</option>";
                    div = div + "<option>C</option>";
                    div = div + "<option>D</option>";
                    div = div + "</select>";
                    div = div + "</br>";
                    div = div + "答案解析：<input type='text' id='tip" + (i + 1) + "'" + "name='" + "tip" + (i + 1) + "'" + "value=" + "'" + data.Info.Tip[i] + "'" + "/>";
                    div = div + "</br>";
                    $("#divi").append(div);
                    $("#Option" + (i * 4 + 1)).val(data.Choices[i * 4]);
                    $("#Option" + (i * 4 + 2)).val(data.Choices[(i * 4 + 1)]);
                    $("#Option" + (i * 4 + 3)).val(data.Choices[(i * 4 + 2)]);
                    $("#Option" + (i * 4 + 4)).val(data.Choices[(i * 4 + 3)]);
                    $("#tip" + (i + 1)).val(data.Info.Tip[i]);
                    var na = "#D" + (i + 1);
                    var diff = "#difficultquestion" + (i + 1);
                    var difscore = $(diff).find("option");
                    difscore.each(function (j) {

                        if ($(this).text() == data.Info.DifficultQuestion[i]) {
                            $(this).attr("selected", true);
                        }
                    })

                    var n = $(na).find("option");
                    var answer = data.Info.AnswerValue[i].replace("\"", "");
                    n.each(function (i) {
                        if ($(this).text() == answer) {
                            $(this).attr("selected", true);
                        }
                    })
                    $("#QuestionID").val(questionids);
                }
            }
        })
    </script>
    <script type="text/javascript">
        var aid;
        $('#dd').dialog({
            title: '设置知识点',
            modal: true,
            closed: true,
            //            width: 580,
            //            height: 280,
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
                    var span = "#span" + aid.substring(1);
                    var hidden = "#hidden" + aid.substring(1);
                    $(span).html("");
                    $(hidden).val("");
                    $(span).html(text);
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
                $.post("/admin/examination/EditGetPoint", { PartType: $("#PartType").val() }, function (data) {
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
</asp:Content>
