<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ExaminationItem>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    试题管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var array = new Array();
            array = $("#hidden").val().split(',');
            if (array[0] != "") {
                $("#txtSearch").val(array[0]);
            }
            $("#condition").change(function () {
                if ($(this).val() == "1") {
                    $("#txtSearch").hide();
                    $("#txtSearch").val($("#itemType").val());
                    $("#itemType").show();
                } else {
                    $("#txtSearch").val("");
                    $("#txtSearch").show();
                    $("#itemType").hide();
                }
            })
            $("#condition").children("option").each(function () {
                if ($(this).val() == array[1]) {
                    $(this).attr("selected", "selected");
                    $("#condition").change();
                }
            });
            $("#itemType").children("option").each(function () {
                if ($(this).val() == array[0]) {
                    $(this).attr("selected", "selected");
                }
            });
            $("#itemType").change(function () {
                $("#txtSearch").val($(this).val());
            })
            $(".label").click(function () {
                if (!window.confirm("确定要删除您选择的数据项吗?")) {
                    return;
                }

                var id = $(this).attr("id").split("_")[1];
                $.ajax({
                    type: "POST",
                    url: "/Admin/Examination/DeleteItem/",
                    data: { id: function () { return id } },
                    dataType: "html",
                    success: function (data) {
                        if (data == 1) {
                            alert("删除成功!");
                        }
                        else {
                            alert("删除失败!");
                        }
                        window.location.reload();
                    }
                });
            });
        });
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试题管理首页","Index") %>
    </div>
    <div class="divfloat">
        <%  using (Html.BeginForm())
            { %>
        <div id="Search">
            查询条件：<select id="condition" name="condition">
                <option value="1">题型</option>
                <option value="2">所属试卷</option>
            </select>
            <input type="text" value="快速阅读" id="txtSearch" name="txtSearch" style="display:none" />
            <select id="itemType" name="itemType">
            <option value="快速阅读">快速阅读</option>
            <option value="短对话听力">短对话听力</option>
            <option value="长对话听力">长对话听力</option>
            <option value="短文听力理解">短文听力理解</option>
            <option value="复合型听力">复合型听力</option>
            <option value="阅读选词填空">阅读选词填空</option>
            <option value="阅读选择题型">阅读选择题型</option>
            <option value="完型填空">完型填空</option>
            </select>
            <input type="submit" id="btnSearch" value="查 询" />
        </div>
        <%} %>
        <div>
            <input type="hidden" id="hidden" name="hidden" value="<%:TempData["info"] %>" /></div>
        <div id="divpages">
            <table class="tbRecords">
                <tr>
                    <th>
                        编号
                    </th>
                    <th>
                        类型
                    </th>
                    <th>
                        难度
                    </th>
                    <th>
                        估时
                    </th>
                    <th>
                        预设分数
                    </th>
                    <th>
                        保存时间
                    </th>
                    <th>
                        上次更新时间
                    </th>
                    <th>
                        使用次数
                    </th>
                    <th>
                        所属试卷
                    </th>
                    <th>
                        编辑
                    </th>
                    <th>
                        删除
                    </th>
                </tr>
                <%int i = 0; %>
                <% foreach (var item in Model)
                   { %>
                <%i++; %>
                <tr>
                    <td>
                        <%: Html.Encode( i.ToString()) %>
                    </td>
                    <td>
                        <%: item.ItemName %>
                    </td>
                    <td>
                        <%: String.Format("{0:F}", item.Difficult) %>
                    </td>
                    <td>
                        <%: item.Duration %>
                    </td>
                    <td>
                        <%: item.Score %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.SaveTime) %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.UpdateTime) %>
                    </td>
                    <td>
                        <%: item.Count %>
                    </td>
                    <td>
                        <%: item.PaperName %>
                    </td>
                    <td>
                        <%: Html.ActionLink("编辑", "EditListen", new { id=item.AssessmentItemID }) %>
                    </td>
                    <td>
                        <a href="#" class="label" id="Delete_<%=item.AssessmentItemID %>">删除</a>
                    </td>
                </tr>
                <% } %>
            </table>
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
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" })%>
        </div>
    </div>
    <p class="actionbar">
        <%: Html.ActionLink("新增", "Create") %>
    </p>
</asp:Content>
