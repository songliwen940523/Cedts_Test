<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<Cedts_Test.Areas.Admin.Models.CEDTS_PartType>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    试卷添加--题型数量设置
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#btnSet").click(function () {
                $("select").each(function (i) {
                    if (i == 0) { $(this)[0].selectedIndex = 0; }
                    if (i == 1) { $(this)[0].selectedIndex = 8; }
                    if (i == 2) { $(this)[0].selectedIndex = 2; }
                    if (i == 3) { $(this)[0].selectedIndex = 3; }
                    if (i == 4) { $(this)[0].selectedIndex = 1; }
                    if (i == 5) { $(this)[0].selectedIndex = 1; }
                    if (i == 6) { $(this)[0].selectedIndex = 2; }
                    if (i == 7) { $(this)[0].selectedIndex = 1; }
                    if (i == 8) { $(this)[0].selectedIndex = 0; }
                });
            });

            $("#btnRst").click(function () {
                $("select").each(function () {
                    $(this)[0].selectedIndex = 0;
                });
            })
        })
    </script>
    <% using (Html.BeginForm())
       { %>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试卷管理首页", "Index")%>
        =>
        <%:Html.ActionLink("试卷题型数量设置","SetTopic") %>
    </div>
    <input type="hidden" name="PSID" id="hidden" value="" />
    <fieldset>
        <legend>试卷试题数量设置：</legend>
        <%int j = 0;%><%string[] arry = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" }; %>
        <% foreach (var item in Model)
           { %>
        <div id="parttype" style="border: 1px solid red;">
            <%:arry[j] %>、 类型英文名：<%: Html.Label(item.TypeName) %>； 类型中文名：<%: Html.Label(item.TypeName_CN) %><br />
            <br />
            <div id="itemtype">
                <% Html.RenderAction("ItemType", new { PartTypeID = item.PartTypeID }); %>
            </div>
        </div>
        <br />
        <%j++; %>
        <% } %>
    </fieldset>
    <div>
        <center>
            <input type="button" id="btnSet" value="标准试卷设置" />&nbsp;
            <input type="button" id="btnRst" value="清空选项" />
        </center>
    </div>
    <div>
        <center>
            请选择试卷完整的音频文件：<input type="file" name="uploadify" id="uploadify" />
            <div id="fileQueue">
            </div>
        </center>
    </div>
    <div>
        <center>
            <input type="button" id="testbtn" value="上一步" onclick="history.go(-1);" />&nbsp;
            <input type="button" id="btn" value="下一步" />
            <input type="submit" id="btnNext" value="下一步" style="display: none" />
        </center>
    </div>
    <%} %>
    <script type="text/javascript">
        $(function () {
            var filesize = 0;
            $("#uploadify").uploadify({
                'uploader': '../../uploadify/uploadify.swf', // uploadify.swf 文件的相对路径，该swf文件是一个带有文字BROWSE的按钮，点击后淡出打开文件对话框，默认值：uploadify.swf。 
                'script': '/Upload.ashx?type=2', //后台处理程序的相对路径 。默认值：uploadify.php 
                'cancelImg': '../../uploadify/cancel.png', //选择文件到文件队列中后的每一个文件上的关闭按钮图标
                'queueID': 'fileQueue', //文件队列的ID，该ID与存放文件队列的div的ID一致。
                'multi': false, //true：上传多个文件，false：只能上传一个文件
                'auto': false, //设置为true当选择文件后就直接上传了，为false需要点击上传按钮才上传 
                'fileDesc': '请选择mp3,wma格式', //这个属性值必须设置fileExt属性后才有效，
                'fileExt': '*.mp3;*.wma', //设置可以选择的文件的类型，格式如：'*.doc;*.pdf;*.rar'
                'sizeLimit': 104550400, //上传文件的大小限制 。单位字节，104550400=100mb
                'queueSizeLimit': 1, //选择上传文件个数
                'fileDataName': "file", //在服务器处理程序中根据该名字来取上传文件的数据(获取客户端文件集合的name名称，即,Reques.Flies["FileData"])。默认为Filedata 
                'buttonText': "浏览",
                'onSelectOnce': function (event, data) {
                    filesize = data.fileCount;
                },
                'onError': function (event, queueId, fileObj, errorObj) {
                    alert("请选择大小不超过100MB的文件！");
                    closett();
                    $('#uploadify').uploadifyCancel($('.uploadifyQueueItem').last().attr('id').replace('uploadify', ''));
                },
                'onComplete': function (event, queueId, fileObj, response, data) {//response服务器回调的数据
                    $("#hidden").val(response);
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

            $("#btn").click(function () {
                var item1 = $('select[name=1_1]').val();
                var item2 = $('select[name=2_1]').val();
                var item3 = $('select[name=2_2]').val();
                var item4 = $('select[name=2_3]').val();
                var item5 = $('select[name=2_4]').val();
                var item6 = $('select[name=3_1]').val();
                var item7 = $('select[name=3_2]').val();
                var item8 = $('select[name=3_3]').val();
                var item9 = $('select[name=4_1]').val();
                var num = Number(item1) + Number(item2) + Number(item3) + Number(item4) + Number(item5) + Number(item6) + Number(item7) + Number(item8) + Number(item9);
                if (num == 0) {
                    alert("你一道题目都还没有选择！请选择题目数量！");
                    return false;
                }

                if ($('select[name=2_1]').val() > 0 || $('select[name=2_2]').val() > 0 || $('select[name=2_3]').val() > 0 || $('select[name=2_4]').val() > 0) {
                    if (filesize < 1) {
                        alert("请为试卷选择音频文件！");
                        return false;
                    } else {
                        opentt();
                        $('#uploadify').uploadifyUpload();
                    }
                } else {
                    $("#btnNext").click();
                }
            })

        })
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
