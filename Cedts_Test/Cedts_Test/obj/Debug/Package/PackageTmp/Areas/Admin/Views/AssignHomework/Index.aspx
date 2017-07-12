<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" 
Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	发布作业首页
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript" type="text/javascript">
        //定义 城市 数据数组
        ModArray = new Array();
        ModArray[0] = new Array("0","请选择");
        ModArray[1] = new Array("系统题库", "四级真题|按题型组卷|按知识点组卷");
        ModArray[2] = new Array("个人题库", "选择试卷|自主选题");
        
        function GetSecondLevel(curValue) {
            var i, j, k;
            $("#SecondLevel").empty();
            for (i = 0; i < ModArray.length; i++) {
                if (ModArray[i][0] == curValue) {
                    //得到 当前省 所辖制的 地市
                    var TmpArray = ModArray[i][1].split("|")
                    for (j = 0; j < TmpArray.length; j++) {
                        //填充 城市 下拉选单
                        $("<option value='" + TmpArray[j] + "'>" + TmpArray[j] + "</option>").appendTo($("#SecondLevel"));
                    }
                }
            }
        }        
    </script>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>
    <div id="menu">
    当前位置： =>
    <%:Html.ActionLink("发布作业首页", "Index")%>
    </div>
        
    <fieldset>
        <legend>作业基本信息设置:</legend>
        <div>发布班级： <%:Html.DropDownList("ClassIDList") %></div>
        <div>作业名称： <%:Html.TextBox("TaskName") %></div>
        <div>出题模式： 
        <% 
           List<SelectListItem> FirstLevelList = new List<SelectListItem>
           {
               new SelectListItem {Text = "请选择", Value = "0", Selected = true},
               new SelectListItem {Text =  "系统题库", Value = "系统题库"},
               new SelectListItem {Text =  "个人题库", Value = "个人题库"}
           };
             %>
        <%: Html.DropDownList("FirstLevel", FirstLevelList, new { onChange = "GetSecondLevel(this.options[this.selectedIndex].value)" })%>
        <%: Html.DropDownList("SecondLevel", new List<SelectListItem>(){new SelectListItem { Text = "请选择", Value = "0" }})%>
        </div>
        <p><input type="submit" value="下一步" id = "submit"/></p>
    </fieldset>
    <%} %>

    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#submit").click(function () {
                $("#TaskName").rules("add", {
                    required: true,
                    messages: { required: "名称不能为空！" }
                });
                if ($("#SecondLevel").val() == "0") {
                    alert("请选择出题模式！");
                    return false;
                }
            });
        });
    </script>
</asp:Content>
