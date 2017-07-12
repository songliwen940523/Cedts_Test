<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    试卷信息汇总
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var num = $("#hidden").val();
            var array = num.split(',');
            $(".txt").each(function (i) {
                $(this).attr("readonly", "");
                $(this).val(array[i]);
                $(this).attr("readonly", "readonly");
            })

            $("#btnSave").click(function () {
                opentt();
                $.post("/admin/paper/SavePaper", null, function (data) {
                    if (data == 1) {
                        closett();
                        alert("创建成功");
                        window.location.href = "/admin/paper/index";
                    }
                    else {
                        closett();
                        alert("创建失败:" + data + "```请稍后重新点击提交！");
                    }

                }, "json")
            })


        })
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试卷管理首页","Index") %>
        =>
        <%:Html.ActionLink("试卷信息汇总","TotalPaper") %>
    </div>
    <%:Html.Hidden("hidden", ViewData["题目数量"], new { id = "hidden" })%>
    <div>
        <% Html.RenderAction("PaperInfo"); %>
    </div>
    <div>
        <% Html.RenderAction("PartTypeList"); %>
    </div>
    <div>
        <input type="button" id="testbtn" value="上一步" onclick="history.go(-1);" />&nbsp;
        &nbsp;&nbsp;
        <input type="button" id="btnSave" name="btnSave" class="subSave" value="保  存" />
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
