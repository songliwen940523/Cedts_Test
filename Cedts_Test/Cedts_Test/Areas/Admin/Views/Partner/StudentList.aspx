<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_User>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	学生列表
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function uploadexcel() {
            var ClassID = $("#ClassID").val();
            $("#uploadify").uploadify({
                'uploader': '../../../../uploadify/uploadify.swf', // uploadify.swf 文件的相对路径，该swf文件是一个带有文字BROWSE的按钮，点击后淡出打开文件对话框，默认值：uploadify.swf。 
                'script': '/Upload.ashx?type=3', //后台处理程序的相对路径 。默认值：uploadify.php 
                'cancelImg': '../../../../uploadify/cancel.png', //选择文件到文件队列中后的每一个文件上的关闭按钮图标
                'queueID': 'fileQueue', //文件队列的ID，该ID与存放文件队列的div的ID一致。
                'multi': false, //true：上传多个文件，false：只能上传一个文件
                'auto': false, //设置为true当选择文件后就直接上传了，为false需要点击上传按钮才上传 
                'fileDesc': '请选择xlsx格式', //这个属性值必须设置fileExt属性后才有效，
                'fileExt': '*.xlsx', //设置可以选择的文件的类型，格式如：'*.doc;*.pdf;*.rar'
                'sizeLimit': 10455040, //上传文件的大小限制 。单位字节，104550400=100mb
                'queueSizeLimit': 1, //选择上传文件个数
                'fileDataName': "file", //在服务器处理程序中根据该名字来取上传文件的数据(获取客户端文件集合的name名称，即,Reques.Flies["FileData"])。默认为Filedata 
                'buttonText': "浏览",
                'onError': function (event, queueId, fileObj, errorObj) {
                    alert("请选择大小不超过10MB的文件！");
                    closett();
                    $('#uploadify').uploadifyCancel($('.uploadifyQueueItem').last().attr('id').replace('uploadify', ''));
                },
                'onComplete': function (event, queueId, fileObj, response, data) {

                    $.post("/Admin/Partner/Excel", { filepath: response, classid: ClassID }, function (data) {
                        if (data.message != 1) {
                            $('#tt').dialog('close');
                            alert(data.url);
                        }
                        else {
                            var arr = new Array();
                            var brr = new Array();
                            brr = data.why.split(',');
                            arr = data.url.split(',');
                            var m = "";
                            for (var i = 0; i < arr.length - 1; i++) {
                                m += "第" + arr[i] + "行数据有错,原因：" + brr[i] + "<br/>";
                            }
                            $("#tt").empty().append("<h1><center>成功导入" + arr[arr.length - 1] + "个学生！<br/></center></h1>");
                            if (m != "")
                                $("#tt").append("<center><div style='color:Red;'>" + m + "</div></center>");
                            $("#tt").append("<center><div><input type='button' value='关 闭' onclick='closett();' /></div></center>");
                        }
                    }, "json");
                }
            });
        }
        
    </script>
    <div style="padding: 10px;">
        <center>
            当前班级<%:ViewData["Class"]%>成员总共有<%:ViewData["Tcount"] %>人，待审核人数有：<%:ViewData["Fcount"] %>人。
        </center>
    </div>
    <div id="pp">
        <table class="tbRecords" cellspacing="0" >
            <tr>
                <th class="pname">
                    学生帐号
                </th>
                <th class="pname">
                    学生邮件
                </th>
                <th class="pname">
                    学生角色
                </th>
                <th class="pname">
                    学生性别
                </th>
                <th class="pname">
                    学生姓名
                </th>
                <th class="pname">
                    学生电话
                </th>
                <th class="pname">
                    学生学号
                </th>
                <th class="pname">
                    注册时间
                </th>
                <th class="pname">
                    审核状态
                </th>
                <th class="pname">
                </th>
            </tr>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td>
                    <%: item.UserAccount %>
                </td>
                <td>
                    <%: item.Email %>
                </td>
                <td>
                    <%: item.Role %>
                </td>
                <td>
                    <%: item.Sex %>
                </td>
                <td>
                    <%: item.UserNickname %>
                </td>
                <td>
                    <%: item.Phone %>
                </td>
                <td>
                    <%: item.StudentNumber %>
                </td>
                <td>
                    <%: String.Format("{0:g}", item.RegisterTime) %>
                </td>
                <td>
                    <%if (item.State != true)
                      { %>
                    <span style="color: Red">待审核</span>
                    <%} %>
                    <%else
                      { %>
                    <span style="color: Blue">通过</span>
                    <%} %>
                </td>
                <td>
                    <%if (item.State != true)
                      { %>
                    <%:Html.ActionLink("同意", "Audit", "User", new { id=item.UserID,State=true})%>
                    <%:Html.ActionLink("拒绝", "Audit", "User", new { id=item.UserID,State=false})%>
                    <%} %>
                    <%else
                      { %>
                      <%: Html.ActionLink("编辑", "EditStudent", new {  id=item.UserID}) %>
                    <a href="#" id="<%:item.UserID %>" class="a">移除</a>
                    <%} %>
                </td>
            </tr>
            <% } %>
        </table>
        <%=Html.Pager(Model, new PagerOptions  
{  
    PageIndexParameterName = "id",  
    CssClass = "pages", 
    FirstPageText = "首页", 
    LastPageText = "末页",  
    PrevPageText = "上一页",  
    NextPageText = "下一页",  
    CurrentPagerItemWrapperFormatString = "<span class=\"cpb\">{0}</span>",  
    ShowPageIndexBox = true, 
    NumericPagerItemWrapperFormatString = "<span class=\"item\">{0}</span>",  
    PageIndexBoxType = PageIndexBoxType.DropDownList,   
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" 
})%>
        <br />
        <p>
            <%: Html.ActionLink("单个导入学生", "AddStudent", new { id = ViewData["ClassID"]})%>&nbsp;<input type="button" value="Excel导入学生信息"
                id="btnE" />
        </p>
    </div>
    <%--隐藏层Html--%>
    <div id="dd" style="padding: 5px; display: none">
        <center>
            <br />
            格式要求：(EXCEL2007及以上版本)共5列，<br />
            第一列：学生编号，<br />
            第二列：学生姓名，<br />
            第三列：学生性别，<br />
            第四列：学生邮箱，<br />
            第五列：联系电话<br />
            说明：学生编号是学生的学号，初始帐号即为学生编号，密码默认为123123，用户可以登录后修改密码。
            <br />
            <div style="width: 600px; height: auto">
                请选择要导入的Excle文件：<br />
                <br />
                <input type="file" name="uploadify" id="uploadify" />
                <div id="fileQueue">
                </div>
            </div>
            <br />
            <br />
            <div>
                <input type="button" value="导入" id="btnSend" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input type="button" value="取消" id="btnBack" />
            </div>
        </center>
    </div>
    <div id="tt">
    </div>
    <input type="hidden" id="ClassID" value='<%:ViewData["ClassID"] %>' />
    <%--弹出层JS--%>
    <script type="text/javascript">
        $("#btnE").click(function () {
            $('#dd').show();
            $("#pp").hide();
        })

        $('#tt').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true,
            onBeforeOpen: function () {
                $("#tt").empty().append("<h1><center>正在导入，请稍后~~~！<br /><img src='../../../../Images/06.gif' /></center></h1>");
            }
        });
        function opentt() {
            $('#tt').dialog('open');
        }
        function closett() {
            $('#tt').dialog('close');
            window.location.reload();
        }

        $(function () {
            $.messager.defaults = { ok: "确   定", cancel: "取   消" };
            $(".a").click(function () {
                var id = $(this).attr("id");
                var ClassID = $("#ClassID").val();
                $.messager.confirm('提    示', '<b>你确认要移除该学生?<br>如果移除，与之相关的作业信息将全部丢失！</b>', function (r) {
                    if (r) {
                        location.href = "/Admin/Partner/DeleteStudent?id=" + id + "&classid=" + ClassID;
                    }
                });
            });
            uploadexcel();

            $("#btnSend").click(function () {
                $('#tt').dialog('open');
                $('#uploadify').uploadifyUpload();
            })
            $("#btnBack").click(function () {
                $("#dd").hide();
                $("#pp").show();
            })
        })
    </script>
</asp:Content>
