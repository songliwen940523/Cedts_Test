<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_User>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    院校管理员管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        // 用来连接字符串，提高字符串的拼接速度   
        function StringBuilder() {
            this.buffer = new Array();
        }
        StringBuilder.prototype.Append = function Append(string) {
            if ((string == null) || (typeof (string) == 'undefined'))
                return;
            if ((typeof (string) == 'string') && (string.length == 0))
                return;
            this.buffer.push(string);
        };
        StringBuilder.prototype.AppendLine = function AppendLine(string) {
            this.Append(string);
            this.buffer.push("\r\n");
        };
        StringBuilder.prototype.Clear = function Clear() {
            if (this.buffer.length > 0) {
                this.buffer.splice(0, this.buffer.length);
            }
        };
        StringBuilder.prototype.IsEmpty = function IsEmpty() {

        };
        StringBuilder.prototype.ToString = function ToString() {
            return this.buffer.join("");
        };
        function CheckAll(obj) {
            if ($("#checkedAll").attr("checked")) {
                $("input[type='checkbox']").attr("checked", true)
            }
            else {
                $("input[type='checkbox']").attr("checked", false)
            }
        }
        //收集被选中的项
        function CollectCheckItems() {
            var allcheckbox = $("input[@type=checkbox][@name=checkItem][checked]");
            var ids = new StringBuilder();
            for (var i = 0; i < allcheckbox.length; i++) {
                var id = $(allcheckbox[i]).attr("id").split("_")[1];
                ids.Append(id);
                if (id != null) {
                    ids.Append(",");
                }
            }
            var strIds = ids.ToString();
            return strIds.substr(0, strIds.length - 1);
        }
        //删除选中信息
        function DeleteUser() {
            if (!window.confirm("确定要删除您选择的数据项吗?")) {
                return;
            }
            var checkedItems = CollectCheckItems();

            if (checkedItems == "") {
                alert("请选择要删除的用户!");
                return;
            }
            $.ajax({
                type: "POST",
                url: "/Admin/Admin/DeleteUser",
                data: { name: function () { return checkedItems } },
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
        }

    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("院校管理员管理首页", "Index")%>
    </div>
    <div class="divfloat">
        <div id="divpages">
            <table class="tbRecords" cellspacing="0" >
                <tr>
                    <th class="pname">
                        <input type="checkbox" id="checkedAll" onclick="CheckAll(this)" />
                    </th>
                    <th class="pname">
                        用户帐号
                    </th>
                    <th class="pname">
                       真实姓名
                    </th>
                    <th class="pname">
                        角色
                    </th>
                    <th class="pname">
                        性别
                    </th>
                    <th class="pname">
                        邮箱地址
                    </th>
                    <th class="pname">
                        联系电话
                    </th>
                    <th class="pname">
                        注册时间
                    </th>
                    <th class="pname">
                        编辑
                    </th>
                </tr>
                <%int i = 0; %>
                <% foreach (var item in Model)
                   { %>
                <%i++; %>
                <tr>
                    <td >
                        <input id="checkItem_<% =item.UserID%>" name="checkItem" type="checkbox" />
                    </td>
                    <td >
                        <%: item.UserAccount %>
                    </td>
                    <td>
                        <%: item.UserNickname %>
                    </td>
                    <td>
                        <%: item.Role %>
                    </td>
                    <td>
                        <%: item.Sex %>
                    </td>
                    <td>
                        <%: item.Email %>
                    </td>
                    <td >
                        <%: item.Phone %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.RegisterTime) %>
                    </td>
                    <td>
                        <%: Html.ActionLink("编辑", "Edit", new { id=item.UserID }) %>
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
        <input type="button" id="Delete" value="删除" onclick="DeleteUser()" />
        <%: Html.ActionLink("新增角色", "Create") %>
        <span class="span"></span>
        <input type="button" id="btn_backup" value="备份所有数据" />
    </p>
    <script type="text/javascript">
        $(function () {
            $("#btn_backup").click(function () {
                $.post("/admin/admin/DbBackup", null, function (data) {
                    alert(data);
                }, "text")
            })

        })
    </script>
</asp:Content>
