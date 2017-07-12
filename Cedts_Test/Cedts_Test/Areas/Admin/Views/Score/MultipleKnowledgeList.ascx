<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Areas.Admin.Models.WrongKnowledgeInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%if(Model.Count() > 0)
  { %>
<table class="tbRecords" cellspacing="0">
    <caption>易错知识点列表</caption>
    <thead>
        <tr>
            <th class="pname">
                知识点名称
            </th>
            <th class="pname">
                知识点题数
            </th>
            <th class="pname">
                得分率
            </th>
        </tr>
    </thead>    
    <tbody>
        <% foreach (var item in Model)
            { %>
        <tr>
            <td class="pname">
                <%: item.KnowledgeName%>
            </td>
            <td class="pname">
                <%: item.TotalNum%>
            </td>
            <td class="pname">
                <%: item.CorrectRate.ToString("0.0")%>%
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
<%} %>

