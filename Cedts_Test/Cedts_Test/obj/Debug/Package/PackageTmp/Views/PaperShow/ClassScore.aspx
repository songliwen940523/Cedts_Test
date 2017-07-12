<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Models.SingleStatistics>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	班级成绩
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p class="tableinfo">试卷名称：<%:ViewData["TaskName"]%>&nbsp;&nbsp;&nbsp;&nbsp;班级名称：<%:ViewData["ClassName"] %></p>
    <%if (ViewData["Empty"] == "Empty")
      {%>
      <p class = "taskinfo">该班级没有学生！</p>
    <%} %>
    <%else
      { %>
    <div class="scorecenter">
        <%:Html.Action("SingleScoreList", "PaperShow", new { PaperID = ViewData["PaperID"]})%>
        <table class="tbRecords" cellspacing="0">
            <%if (Model.DoNum == 0)
              {%>
              <tr>
                <td class="ptitle">平均得分率</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">平均得分</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">方差</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">标准差</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">最高分</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">最高分姓名</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">最低分</td>
                <td class="pname">--</td>
              </tr>
              <tr>
                <td class="ptitle">最低分姓名</td>
                <td class="pname">--</td>
              </tr>
            <%} %>
            <%else
              {%>
              <tr>
                <td class="ptitle">平均得分率</td>
                <td class="pname"><%:Model.AvePercent.ToString("0.0")%>%</td>
              </tr>
              <tr>
                <td class="ptitle">平均得分</td>
                <td class="pname"><%:Model.AveScore.ToString("0.0")%></td>
              </tr>
              <tr>
                <td class="ptitle">方差</td>
                <td class="pname"><%:Model.Variance.ToString("0.0")%></td>
              </tr>
              <tr>
                <td class="ptitle">标准差</td>
                <td class="pname"><%:Model.StandardDeviation.ToString("0.0")%></td>
              </tr>
              <tr>
                <td class="ptitle">最高分</td>
                <td class="pname"><%:Model.HighestScore.ToString("0.0")%></td>
              </tr>
              <tr>
                <td class="ptitle">最高分姓名</td>
                <td class="pname"><%:Model.HighestStudName %></td>
              </tr>
              <tr>
                <td class="ptitle">最低分</td>
                <td class="pname"><%:Model.LowestScore.ToString("0.0")%></td>
              </tr>
              <tr>
                <td class="ptitle">最低分姓名</td>
                <td class="pname"><%:Model.LowestStudName %></td>
              </tr>
            <%} %>
            <tr>
                <td class="ptitle">完成人数</td>
                <td class="pname"><%:Model.DoNum %></td>
            </tr>
            <tr>
                <td class="ptitle">未做人数</td>
                <td class="pname"><%:Model.UndoNum %></td>
            </tr>
        </table>
    </div>
    <%} %>

</asp:Content>
