<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Models.CEDTS_ItemType>>" %>
<%int i = 0; %>
<% foreach (var item in Model)
   { %>
<%i++; %>
<%:i %>、 试题类型英文名:&nbsp;<%: item.TypeName %>； 试题类型中文名：<%: item.TypeName_CN %>
<%: Html.DropDownList(item.PartTypeID.ToString() + "_" + i.ToString(), new List<SelectListItem>
{
    (new SelectListItem() {Text = "0", Value = "0", Selected = true }),
    (new SelectListItem() {Text = "1", Value = "1", Selected = false}),
    (new SelectListItem() {Text = "2", Value = "2", Selected = false}),
    (new SelectListItem() {Text = "3", Value = "3", Selected = false}),
    (new SelectListItem() {Text = "4", Value = "4", Selected = false}),
    (new SelectListItem() {Text = "5", Value = "5", Selected = false}),
    (new SelectListItem() {Text = "6", Value = "6", Selected = false}),
    (new SelectListItem() {Text = "7", Value = "7", Selected = false}),
    (new SelectListItem() {Text = "8", Value = "8", Selected = false})
})%><br /><br />
<% } %>
