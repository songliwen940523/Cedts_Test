<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Areas.Admin.Models.CEDTS_ItemType>>" %>

<%int i = 0; %>
<% foreach (var item in Model)
   { %>
<%i++; %>
<%:i %>、 试题类型英文名:&nbsp;<%: item.TypeName %>； 试题类型中文名：<%: item.TypeName_CN %>；题型数量：
<input type="text" id='<%:item.ItemTypeID %>' name='<%:item.ItemTypeID %>' class="txt" readonly="readonly" />
<br />
<br />
<% } %>
