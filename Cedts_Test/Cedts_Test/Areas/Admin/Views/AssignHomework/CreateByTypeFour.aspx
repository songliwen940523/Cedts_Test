<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" 
Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Paper>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	自主出题
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="chooseTask">
    <div class = "ceBody">
        <p class="ceInfo">
        您可以从自主题库中选择一份试卷，然后点击右下角的发布按钮进行发布作业。</p>
        <div>
        <ul class="cetList">
        <% foreach (var item in Model)
           { %>
        <li id="<%=item.PaperID%>">
            <h3 class="title">
                <%: item.Title %></h3>
            <p class="meta">
                类型：<strong><%: item.Type %></strong>难度：<strong><%: String.Format("{0:F}", item.Difficult) %></strong>估时：<strong><%: String.Format("{0:F}", item.Duration) %></strong>总分：<strong><%: String.Format("{0:F}", item.Score) %></strong></p>
            <p class="info">
                <%: item.Description %></p>
        </li>
        <% } %>
        </ul>
        </div>
    </div>
    <div class="ceFoot">
        <a href="#link" class="btnAssign">发布</a>
    </div>
    <div id="pp">
    </div>    
</div>
    <script language="javascript" type="text/javascript">
        $('.cetList li').click(function () {
            $(this).siblings().removeClass('chosen');
            $(this).addClass('chosen');
        }).hover(function () { $(this).toggleClass('hover'); });
        $('.btnAssign').click(function (e) {
            e.preventDefault();
            var PaperID;
            $('.cetList li').each(function () {
                var s = $(this).attr("class");
                if (s == "chosen") {
                    PaperID = $(this).attr("id");
                }
            })
            if (typeof PaperID == "undefined" || PaperID == null) {
                alert("请选择试卷！");
            }
            else {
                opentt();
                $.post("/AssignHomework/CreateByTypeFour", { PaperIDNum: PaperID }, function (data) {
                    if (data == "0") {
                        alert("发布失败！");
                    }
                    else {
                        alert("发布成功！");
                    }
                    window.location.href = "/Admin/AssignHomework/Index";
                });

            }
        });
        function opentt() {
            $('#pp').dialog('open');
        }
        $('#pp').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true,
            onBeforeOpen: function () {
                $("#pp").empty().append("<h1><center>正在组卷中，请稍后~~~！<br /><img src='../../../../Images/06.gif' /></center></h1>");
            },
            onBeforeClose: function () { }
        });
        function closett() {
            $('#pp').dialog('close');
        }
    </script>
</asp:Content>

