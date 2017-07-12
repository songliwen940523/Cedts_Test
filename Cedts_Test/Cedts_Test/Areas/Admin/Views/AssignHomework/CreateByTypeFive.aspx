<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.SelectAssessmentItem>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	自主选题
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div style="padding-left: 20px;padding-top: 10px;">
    <%if (Model.sspcList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">快速阅读</h2>
      <div class="selectitemdiv">      
<%--      <%:Html.Action("SspcList", "AssignHomework")%>--%>
        
        <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.sspcList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
    <%} %>
    <%if (Model.slpoList.Count > 0)
      {%>      
<%--      <%:Html.Action("SlpoList", "AssignHomework")%>--%>
      <h2 style="margin: 10px 0; clear: both;">短对话听力</h2>
      <div class="selectitemdiv">
      
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.slpoList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
    <%} %>
    <%if (Model.llpoList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">长对话听力</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.llpoList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("LlpoList", "AssignHomework")%>--%>
    <%} %>
    <%if (Model.rlpoList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">短文听力理解</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.rlpoList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("RlpoList", "AssignHomework")%>--%>
    <%} %>
    <%if (Model.lpcList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">复合型听力</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.lpcList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("LpcList", "AssignHomework")%>--%>
    <%} %>
    <%if (Model.rpcList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">阅读选词填空</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.rpcList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("RpcList", "AssignHomework")%>--%>
    <%} %>
    <%if (Model.rpoList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">阅读选择题型</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.rpoList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("RpoList", "AssignHomework")%>--%>
    <%} %>

    <%if (Model.infoMatList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">阅读信息匹配</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.infoMatList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("RpcList", "AssignHomework")%>--%>
    <%} %>

    <%if (Model.cpList.Count > 0)
      {%>
      <h2 style="margin: 10px 0; clear: both;">完型填空</h2>
      <div class="selectitemdiv">
      <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.cpList)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
      </div>
<%--      <%:Html.Action("CpList", "AssignHomework")%>--%>
    <%} %>
    <br />
    <div style="clear:both;">
        <input type="submit" class="makepaper" value="组卷" />
    </div>
    <div id="pp">
    </div>
</div>
<script type="text/javascript">
    $(function () {
        $("form").validate({});
        $(".makepaper").click(function () {
            var flag = false;
            var ItemIDArray = new Array();
            $(":checkbox").each(function () {
                if ($(this).attr("checked") == true) {
                    flag = true;
                    ItemIDArray.push($(this).val());
                }
            })
            if (!flag) {
                alert("请至少选择一个试卷名称！");
                return false;
            }
            else {
                var AssessmentID = ItemIDArray.join(',');
                opentt();
                $.post("/AssignHomework/CreateByTypeFive", { AssessmentID: AssessmentID }, function (data) {
                    if (data == "0") {
                        alert("发布失败！");
                    }
                    else {
                        alert("发布成功！");
                    }
                    window.location.href = "/Admin/AssignHomework/Index";
                });
            }
        });
        function opentt() {
            $('#pp').dialog('open');
        }
        $('#pp').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true,
            onBeforeOpen: function () {
                $("#pp").empty().append("<h1><center>正在组卷中，请稍后~~~！<br /><img src='../../../../Images/06.gif' /></center></h1>");
            },
            onBeforeClose: function () { }
        });
        function closett() {
            $('#pp').dialog('close');
        }
        $(".selectitemdiv").each(function () {
            if ($(this).height() > 300) {
                $(this).height(300);
                $(this).css("overflow", "auto");
            }
        })
    });
</script>
</asp:Content>
