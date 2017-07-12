<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Card>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    充值卡管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#CKind").change(function () {
                $("#btnQuery").click();
            })

            $("#CType").change(function () {
                $("#btnQuery").click();
            })

            $("#AState").change(function () {
                $("#btnQuery").click();
            })
            if ($("#hidden").val() != "") {
                array = $("#hidden").val().split(',');

                $("#CKind").children("option").each(function () {
                    if ($(this).val() == array[0]) {
                        $(this).attr("selected", "selected");
                    }
                });
                $("#CType").children("option").each(function () {
                    if ($(this).val() == array[1]) {
                        $(this).attr("selected", "selected");
                    }
                });
                $("#AState").children("option").each(function () {
                    if ($(this).val() == array[2]) {
                        $(this).attr("selected", "selected");
                    }
                });
                $("#SCondition").children("option").each(function () {
                    if ($(this).val() == array[3]) {
                        $(this).attr("selected", "selected");
                    }
                });
                $("#TCondition").val(array[4]);
            }

            $("label").click(function () {
                if (!window.confirm("确定要删除您选择的数据项吗?")) {
                    return;
                }

                var id = $(this).attr("id").split("_")[1];
                $.ajax({
                    type: "POST",
                    url: "/Admin/Card/DeleteCard/",
                    data: { id: function () { return id } },
                    dataType: "json",
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
        })
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("充值卡管理首页","Index") %>
    </div>
    <%  using (Html.BeginForm(null, null, FormMethod.Post, new { @class = "filter" }))
        { %>
    <div>
            <span style=" display:none"> 卡类别：<select id="CKind" name="CKind">
                <option value="4">全部</option>
                <option value="0">实体卡</option>
                <option value="1" selected="selected">虚拟卡</option>
            </select></span>
            卡类型：<select id="CType" name="CType">
                <option value="4">全部</option>
                <option value="0">年卡</option>
                <option value="1">月卡</option>
                <option value="2">次数卡</option>
            </select>
            激活状态：<select id="AState" name="AState">
                <option value="4">全部</option>
                <option value="0">待激活</option>
                <option value="1">已激活</option>
                <option value="2">已过期</option>
            </select>
    </div>
    <div>
        模糊查询条件选择：<select id="SCondition" name="SCondition">
            <option value="0">无</option>
            <option value="1">生产人</option>
            <option value="2">激活人</option>
            <option value="3">卡折扣</option>
            <option value="4">院校</option>
            <option value="5">卡金额</option>
        </select>
        <input type="text" id="TCondition" name="TCondition" />
        <input type="submit" id="btnQuery" value="查询" />
    </div>
    <%} %>
    <hr />
    <input type="hidden" id="hidden" name="hidden" value="<%:TempData["info"] %>" />
    <table class="tbRecords" cellspacing="0" >
        <tr>
            <th class="pname" style="width: 130px;">
                序列号
            </th>
            <%--<th>
                卡类别
            </th>--%>
            <th class="pname" >
                卡类型
            </th>
            <th class="pname" style="width: 60px;">
                创建时间
            </th>
            <th class="pname" style="width: 60px;">
                激活时间
            </th>
            <th class="pname">
                激活状态
            </th>
            <th class="pname">
                有效时间
            </th>
            <th class="pname" style="width: 60px;">
                过期时间
            </th>
            <th class="pname" style="width: 60px;">
                创建人
            </th>
            <th class="pname" style="width: 60px;">
                激活人
            </th>
            <%--<th>
                折扣
            </th>--%>
            <th class="pname" style="width: 100px;">
                院校
            </th>
            <th class="pname">
                原始标价
            </th>
            <th class="pname">
                操作
            </th>
        </tr>
        <% using (Cedts_Entities db = new Cedts_Entities())
           { %>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.SerialNumber %>
            </td>
            <%--<td>
                <% string CardKind = string.Empty;
                   switch (item.CardKind)
                   {
                       case 0: CardKind = "实体卡"; break;
                       case 1: CardKind = "虚拟卡"; break;
                       default: break;
                   }   
                %><%:CardKind%>
            </td>--%>
            <td>
                <% string CardType = string.Empty;
                   switch (item.CardType)
                   {
                       case 0: CardType = "年卡"; break;
                       case 1: CardType = "月卡"; break;
                       case 2: CardType = "次数卡"; break;
                       default: break;
                   }
                %><%:CardType%>
            </td>
            <td>
                <%: String.Format("{0:g}", item.CreateTime) %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.ActivationTime) %>
            </td>
            <td>
                <% string ActivationState = string.Empty;
                   switch (item.ActivationState)
                   {
                       case 0: ActivationState = "待激活"; break;
                       case 1: ActivationState = "已激活"; break;
                       case 2: ActivationState = "已过期"; break;
                       default: break;
                   }
                %>
            </td>
            <td>
                <%: item.EffectiveTime %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.OverdueTime) %>
            </td>
            <td>
                <% int CreateUser = item.CreateUser.Value;
                   string CreateUserName = db.CEDTS_User.Where(p => p.UserID == CreateUser).Select(p => p.UserAccount).FirstOrDefault(); 
                %>
                <%:CreateUserName%>
            </td>
            <td>
                <% 
                   int? ActivationUser = item.ActivationUser;
                   string ActivationUserName = db.CEDTS_User.Where(p => p.UserID == ActivationUser).Select(p => p.UserAccount).FirstOrDefault();                
                %>
                <%:ActivationUserName%>
            </td>
            <%--<td>
                <%: String.Format("{0:F}", item.Discount) %>
            </td>--%>
            <td>
                <% Guid PartnerID = item.PartnerID.Value;
                   string PartnerName = db.CEDTS_Partner.Where(p => p.PartnerID == PartnerID).Select(p => p.PartnerName).FirstOrDefault();
                %>
                <%:PartnerName %>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Money) %>
            </td>
            <td>
                <%if (item.ActivationState == 0)
                  { %>
                <label id="Delete_<%: item.ID %>" style="color: Blue">
                    删除</label>
                <%} %>
            </td>
        </tr>
        <% } %>
        <%} %>
    </table>
    <%if (Model.Count == 0)
      { %>
    <center>
        当前没有充值卡记录！</center>
    <%} %>
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
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" 
})%>
    <p class="actionbar">
        <%: Html.ActionLink("批量生产充值卡", "Create") %>
    </p>
</asp:Content>
