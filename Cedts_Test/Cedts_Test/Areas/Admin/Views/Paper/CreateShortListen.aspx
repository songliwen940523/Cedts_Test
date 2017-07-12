<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    短对话听力题型添加
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function keyPress() {
            var keyCode = event.keyCode;
            if ((keyCode >= 48 && keyCode <= 57)) {
                event.returnValue = true;
            }
            else {
                event.returnValue = false;
            }
        }

        var isok = 0;
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
                div_1 += "时间间隔：<input type='text' class='interval' id='interval" + "_" + i + "'" + " name='interval" + "_" + i + "'" + " onkeypress='keyPress()' /><br />";
                div_1 += "原&nbsp;&nbsp;文：<textarea id='textarea" + "_" + i + "'" + " name='textarea" + "_" + i + "'" + " class='textarea' cols='40' rows='8'></textarea>";
                div_1 += "</div>";

                div_1 += "<div>";
                div_1 += "预设分数：<input type='text' id='scorequestion" + "_" + i + "'" + " name='scorequestion" + "_" + i + "'" + " class='scorequestion' /><br />";
                div_1 += "答题估时：<input type='text' id='timequestion" + "_" + i + "'" + " name='timequestion" + "_" + i + "'" + " class='timequestion' /><br />";
                div_1 += "难度：<select name='difficultquestion" + "_" + i + "'" + " id='difficultquestion" + "_" + i + "'" + " class='difficultquestion' style='width: 144px'>";
                div_1 += "<option>0.1</option><option>0.2</option><option>0.3</option><option>0.4</option><option>0.5</option><option>0.6</option><option>0.7</option><option>0.8</option><option>0.9</option><option>1</option></select><br />";
                div_1 += "题目：&nbsp;&nbsp;<input type='text' id='question" + "_" + i + "'" + " name='question" + "_" + i + "'" + " class='question' /><br />";
                if ($("#checkrole").val() == "Admin") {
                    div_1 += "知识点：<a href='#' class='classa' id='a_a" + "_" + i + "'" + " >请选择知识点</a>";
                    div_1 += "<input type='text' id='txt" + "_" + i + "'" + " name='txt" + "_" + i + "'" + " class='txt' value='' readonly='readonly' />";
                    div_1 += "<input class='hidden' type='hidden' id='hidden_a" + "_" + i + "'" + " name='hidden_a" + "_" + i + "'" + " /><br />";
                }
                div_1 += "选项：<br />";
                div_1 += "A<input type='text' id='optionA" + "_" + i + "'" + " name='optionA" + "_" + i + "'" + " class='option' /><br />";
                div_1 += "B<input type='text' id='optionB" + "_" + i + "'" + " name='optionB" + "_" + i + "'" + " class='option' /><br />";
                div_1 += "C<input type='text' id='optionC" + "_" + i + "'" + " name='optionC" + "_" + i + "'" + " class='option' /><br />";
                div_1 += "D<input type='text' id='optionD" + "_" + i + "'" + " name='optionD" + "_" + i + "'" + " class='option' /><br />";
                div_1 += "答案：&nbsp;<select style='width: 144px' id='answer" + "_" + i + "'" + " name='answer" + "_" + i + "'" + " class='answer' >";
                div_1 += "<option>A</option><option>B</option><option>C</option><option>D</option></select><br />";
                div_1 += "答案解析：<input type='text' id='tip" + "_" + i + "'" + " name='tip" + "_" + i + "'" + " class='tip' /><br />";
                div_1 += "</div>";
                myArray[i - 1] = $("#view").append(div_1);
            }

            //将textarea改为编辑器
            for (var s = 1; s <= itemNum; s++) {
                myArray[s - 1] = CKEDITOR.replace('textarea_' + s);
            }
            $.post("/admin/paper/GetShortListen", null, function (data) {
                if (data == "1") { }
                else {
                    $("#filename").val("");
                    for (var i = 1; i <= data.length; i++) {
                        $("#textfile_" + i).val(data[i - 1].SoundFile);
                        myArray[i - 1].setData(data[i - 1].Script);
                        //$("#textarea_" + i).val(data[i - 1].Script);
                        $("#scorequestion_" + i).val(data[i - 1].Info.ScoreQuestion[0]);
                        $("#timequestion_" + i).val(data[i - 1].Info.TimeQuestion[0]);
                        $("#interval_" + i).val(data[i - 1].Info.QustionInterval);
                        $("#difficultquestion_" + i).children("option").each(function () {
                            if ($(this).val() == data[i - 1].Info.DifficultQuestion[0].toString()) {
                                $(this).attr("selected", true);
                            }
                        });
                        $("#question_" + i).val(data[i - 1].Info.Problem[0]);
                        if ($("#checkrole").val() == "Admin") {
                            $("#txt_" + i).val(data[i - 1].Info.Knowledge[0]);
                            $("#hidden_a_" + i).val(data[i - 1].Info.KnowledgeID[0]);
                        }
                        $("#optionA_" + i).val(data[i - 1].Choices[0]);
                        $("#optionB_" + i).val(data[i - 1].Choices[1]);
                        $("#optionC_" + i).val(data[i - 1].Choices[2]);
                        $("#optionD_" + i).val(data[i - 1].Choices[3]);
                        $("#answer_" + i).children("option").each(function () {
                            if ($(this).val() == data[i - 1].Info.AnswerValue[0]) {
                                $(this).attr("selected", true);
                            }
                        });
                        $("#tip_" + i).val(data[i - 1].Info.Tip[0]);
                    }
                }
            }, "json");

            $("#btn").click(function () {
                if (filesize < itemNum) {
                    alert("您选择的音频文件数小于题目数量，请选择音频文件！");
                    return false;
                }
                if (isTrue()) {
                    opentt();
                    $('#uploadify').uploadifyUpload();
                }
            })

            var filesize = 0;
            $("#uploadify").uploadify({
                'uploader': '../../uploadify/uploadify.swf', // uploadify.swf 文件的相对路径，该swf文件是一个带有文字BROWSE的按钮，点击后淡出打开文件对话框，默认值：uploadify.swf。 
                'script': '/Upload.ashx?type=2', //后台处理程序的相对路径 。默认值：uploadify.php 
                'cancelImg': '../../uploadify/cancel.png', //选择文件到文件队列中后的每一个文件上的关闭按钮图标
                'queueID': 'fileQueue', //文件队列的ID，该ID与存放文件队列的div的ID一致。
                'multi': true, //true：上传多个文件，false：只能上传一个文件
                'auto': false, //设置为true当选择文件后就直接上传了，为false需要点击上传按钮才上传 
                'fileDesc': '请选择mp3,wma格式', //这个属性值必须设置fileExt属性后才有效，
                'fileExt': '*.mp3;*.wma', //设置可以选择的文件的类型，格式如：'*.doc;*.pdf;*.rar'
                'sizeLimit': 10455040, //上传文件的大小限制 。单位字节，104550400=100mb
                'queueSizeLimit': itemNum, //选择上传文件个数
                'fileDataName': "file", //在服务器处理程序中根据该名字来取上传文件的数据(获取客户端文件集合的name名称，即,Reques.Flies["FileData"])。默认为Filedata 
                'buttonText': "浏览",
                'onSelectOnce': function (event, data) {
                    filesize = data.fileCount;
                },
                'onError': function (event, queueId, fileObj, errorObj) {
                    alert("请选择大小不超过10MB的文件！");
                    closett();
                    $('#uploadify').uploadifyCancel($('.uploadifyQueueItem').last().attr('id').replace('uploadify', ''));
                },
                'onComplete': function (event, queueId, fileObj, response, data) {
                    $("#filename").val($("#filename").val() + response + ",");
                },
                'onAllComplete': function (event, data) {
                    if (data.errors > 0) {
                        closett();
                        alert("文件上传失败！");
                    }
                    else {
                        closett();
                        $("#btnNext").click();
                    }
                }
            });

            if ($("#IsLast").val() == "1") {
                $("#sunmit").hide();
            }

            $("#sunmit").click(function () {
                $("#sa").val(1);
                $("#btn").click();
            });

        })

        //验证数据的合法性
        function isTrue() {
            var istrue = true;

            $(".interval").each(function () {
                if ($(this).val() == "") {
                    alert("时间间隔不能为空");
                    istrue = false;
                }
                return istrue;
            });

            $(".scorequestion").each(function () {
                if ($(this).val() == "") {
                    alert("小题分数不能为空！");
                    istrue = false;
                    return istrue;
                }
                return istrue;
            });
            $(".timequestion").each(function () {
                if ($(this).val() == "") {
                    alert("小题估时不能为空！");
                    istrue = false;
                    return istrue;
                }
                return istrue;
            });
            $(".question").each(function () {
                if ($(this).val() == "") {
                    alert("题目不能为空！");
                    istrue = false;
                    return istrue;
                }
                return istrue;
            });
            $(".option").each(function () {
                if ($(this).val() == "") {
                    alert("选项不能为空！");
                    istrue = false;
                    return istrue;
                }
                return istrue;
            });
            if ($("#checkrole").val() == "Admin") {
                $(".hidden").each(function () {
                    if ($(this).val() == "") {
                        alert("知识点不能为空！");
                        istrue = false;
                        return istrue;
                    }
                    return istrue;
                });
            }
            $(".answer").each(function () {
                if ($(this).val() == "") {
                    alert("答案不能为空！");
                    istrue = false;
                    return istrue;
                }
                return istrue;
            });
            return istrue;
        }
    </script>
    <div>
        <%using (Html.BeginForm())
          { %>
        <%: Html.ValidationSummary(true) %>
        <div id="menu">
            当前位置： =>
            <%:Html.ActionLink("试卷管理首页","Index") %>
            =>
            <%:Html.ActionLink("试卷短对话听力添加","CreateShortListen") %>
        </div>
        <input type="hidden" id="IsLast" name="IsLast" value='<%:ViewData["最后一项"] %>' />
        <input type="hidden" id="LastOrder" name="LastOrder" value='<%:ViewData["前一项"] %>' />
        <input type="hidden" id="checkrole" name="checkrole" value='<%:ViewData["角色"] %>' />
        <input type="hidden" id="hidden" name="hidden" value='<%:ViewData["短对话听力"] %>' />
        <input type="hidden" id="hid" name="hid" value='<%:ViewData["ss"] %>' />
        <input type="hidden" id="filename" name="filename" value="" />
        <input type="hidden" id="sa" name="sa" value="2" />
        <div>
            <div>
                <h3>
                    您当前选择了<%:ViewData["短对话听力"] %>个题目，请选择相应的听力文件：
                </h3>
                <input type="file" name="uploadify" id="uploadify" />
                <div id="fileQueue">
                </div>
            </div>
            <div id="view">
            </div>
            <div>
                <input type="button" id="testbtn" value="上一步" />&nbsp;
                <input type="button" id="btn" value="下一步" />
                <input type="submit" id="btnNext" value="下一步" class="btnNext" style="display: none" />
                <input type="button" id="sunmit" value="暂存" />
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
                            var txt = "#txt" + aid.substring(aid.lastIndexOf("a") + 1);
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
                        var partType = 2;
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
