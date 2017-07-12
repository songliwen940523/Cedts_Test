<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	成绩详情
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="form1" runat="server">

    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>
         <div class="centralcontent">    
            <fieldset>
                <legend>成绩查看条件选择:</legend>
                <br />   
                <div class="needspace">请选择练习名称： &nbsp;&nbsp;<%:Html.DropDownList("PaperNameList")%></div><br />        
                <div class="needspace">请选择查询内容： &nbsp;&nbsp;
                <% 
                   List<SelectListItem> SelectWayList = new List<SelectListItem>
                   {
                       new SelectListItem {Text = "请选择", Value = "0", Selected = true},
                       new SelectListItem {Text =  "学生成绩", Value = "学生成绩"},
                       new SelectListItem {Text =  "试卷分析", Value = "试卷分析"}
                   };
                     %>
                <%: Html.DropDownList("SelectWay", SelectWayList)%>        
                </div><br /><br />
                <p class="needspace"><input type="submit" value="确定" id = "submit"/>
                </p><br />
            </fieldset>
            <%} %>
        </div>
    </form>
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#submit").click(function () {
                if ($("#SelectWay").val() == "0") {
                    alert("请选择练习类型！");
                    return false;
                }
            });
        });
    </script>
</asp:Content>
