<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Models.CEDTS_Paper>>" %>
<p class="ceInfo">
    您可以从历年四级真题中选择一份试卷，然后点击右下角的开始按钮进行练习。</p>
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
