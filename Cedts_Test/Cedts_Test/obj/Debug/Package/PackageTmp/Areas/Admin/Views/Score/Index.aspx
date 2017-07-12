<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	成绩整体情况
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
                <div class="needspace">请选择时间： &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input class="Wdate" id="CalenderBegin" name="CalenderBegin" type="text" onclick="WdatePicker()" />&nbsp;&nbsp;至&nbsp;&nbsp;<input class="Wdate" id="CalenderEnd" name="CalenderEnd" type="text" onclick="WdatePicker()" />&nbsp;&nbsp;<font color="red">日期格式：YYYY-MM-DD</font></div><br />     
                <div class="needspace">请选择班级： &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%:Html.DropDownList("ClassIDList") %></div><br />        
                <div class="needspace">请选择练习类型： &nbsp;&nbsp;
                <% 
                   List<SelectListItem> FirstLevelList = new List<SelectListItem>
                   {
                       new SelectListItem {Text = "请选择", Value = "0", Selected = true},
                       new SelectListItem {Text =  "所有练习", Value = "所有练习"},
                       new SelectListItem {Text =  "布置练习", Value = "布置练习"},
                       new SelectListItem {Text =  "自主练习", Value = "自主练习"}
                   };
                     %>
                <%: Html.DropDownList("FirstLevel", FirstLevelList)%>        
                </div><br /><br />
                <p class="needspace"><input type="submit" value="确定" id = "submit"/>
                </p><br />
            </fieldset>
            <%} %>
        </div>
    </form>

    <script type="text/javascript">
        $(function () {
            var d = new Date();
            var dd = d.format("yyyy-MM-dd");
            $(".Wdate").val(dd);
            $("form").validate({});
            $("#submit").click(function () {
                if ($("#CalenderBegin").val() == "" || $("#CalenderEnd").val() == "") {
                    alert("日期不能为空！");
                    return false;
                }
                if ($("#FirstLevel").val() == "0") {
                    alert("请选择练习类型！");
                    return false;
                }
                if (dateCompare($("#CalenderBegin").val(), $("#CalenderEnd").val())) {
                    alert("开始日期不能大于结束日期！");
                    return false;
                }

            });
            function dateCompare(startdate, enddate) {
                var arr = startdate.split("-");
                var starttime = new Date(arr[0], arr[1], arr[2]);
                var starttimes = starttime.getTime();
                var arrs = enddate.split("-");
                var lktime = new Date(arrs[0], arrs[1], arrs[2]);
                var lktimes = lktime.getTime();
                if (starttimes > lktimes) {
                    return true;
                }
                else
                    return false;
            }
        });
    </script>
</asp:Content>

