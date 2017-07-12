<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PagedList<Cedts_Test.Models.TestRecord>>" %>
<%@ Import Namespace="Cedts_Test.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<div>
    <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">
                    试卷名称
                </th>
                <th class="ptime">
                    提交时间
                </th>
                <th class="pstat">
                    完成状态
                </th>
                <th class="pscore">
                    结果
                </th>
                <th class="paction">
                    操作
                </th>
            </tr>
        </thead>
        <tfoot>
            <tr>
                <td colspan="5" class="more">
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
                </td>
            </tr>
        </tfoot>
        <tbody>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="ptime">
                    <%: String.Format("{0:g}", item.FinishTime) %>
                </td>
                <td class="pstat">
                <%if (item.isFinish == true)
                  {%>
                    已完成
                    <%}
                  else
                  {%>
                  <span>暂停中</span>
                    <%} %>
                </td>
                <td class="pscore">
                    得分
                    <%: item.Score %>/<%: item.TotalScore %>,<%: item.Mark %>
                </td>
                <td class="paction">
                    
                    <%if (item.isFinish == true)
                      { %>
                      <%if (item.TestType != 8 && item.TestType != 9 && item.TestType != 10)
                        {%>
                      <%: Html.ActionLink("再次练习", "Exerciser", new { id = item.TestID })%><%: Html.ActionLink("查看详细报告", "AnswerInfo", new { id = item.TestID })%><%: Html.ActionLink("查看答案", "Answer", new { id = item.TestID })%>
                      <%} %>
                      <%else
                        { %>
                      <%: Html.ActionLink("查看详细报告", "AnswerInfo", new { id = item.TestID })%><%: Html.ActionLink("查看班级成绩", "ClassScore", new { id = item.TestID })%>
                      <%} %>
                    <%}
                      else
                      {%>
                      <%:Html.ActionLink("继续练习", "ContinuesPractice", new { TestID = item.TestID })%>
                    <%} %>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
</div>
