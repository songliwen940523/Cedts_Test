<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    EditComplex
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../../ckeditor/ckeditor.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../../../../Content/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../../../Content/icon.css" rel="stylesheet" type="text/css" />
    <%using (Html.BeginForm())
      {%>
    <%: Html.ValidationSummary(true)%>
    <div>
        复合听力原文:
        <div>
            <textarea cols="40" rows="8" id="textarea" name="textarea"></textarea>
        </div>
        <div id="divi">
        </div>
        <div>
            <div style="display: none">
                <input type="text" id="AssessID" name="AssessID" value="<%=ViewData["ID"]%>" />
                <input type="text" id="ItemType" name="ItemType" />
                <input type="text" id="ItemType_CN" name="ItemType_CN" />
                <input type="text" id="PartType" name="PartType" />
                <input type="text" id="SoundFile" name="SoundFile" />
                <input type="text" id="QuestionCount" name="QuestionCount" />
                <input type="text" id="QuestionID" name="QuestionID" />
                <input type="text" id="QkID" name="QkID" />
                <input type="text" id="Count" name="Count" />
            </div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="button" id="Btn" value="提交" />
            <input type="submit" id="submit" class="submit" value="确定" style="display: none" />
        </div>
        <div>
            <div id="dd" style="padding: 5px;">
                <div style="width: 350px; float: left;">
                    <h3>
                        待添加知识点</h3>
                    <br />
                    <select style="width: 350px;  size="20" name="listLeft" id="listLeft" class="normal" title="双击可实现右移">
                    </select>
                    <br />
                    <input type="button" id="btnRight" value="添加>>" />
                </div>
                <div style="width: 350px; float: right;">
                    <h3>
                        已添加知识点</h3>
                    <br />
                    <select style="width: 350px;  size="20" name="listRight" id="listRight" class="normal" title="双击可实现左移">
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
            url: "/Admin/Examination/GetComplex",
            data: { id: id },
            datatype: "json",
            success: function (data) {
                $("#divi").html("");
                $("#ItemType").val(data.Info.ItemType);
                $("#ItemType_CN").val(data.Info.ItemType_CN);
                $("#PartType").val(data.Info.PartType);
                $("#SoundFile").val(data.SoundFile);
                $("#QuestionCount").val(data.Info.QuestionCount);
                $("#Count").val(data.Info.Count);
                $("#QkID").val(data.Info.QuestionKnowledgeID);
                editor.setData(data.Script);
                //$("#textarea").val(data.Script);
                $("#score").val(data.Info.Score);
                $("#time").val(data.Info.ReplyTime);


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
                    div = div + "<fieldset style='width: 900px'><legend>第" + (i + 1) + "小题：</legend>";
                    div = div + "<input type='hidden' id='OldQuestionID" + (i + 1) + "'" + "name='OldQuestionID" + (i + 1) + "'" + "value='" + data.Info.QuestionID[i] + "'/>";
                    div = div + "<br/>";
                    div = div + "<a href='#' class='classa' id='a" + (i + 1) + "'" + " >请选择知识点</a><span id='span" + (i + 1) + "'" + "'" + "class='" + "span" + "'style='display:none'" + ">" + data.Info.Knowledge[i] + "</span><input class='hidden' type='hidden' id='hidden" + (i + 1) + "'" + " name='hidden" + (i + 1) + "'"  + " />";
                    div = div + "<input type='hidden' id='OldKPID" + (i + 1) + "'" + "name='OldKPID" + (i + 1) + "'" + "value='" + data.Info.KnowledgeID[i] + "'/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;答案：&nbsp;";
                    div = div + "<input class='answer' type='text' id=" + "'" + "textanswer" + (i + 1) + "'" + "name=" + "'" + "textanswer" + (i + 1) + "'" + "value='" + data.Info.AnswerValue[i] + "'" + "style='width:800px'/>"
                    div = div + "</select>";
                    div = div + "</br>";
                    div = div + "答案解析：<input type='text' id='tip" + (i + 1) + "'" + "name='" + "tip" + (i + 1) + "'" + "value=" + "'" + data.Info.Tip[i] + "'" + "style='width:800px'/>";
                    div = div + "</br>";
                    $("#divi").append(div);
                    $("#textanswer" + (i + 1)).val(data.Info.AnswerValue[i]);
                    $("#tip" + (i + 1)).val(data.Info.Tip[i]);
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
                    $(span).show();
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
            var wrapper = $("#dd").parent();
            wrapper.next("div.window-shadow").css("position", "fixed");
            wrapper.css("position", "fixed");
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
    <script type="text/javascript">
        $(function () {
            $("#Btn").click(function () {
                if (IsTrue()) {
                    $("#submit").click();
                }
                else {
                    alert("有小题未赋值知识点，请检查后再提交！");
                }
            })
        })
        function IsTrue() {
            var istrue = true;
            $(".hidden").each(function () {
                if ($(this).val() == "") {
                    istrue = false;
                    return false;
                }
            })
            return istrue;
        }
    </script>
</asp:Content>
