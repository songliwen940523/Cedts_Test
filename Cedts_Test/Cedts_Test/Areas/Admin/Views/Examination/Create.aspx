<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_AssessmentItem>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增试题
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var itemNum = 0;
        var filesize = 0;

        function keyPress() {
            var keyCode = event.keyCode;
            if ((keyCode >= 48 && keyCode <= 57)) {
                event.returnValue = true;
            }
            else {
                event.returnValue = false;
            }
        }

        function soundfile() {
            $("#uploadify").uploadify({
                'uploader': '../../../../uploadify/uploadify.swf', // uploadify.swf 文件的相对路径，该swf文件是一个带有文字BROWSE的按钮，点击后淡出打开文件对话框，默认值：uploadify.swf。 
                'script': '/Upload.ashx?type=1', //后台处理程序的相对路径 。默认值：uploadify.php 
                'cancelImg': '../../../../uploadify/cancel.png', //选择文件到文件队列中后的每一个文件上的关闭按钮图标
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
                    $('#uploadify').uploadifyCancel($('.uploadifyQueueItem').last().attr('id').replace('uploadify', ''));
                    closett();
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
                        $("#submit").click();
                    }
                }
            });
        }

        function fristload() {
            var item = $("#ItemType option:selected").val();
            if (item == "2" || item == "5") {
                itemNum = 1;
                // $("#editor").hide();
            }

            else {
                $("#editor").show();
                itemNum = 2;
            }

            $("#diview").empty();

            $.ajax({
                type: "POST",
                url: "/Admin/Examination/Partial",
                async: false,
                data: { item: item },
                success: function (data) {
                    $("#diview").html(data);
                }
            });

            soundfile();
        }

        $(document).ready(function () {
            if ($("#isok").val() == "1") {
                alert("试题添加成功！");
                $("#isok").val("");
            }
            fristload();

            $("#editor").html("<textarea id='textarea' name='textarea' cols='40' rows='8'></textarea>");
            var editor = CKEDITOR.replace('textarea');


            $("#PartType").change(function () {
                if ($(this).val() == 2) {
                    itemNum = 1;
                }
                $("#ItemType").empty();
                var selindex = $("#PartType").val();
                var url = "/Admin/Examination/GetItem/?type=" + selindex;
                $.post(url, { type: selindex }, function (data) {
                    $.each(data, function (i, item) {
                        $("<option value=" + item.Value + ">" + item.Text + "</option>").appendTo("#ItemType");
                    });
                    fristload();
                }, "json");
            });

            $("#ItemType").change(function () {
                fristload();
            });

            $("#btn").click(function () {
                if ($("#PartType").val() == 2) {
                    if (filesize < itemNum) {
                        alert("您选择的音频文件数小于题目数量，请选择音频文件！");
                        return false;
                    }
                }
                if (isTrue()) {
                    var part = $("#PartType").val();
                    if (part == 2) {
                        opentt();
                        $('#uploadify').uploadifyUpload();
                    }
                    else {
                        $("#submit").click();
                    }
                }
            })
        });

        $("#selectnum1").live("change", function () {
            var name = $("#ItemType option:selected").val();
            $("#divb").empty();
            var num = Number($("#SkimmingAndScanning_selectnum").val());
            var num1 = $("#selectnum1").val();
            if (name == "1") {
                for (var n = 0; n < num1; n++) {
                    var div3 = "<br/>"
                    div3 = div3 + "<fieldset style='width: 400px'><legend>第" + (n + num + 1) + "小题：</legend>";
                    div3 = div3 + "<br/>";
                    div3 = div3 + "预设分数：<input type='text' id='SkimmingAndScanning_scorequestion" + (num + n + 1) + "'" + "name='SkimmingAndScanning_scorequestion" + (num + n + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div3 = div3 + "<br />";
                    div3 = div3 + "答题估时：<input type='text' id=" + "'SkimmingAndScanning_timequestion" + (num + n + 1) + "'" + " name='SkimmingAndScanning_timequestion" + (num + n + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div3 = div3 + "<br/>";
                    div3 = div3 + "难度：<select name='SkimmingAndScanning_difficultquestion" + (num + n + 1) + "'" + "id='SkimmingAndScanning_difficultquestion" + (num + n + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
                    div3 = div3 + "<option>0.1</option>";
                    div3 = div3 + "<option>0.2</option>";
                    div3 = div3 + "<option>0.3</option>";
                    div3 = div3 + "<option>0.4</option>";
                    div3 = div3 + "<option>0.5</option>";
                    div3 = div3 + "<option>0.6</option>";
                    div3 = div3 + "<option>0.7</option>";
                    div3 = div3 + "<option>0.8</option>";
                    div3 = div3 + "<option>0.9</option>";
                    div3 = div3 + "<option>1</option>";
                    div3 = div3 + "</select>";
                    div3 = div3 + "<br/>";
                    div3 = div3 + "题目:";
                    div3 = div3 + "&nbsp;&nbsp;<input class='question' type='text' id=" + "'" + "SkimmingAndScanning_Question" + (num + n + 1) + "'" + "name=" + "'" + "SkimmingAndScanning_Question" + (num + n + 1) + "'" + "/>"
                    div3 = div3 + "<br/>";
                    div3 = div3 + "答案:";
                    div3 = div3 + "&nbsp;&nbsp;<input class='answer' type='text' id=" + "'" + "SkimmingAndScanning_textanswer" + (num + n + 1) + "'" + "name=" + "'" + "SkimmingAndScanning_textanswer" + (num + n + 1) + "'" + "/>"
                    div3 = div3 + "<br/>";
                    div3 = div3 + "<a href='#' class='classa' id='SkimmingAndScanning_a" + (num + n + 1) + "'" + " >请选择知识点</a><span id='SkimmingAndScanning_span" + (num + n + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='SkimmingAndScanning_hidden" + (num + n + 1) + "'" + " name='SkimmingAndScanning_hidden" + (num + n + 1) + "'" + " />";
                    div3 = div3 + "<br/>";
                    div3 = div3 + "答案解析:&nbsp;<input class='tip' type='text' id='" + "SkimmingAndScanning_tip" + (num + n + 1) + "'" + "name='" + "SkimmingAndScanning_tip" + (num + n + 1) + "'/>";
                    div3 = div3 + "</br>";
                    div3 = div3 + "</fieldset>";
                    $("#divb").append(div3);
                }
            }
        })

        $(".selectnum").live("change", function () {
            itemNum = Number($(this).val()) + 1;
            var name = $("#ItemType option:selected").val();
            if (name != "5") {
                $("#soundfile").empty();

                var html = "<input type='hidden' id='filename' name='filename' />";
                html += "<h3>请选择听力文件：</h3>";
                html += "<input type='file' name='uploadify' id='uploadify' />";
                html += "<div id='fileQueue'></div>";

                $("#soundfile").html(html);

                soundfile();
            }
            else {
                itemNum = 1;
            }


            if (name == "3") {
                $("#LongConversations_divi").empty();
                var LongConversationsnum = $("#LongConversations_selectnum").val();
                for (var i = 1; i < LongConversationsnum; i++) {
                    var div = "<br/>"
                    div = div + "<fieldset style='width: 400px'><legend>第" + (i + 1) + "小题：</legend>";
                    div = div + "<br/>";
                    div = div + "预设分数：<input type='text' id='LongConversations_scorequestion" + (i + 1) + "'" + "name='LongConversations_scorequestion" + (i + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div = div + "<br />";
                    div = div + "答题估时：<input type='text' id=" + "'LongConversations_timequestion" + (i + 1) + "'" + " name='LongConversations_timequestion" + (i + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "难度：<select name='LongConversations_difficultquestion" + (i + 1) + "'" + "id='LongConversations_difficultquestion" + (i + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
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
                    div = div + "题目:";
                    div = div + "&nbsp;&nbsp;<input class='question' type='text' id=" + "'" + "LongConversations_Question" + (i + 1) + "'" + "name=" + "'" + "LongConversations_Question" + (i + 1) + "'" + "/>"
                    div = div + "<br/>";
                    div = div + " 时间间隔：";
                    div = div + " <input type='text' class='interval' id=" + "'" + "interval_" + (i + 1) + "'" + "name=" + "'" + "interval_" + (i + 1) + "'" + "onkeypress='keyPress()' /><br />";
                    div = div + "<a href='#' class='classa' id='LongConversations_a" + (i + 1) + "'" + " >请选择知识点</a><span id='LongConversations_span" + (i + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='LongConversations_hidden" + (i + 1) + "'" + " name='LongConversations_hidden" + (i + 1) + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "选项:" + "<br />" + "A";
                    div = div + "<input class='option' type='text'" + "id='" + "LongConversations_Option" + (i * 4 + 1) + "'" + "name='LongConversations_Option" + (i * 4 + 1) + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; B ";
                    div = div + "<input class='option' type='text'" + "id='" + "LongConversations_Option" + (i * 4 + 2) + "'" + "name='LongConversations_Option" + (i * 4 + 2) + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; C ";
                    div = div + "<input class='option' type='text'" + "id='" + "LongConversations_Option" + (i * 4 + 3) + "'" + "name='LongConversations_Option" + (i * 4 + 3) + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;&nbsp;&nbsp;&nbsp; D ";
                    div = div + "<input class='option' type='text'" + "id='" + "LongConversations_Option" + (i * 4 + 4) + "'" + "name='LongConversations_Option" + (i * 4 + 4) + "'" + "/>";
                    div = div + "<br/>";
                    div = div + "&nbsp;答案：&nbsp;";
                    div = div + "<select style='width: 144px'" + "id='LongConversations_D" + (i + 1) + "'" + "name='LongConversations_D" + (i + 1) + "'" + ">";
                    div = div + "<option>A</option>";
                    div = div + "<option>B</option>";
                    div = div + "<option>C</option>";
                    div = div + "<option>D</option>";
                    div = div + "</select>";
                    div = div + "</br>";
                    div = div + "答案解析：<input type='text' id='LongConversations_tip" + (i + 1) + "'" + "name='" + "LongConversations_tip" + (i + 1) + "'/>";
                    div = div + "</br>";
                    div = div + "</fieldset>";
                    $("#LongConversations_divi").append(div);
                }
            }

            if (name == "4") {
                $("#Listen_divi").empty();
                var Listennum = $("#Listen_selectnum").val();
                for (var s = 1; s < Listennum; s++) {
                    var div6 = "<br/>"
                    div6 = div6 + "<fieldset style='width: 400px'><legend>第" + (s + 1) + "小题：</legend>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "预设分数：<input type='text' id='Listen_scorequestion" + (s + 1) + "'" + "name='Listen_scorequestion" + (s + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div6 = div6 + "<br />";
                    div6 = div6 + "答题估时：<input type='text' id=" + "'Listen_timequestion" + (s + 1) + "'" + " name='Listen_timequestion" + (s + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "难度：<select name='Listen_difficultquestion" + (s + 1) + "'" + "id='Listen_difficultquestion" + (s + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
                    div6 = div6 + "<option>0.1</option>";
                    div6 = div6 + "<option>0.2</option>";
                    div6 = div6 + "<option>0.3</option>";
                    div6 = div6 + "<option>0.4</option>";
                    div6 = div6 + "<option>0.5</option>";
                    div6 = div6 + "<option>0.6</option>";
                    div6 = div6 + "<option>0.7</option>";
                    div6 = div6 + "<option>0.8</option>";
                    div6 = div6 + "<option>0.9</option>";
                    div6 = div6 + "<option>1</option>";
                    div6 = div6 + "</select>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "题目:";
                    div6 = div6 + "&nbsp;&nbsp;<input class='question' type='text' id=" + "'" + "Listen_Question" + (s + 1) + "'" + "name=" + "'" + "Listen_Question" + (s + 1) + "'" + "/>"
                    div6 = div6 + "<br/>";
                    div6 = div6 + " 时间间隔：";
                    div6 = div6 + " <input type='text' class='interval' id=" + "'" + "interval_" + (s + 1) + "'" + "name=" + "'" + "interval_" + (s + 1) + "'" + "onkeypress='keyPress()' /><br />";
                    div6 = div6 + "<a href='#' class='classa' id='Listen_a" + (s + 1) + "'" + " >请选择知识点</a><span id='Listen_span" + (s + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='Listen_hidden" + (s + 1) + "'" + " name='Listen_hidden" + (s + 1) + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "选项:" + "<br />" + "A";
                    div6 = div6 + "<input class='option' type='text'" + "id='" + "Listen_Option" + (s * 4 + 1) + "'" + "name='Listen_Option" + (s * 4 + 1) + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "&nbsp;&nbsp;&nbsp;&nbsp; B ";
                    div6 = div6 + "<input class='option' type='text'" + "id='" + "Listen_Option" + (s * 4 + 2) + "'" + "name='Listen_Option" + (s * 4 + 2) + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "&nbsp;&nbsp;&nbsp;&nbsp; C ";
                    div6 = div6 + "<input class='option' type='text'" + "id='" + "Listen_Option" + (s * 4 + 3) + "'" + "name='Listen_Option" + (s * 4 + 3) + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "&nbsp;&nbsp;&nbsp;&nbsp; D ";
                    div6 = div6 + "<input class='option' type='text'" + "id='" + "Listen_Option" + (s * 4 + 4) + "'" + "name='Listen_Option" + (s * 4 + 4) + "'" + "/>";
                    div6 = div6 + "<br/>";
                    div6 = div6 + "&nbsp;答案：&nbsp;";
                    div6 = div6 + "<select style='width: 144px'" + "id='Listen_D" + (s + 1) + "'" + "name='Listen_D" + (s + 1) + "'" + ">";
                    div6 = div6 + "<option>A</option>";
                    div6 = div6 + "<option>B</option>";
                    div6 = div6 + "<option>C</option>";
                    div6 = div6 + "<option>D</option>";
                    div6 = div6 + "</select>";
                    div6 = div6 + "</br>";
                    div6 = div6 + "答案解析：<input type='text' id='Listen_tip" + (s + 1) + "'" + "name='" + "Listen_tip" + (s + 1) + "'/>";
                    div6 = div6 + "</br>";
                    div6 = div6 + "</fieldset>";
                    $("#Listen_divi").append(div6);
                }
            }

            if (name == "5") {
                $("#Complex_divi").empty();
                var complexnum = $("#Complex_selectnum").val();
                for (var j = 1; j < complexnum; j++) {
                    var div1 = "<br/>"
                    div1 = div1 + "<fieldset style='width: 400px'><legend>第" + (j + 1) + "小题：</legend>";
                    div1 = div1 + "<br/>";
                    div1 = div1 + "预设分数：<input type='text' id='Complex_scorequestion" + (j + 1) + "'" + "name='Complex_scorequestion" + (j + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div1 = div1 + "<br />";
                    div1 = div1 + "答题估时：<input type='text' id=" + "'Complex_timequestion" + (j + 1) + "'" + " name='Complex_timequestion" + (j + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div1 = div1 + "<br/>";
                    div1 = div1 + "难度：<select name='Complex_difficultquestion" + (j + 1) + "'" + "id='Complex_difficultquestion" + (j + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
                    div1 = div1 + "<option>0.1</option>";
                    div1 = div1 + "<option>0.2</option>";
                    div1 = div1 + "<option>0.3</option>";
                    div1 = div1 + "<option>0.4</option>";
                    div1 = div1 + "<option>0.5</option>";
                    div1 = div1 + "<option>0.6</option>";
                    div1 = div1 + "<option>0.7</option>";
                    div1 = div1 + "<option>0.8</option>";
                    div1 = div1 + "<option>0.9</option>";
                    div1 = div1 + "<option>1</option>";
                    div1 = div1 + "</select>";
                    div1 = div1 + "<br/>";
                    div1 = div1 + "答案:";
                    div1 = div1 + "&nbsp;&nbsp;<input class='answer' type='text' id=" + "'" + "Complex_textanswer" + (j + 1) + "'" + "name=" + "'" + "Complex_textanswer" + (j + 1) + "'" + "/>"
                    div1 = div1 + "<br/>";
                    div1 = div1 + "<a href='#' class='classa' id='Complex_a" + (j + 1) + "'" + " >请选择知识点</a><span id='Complex_span" + (j + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='Complex_hidden" + (j + 1) + "'" + " name='Complex_hidden" + (j + 1) + "'" + " />";
                    div1 = div1 + "<br/>";
                    div1 = div1 + "答案解析:&nbsp;<input class='tip' type='text' id='" + "Complex_tip" + (j + 1) + "'" + "name='" + "Complex_tip" + (j + 1) + "'/>";
                    div1 = div1 + "</br>";
                    div1 = div1 + "</fieldset>";
                    $("#Complex_divi").append(div1);
                }
            }
            if (name == "1") {
                $("#SkimmingAndScanning_divi").empty();
                var SkimmingAndScanningnum = $("#SkimmingAndScanning_selectnum").val();
                for (var k = 1; k < SkimmingAndScanningnum; k++) {
                    var div2 = "<br/>";
                    div2 = div2 + "<fieldset style='width: 400px'><legend>第" + (k + 1) + "小题：</legend>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "预设分数：<input type='text' id='SkimmingAndScanning_scorequestion" + (k + 1) + "'" + "name='SkimmingAndScanning_scorequestion" + (k + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div2 = div2 + "<br />";
                    div2 = div2 + "答题估时：<input type='text' id=" + "'SkimmingAndScanning_timequestion" + (k + 1) + "'" + " name='SkimmingAndScanning_timequestion" + (k + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "难度：<select name='SkimmingAndScanning_difficultquestion" + (k + 1) + "'" + "id='SkimmingAndScanning_difficultquestion" + (k + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
                    div2 = div2 + "<option>0.1</option>";
                    div2 = div2 + "<option>0.2</option>";
                    div2 = div2 + "<option>0.3</option>";
                    div2 = div2 + "<option>0.4</option>";
                    div2 = div2 + "<option>0.5</option>";
                    div2 = div2 + "<option>0.6</option>";
                    div2 = div2 + "<option>0.7</option>";
                    div2 = div2 + "<option>0.8</option>";
                    div2 = div2 + "<option>0.9</option>";
                    div2 = div2 + "<option>1</option>";
                    div2 = div2 + "</select>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "题目:";
                    div2 = div2 + "&nbsp;&nbsp;<input class='question' type='text' id=" + "'" + "SkimmingAndScanning_Question" + (k + 1) + "'" + "name=" + "'" + "SkimmingAndScanning_Question" + (k + 1) + "'" + "/>"
                    div2 = div2 + "<br/>";
                    div2 = div2 + "<a href='#' class='classa' id='SkimmingAndScanning_a" + (k + 1) + "'" + " >请选择知识点</a><span id='SkimmingAndScanning_span" + (k + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='SkimmingAndScanning_hidden" + (k + 1) + "'" + " name='SkimmingAndScanning_hidden" + (k + 1) + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "选项:" + "<br />" + "A";
                    div2 = div2 + "<input class='option' type='text'" + "id='" + "SkimmingAndScanning_Option" + (k * 4 + 1) + "'" + "name='SkimmingAndScanning_Option" + (k * 4 + 1) + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "&nbsp;&nbsp;&nbsp;&nbsp; B ";
                    div2 = div2 + "<input class='option' type='text'" + "id='" + "SkimmingAndScanning_Option" + (k * 4 + 2) + "'" + "name='SkimmingAndScanning_Option" + (k * 4 + 2) + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "&nbsp;&nbsp;&nbsp;&nbsp; C ";
                    div2 = div2 + "<input class='option' type='text'" + "id='" + "SkimmingAndScanning_Option" + (k * 4 + 3) + "'" + "name='SkimmingAndScanning_Option" + (k * 4 + 3) + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "&nbsp;&nbsp;&nbsp;&nbsp; D ";
                    div2 = div2 + "<input class='option' type='text'" + "id='" + "SkimmingAndScanning_Option" + (k * 4 + 4) + "'" + "name='SkimmingAndScanning_Option" + (k * 4 + 4) + "'" + "/>";
                    div2 = div2 + "<br/>";
                    div2 = div2 + "&nbsp;答案：&nbsp;";
                    div2 = div2 + "<select style='width: 144px'" + "id='SkimmingAndScanning_D" + (k + 1) + "'" + "name='SkimmingAndScanning_D" + (k + 1) + "'" + ">";
                    div2 = div2 + "<option>A</option>";
                    div2 = div2 + "<option>B</option>";
                    div2 = div2 + "<option>C</option>";
                    div2 = div2 + "<option>D</option>";
                    div2 = div2 + "</select>";
                    div2 = div2 + "</br>";
                    div2 = div2 + "答案解析：<input type='text' id='SkimmingAndScanning_tip" + (k + 1) + "'" + "name='" + "SkimmingAndScanning_tip" + (k + 1) + "'/>";
                    div2 = div2 + "</br>";
                    div2 = div2 + "</fieldset>";
                    $("#SkimmingAndScanning_divi").append(div2);
                }
            }
            if (name == "7") {
                $("#MultipleChoice_divi").empty();
                var MultipleChoicenum = $("#MultipleChoice_selectnum").val();
                for (var l = 1; l < MultipleChoicenum; l++) {
                    var div4 = "<br/>"
                    div4 = div4 + "<fieldset style='width: 400px'><legend>第" + (l + 1) + "小题：</legend>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "预设分数：<input type='text' id='MultipleChoice_scorequestion" + (l + 1) + "'" + "name='MultipleChoice_scorequestion" + (l + 1) + "'" + "class='" + "scorequestion" + "'" + "/>";
                    div4 = div4 + "<br />";
                    div4 = div4 + "答题估时：<input type='text' id=" + "'MultipleChoice_timequestion" + (l + 1) + "'" + " name='MultipleChoice_timequestion" + (l + 1) + "'" + "class='" + "timequestion" + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "难度：<select name='MultipleChoice_difficultquestion" + (l + 1) + "'" + "id='MultipleChoice_difficultquestion" + (l + 1) + "'" + "class='" + "difficultquestion" + "'" + "style='width: 144px'>";
                    div4 = div4 + "<option>0.1</option>";
                    div4 = div4 + "<option>0.2</option>";
                    div4 = div4 + "<option>0.3</option>";
                    div4 = div4 + "<option>0.4</option>";
                    div4 = div4 + "<option>0.5</option>";
                    div4 = div4 + "<option>0.6</option>";
                    div4 = div4 + "<option>0.7</option>";
                    div4 = div4 + "<option>0.8</option>";
                    div4 = div4 + "<option>0.9</option>";
                    div4 = div4 + "<option>1</option>";
                    div4 = div4 + "</select>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "题目:";
                    div4 = div4 + "&nbsp;&nbsp;<input class='question' type='text' id=" + "'" + "MultipleChoice_Question" + (l + 1) + "'" + "name=" + "'" + "MultipleChoice_Question" + (l + 1) + "'" + "/>"
                    div4 = div4 + "<br/>";
                    div4 = div4 + "<a href='#' class='classa' id='MultipleChoice_a" + (l + 1) + "'" + " >请选择知识点</a><span id='MultipleChoice_span" + (l + 1) + "'" + "class='" + "span" + "'" + "></span><input class='hidden' type='hidden' id='MultipleChoice_hidden" + (l + 1) + "'" + " name='MultipleChoice_hidden" + (l + 1) + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "选项:" + "<br />" + "A";
                    div4 = div4 + "<input class='option' type='text'" + "id='" + "MultipleChoice_Option" + (l * 4 + 1) + "'" + "name='MultipleChoice_Option" + (l * 4 + 1) + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "&nbsp;&nbsp;&nbsp;&nbsp; B ";
                    div4 = div4 + "<input class='option' type='text'" + "id='" + "MultipleChoice_Option" + (l * 4 + 2) + "'" + "name='MultipleChoice_Option" + (l * 4 + 2) + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "&nbsp;&nbsp;&nbsp;&nbsp; C ";
                    div4 = div4 + "<input class='option' type='text'" + "id='" + "MultipleChoice_Option" + (l * 4 + 3) + "'" + "name='MultipleChoice_Option" + (l * 4 + 3) + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "&nbsp;&nbsp;&nbsp;&nbsp; D ";
                    div4 = div4 + "<input class='option' type='text'" + "id='" + "MultipleChoice_Option" + (l * 4 + 4) + "'" + "name='MultipleChoice_Option" + (l * 4 + 4) + "'" + "/>";
                    div4 = div4 + "<br/>";
                    div4 = div4 + "&nbsp;答案：&nbsp;";
                    div4 = div4 + "<select style='width: 144px'" + "id='MultipleChoice_D" + (l + 1) + "'" + "name='MultipleChoice_D" + (l + 1) + "'" + ">";
                    div4 = div4 + "<option>A</option>";
                    div4 = div4 + "<option>B</option>";
                    div4 = div4 + "<option>C</option>";
                    div4 = div4 + "<option>D</option>";
                    div4 = div4 + "</select>";
                    div4 = div4 + "</br>";
                    div4 = div4 + "答案解析：<input type='text' id='MultipleChoice_tip" + (l + 1) + "'" + "name='" + "MultipleChoice_tip" + (l + 1) + "'/>";
                    div4 = div4 + "</br>";
                    div4 = div4 + "</fieldset>";
                    $("#MultipleChoice_divi").append(div4);
                }
            }
        })

        function isTrue() {
            var istrue = true;

            $(".interval").each(function () {
                if ($(this).val() == "") {
                    alert("时间间隔不能为空");
                    istrue = false;
                }
                return istrue;
            });

            if ($.trim($("#score").val()) == "") {
                alert("请设置分数！");
                return false;
            }

            if ($.trim($("#time").val()) == "") {
                alert("请设置做题时间!");
                return false;
            }

            $(".scorequestion").each(function () {
                if ($(this).val() == "") {
                    alert("请设置小题分数");
                    istrue = false;
                    return false;
                }
            });

            if (!istrue) {
                return false;
            }

            $(".timequestion").each(function () {
                if ($(this).val() == "") {
                    alert("请设置小题时间");
                    istrue = false;
                    return false;
                }
            });

            if (!istrue) {
                return false;
            }

            $(".hidden").each(function () {
                if ($(this).val() == "") {
                    alert("知识点不能为空!");
                    istrue = false;
                    return false;
                }
            });

            if (!istrue) {
                return false;
            }

            $(".question").each(function () {
                if ($(this).val() == "") {
                    alert("题目内容不能为空。");
                    istrue = false;
                    return false;
                }
            });

            if (!istrue) {
                return false;
            }

            $(".option").each(function () {
                if ($(this).val() == "") {
                    alert("选项内容不能为空。");
                    istrue = false;
                    return false;
                }
            });

            if (!istrue) {
                return false;
            }

            $(".answer").each(function () {
                if ($(this).val() == "") {
                    alert("答案内容不能为空。");
                    istrue = false;
                    return istrue;
                }
            });

            if (!istrue) {
                return false;
            }

            var itemID = $("#ItemType").val();
            var num = 0;
            if (itemID != 2) {
                $(".scorequestion").each(function () {
                    num = num + Number($.trim($(this).val())) * 10;
                });
                if (Number($.trim($("#score").val())) * 10 != num) {
                    alert("小题分数总和不等于试题分数，请重新设置分数");
                    return false;
                }

                var time = 0;
                $(".timequestion").each(function () {
                    time = time + Number($.trim($(this).val())) * 10;
                });
                if (Number($.trim($("#time").val())) * 10 != time) {
                    alert("小题时间总和不等于试题时间之和,请重新设置时间");
                    return false;
                }

                var difficult = 0.0;
                $(".difficultquestion").each(function () {
                    difficult = difficult + $(this).val() * 10;
                    return difficult;
                });
                if (itemID == 1) {
                    if ((Number($.trim($("#SkimmingAndScanning_selectnum").val())) + Number($.trim($("#selectnum1").val()))) * 10 * parseFloat($.trim($("#difficult").val())) != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }

                if (itemID == 3) {
                    if (Number($.trim($("#LongConversations_selectnum").val())) * 10 * parseFloat($.trim($("#difficult").val())) != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
                if (itemID == 4) {
                    if (Number($.trim($("#Listen_selectnum").val())) * 10 * parseFloat($.trim($("#difficult").val())) != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
                if (itemID == 5) {
                    if (Number($.trim($("#Complex_selectnum").val())) * 10 * parseFloat($.trim($("#difficult").val())) != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
                if (itemID == 6 || itemID == 9) {
                    if (10 * parseFloat($.trim($("#difficult").val())) * 10 != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
                if (itemID == 7) {
                    if (Number($.trim($("#MultipleChoice_selectnum").val())) * 10 * parseFloat($.trim($("#difficult").val())) != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
                if (itemID == 8) {
                    if (20 * parseFloat($.trim($("#difficult").val())) * 10 != difficult) {
                        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
                        return false;
                    }
                    else {
                        return istrue;
                    }
                }
            }
            return istrue;
        }

        $("#score").live("keydown", function () {
            $('#score').numberbox({ min: 0, max: 106, precision: 0 });
        });

        $("#time").live("keydown", function () {
            $('#time').numberbox({ min: 0, max: 150, precision: 0 });
        });
    </script>
    <%using (Html.BeginForm("", "", FormMethod.Post, new { enctype = "multipart/form-data" }))
      {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试题管理首页","Index") %>
        =>
        <%:Html.ActionLink("新增试题","Create") %>
    </div>
    <div>
        <span>
            <%=Html.DropDownList("PartType", ViewData["PartType"] as SelectList, new { id = "PartType" })%>
        </span><span>
            <%=Html.DropDownList("ItemType", ViewData["ItemType"] as SelectList, new { id = "ItemType" })%>
        </span>
    </div>
    <div>
        总计分数：<input type="text" id="score" name="score" /><br />
        总计估时：<input type="text" id="time" name="time" /><br />
        总计难度：<select name="difficult" id="difficult" style="width: 144px">
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
        <div id="editor">
        </div>
    </div>
    <div id="diview">
    </div>
    <div>
        <input type="hidden" id="isok" value='<%:ViewData["IsOk"] %>' />
        <input type="button" id="btn" name="btn" value="确定" />
        <input type="submit" id="submit" class="submit" value="123" style="display: none" />
    </div>
    <div>
        <div id="dd" style="padding: 5px; position: relative;">
            <div style="width: 350px; float: left;">
                <h3>
                    待添加知识点</h3>
                <select size="20" name="listLeft" id="listLeft" class="normal" title="双击可实现右移" style="width: 350px">
                </select>
                <br />
                <input type="button" id="btnRight" value="添加>>" />
            </div>
            <div style="width: 350px; float: right;">
                <h3>
                    已添加知识点</h3>
                <select size="20" name="listRight" id="listRight" class="normal" title="双击可实现左移"
                    style="width: 350px">
                </select>
                <br />
                <input type="button" id="btnLeft" value="<<去除" />
                <input type="button" id="btnUp" value="上  移" />
                <input type="button" id="btnDown" value="下  移" />
            </div>
        </div>
    </div>
    <%} %>
    <script type="text/javascript">
        var aid;
        $('#dd').dialog({
            title: '设置知识点',
            modal: true,
            closed: true,
            width: 700,
            //            height: auto,
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

                    var span;
                    var hidden;
                    var itemid = $("#ItemType").val();
                    if (itemid == 1) {
                        span = "#SkimmingAndScanning_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#SkimmingAndScanning_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 2) {
                        span = "#ShortConversations_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#ShortConversations_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 3) {
                        span = "#LongConversations_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#LongConversations_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 4) {
                        span = "#Listen_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#Listen_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 5) {
                        span = "#Complex_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#Complex_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 6) {
                        span = "#BankedCloze_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#BankedCloze_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 7) {
                        span = "#MultipleChoice_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#MultipleChoice_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 8) {
                        span = "#Cloze_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#Cloze_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }
                    if (itemid == 9) {
                        span = "#InfoMatching_span" + aid.substring(aid.lastIndexOf("a") + 1);
                        hidden = "#InfoMatching_hidden" + aid.substring(aid.lastIndexOf("a") + 1);
                    }

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
