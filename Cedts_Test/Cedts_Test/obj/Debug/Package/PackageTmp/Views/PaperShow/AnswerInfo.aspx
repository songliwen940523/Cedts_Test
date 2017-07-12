<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    详细报告
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $.post("/PaperShow/Suggestions", function (data) {
                $("#Suggestions").append(data);
            })
        })
    </script>
    <div class="generalRemark infoItem">
        <h2>
            整体评价：</h2>
        <div id="Suggestions">
        </div>
        <%--            <b>试卷总分：</b><%:ViewData["TotalScore"] %>分, <b>实际得分：</b><%:ViewData["Score"] %>分,
            <b>实际完成时间：</b><%:ViewData["Minutes"] %>分<%:ViewData["Second"] %>秒
        --%>
    </div>
    <div class="learningProposal infoItem">
        <h2>
            教师点评：</h2>
        <div class="proposalDetail">
            <%:ViewData["Proposal"]%></div>
    </div>
    <div class="userStatistic infoItem">
        <%:Html.Action("TestView") %>
    </div>
</asp:Content>
