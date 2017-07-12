<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<Cedts_Test.Areas.Admin.Models.CEDTS_KnowledgePoints>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    知识点管理首页
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
        function DeleteSort() {
            if (!window.confirm("确定要删除您选择的数据项吗?")) {
                return;
            }
            var checkedItems = CollectCheckItems();

            if (checkedItems == "") {
                alert("请选择要删除的知识点!");
                return;
            }
            $.ajax({
                type: "POST",
                url: "/Knowledge/DeleteSort",
                data: { name: function () { return checkedItems } },
                dataType: "html",
                success: function (data) {
                    if (data == 1) {
                        alert("删除成功!");
                    }
                    else {
                        alert("删除失败!你选中项存在下级,请先删除下级！");
                    }
                    window.location.href = "/Admin/Knowledge/Index";
                }
            });
        }
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("知识点管理首页","Index") %>
    </div>
    <div id="tt" class="easyui-tabs">
        <div id="t1" title="知识能力类" style="width: 260px">
            <table class="tbRecords">
                <tr>
                    <th>
                        <input type="checkbox" id="checkedAll" onclick="CheckAll(this)" />
                    </th>
                    <th>
                        知识能力类名称
                    </th>
                    <th>
                        知识能力类描述
                    </th>
                    <th>
                编辑
            </th>
                </tr>
                <% foreach (var item in Model)
                   { %>
                <tr>
                    <td>
                        <input id="checkItem_<% =item.KnowledgePointID%>" name="checkItem" type="checkbox" />
                    </td>
                    <td>
                        <%: item.Title %>
                    </td>
                    <td>
                        <%: item.Describe %>
                    </td>
                    <td>
                        <%: Html.ActionLink("编辑", "EditSort", new { id=item.KnowledgePointID }) %>
                    </td>
                </tr>
                <% } %>
            </table>
            <p class="actionbar">
                <%: Html.ActionLink("新增", "CreateSort") %><input type="button" id="Delete" value="删除"
                    onclick="DeleteSort()" />
            </p>
        </div>
        <div id="t2" title="知识能力面" style="width: 260px">
            <%:Html.Action("Side") %>
        </div>
        <div id="t3" title="知识能力点" style="width: 260px">
            <%:Html.Action("Point") %>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            var title = $("#type").val();
            if (title != "") {
                $('#tt').tabs('update', {
                    tab: $('#tt').tabs('getTab', title),
                    options: {
                        selected: true,
                        fit:true
                    }
                });
            }
        });
        
    </script>
    <%:Html.Hidden("type",ViewData["type"]) %>
</asp:Content>
