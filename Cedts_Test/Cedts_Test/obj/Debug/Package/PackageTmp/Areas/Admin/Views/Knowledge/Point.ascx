<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Areas.Admin.Models.KnowPoint>>" %>
<%@ Import Namespace="Cedts_Test.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
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
    function DeletePoint() {
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
            url: "/Knowledge/DeletePoint",
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
<table class="tbRecords">
    <tr>
        <th>
            <input type="checkbox" id="checkedAll" onclick="CheckAll(this)" />
        </th>
        <th>
            知识能力点名称
        </th>
        <th>
            所属知识能点面
        </th>
        <th>
            所属知识能力类
        </th>
        <th>
            描述
        </th>
        <th>
            编辑
        </th>
    </tr>
    <% foreach (var item in Model)
       { %>
    <tr>
        <td>
            <input id="checkItem_<% =item.KnowID%>" name="checkItem" type="checkbox" />
        </td>
        <td>
            <%: item.PointName %>
        </td>
        <td>
            <%: item.SideName %>
        </td>
        <td>
            <%: item.SortName %>
        </td>
        <td>
            <%: item.Describle %>
        </td>
        <td>
            <%: Html.ActionLink("编辑", "EditPoint", new { id=item.KnowID }) %>
        </td>
    </tr>
    <% } %>
</table>
<p class="actionbar">
    <%: Html.ActionLink("新增", "CreatePoint") %><input type="button" id="Delete" value="删除"
        onclick="DeletePoint()" />
</p>
