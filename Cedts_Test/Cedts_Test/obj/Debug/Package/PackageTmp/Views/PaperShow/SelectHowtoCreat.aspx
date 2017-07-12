<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../../Content/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Content/themes/icon.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../../Scripts/jquery-1.4.1.min.js"></script>
    <script src="../../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            $("#Next").click(function () {
                var Num = $('input:radio[name="action"]:checked').val();
                if (Num == null) {
                    alert("请选中一种组卷方式！");
                }
                else {
                    if (Num != "1") {
                        open1();
                    }
                    else {
                        $("#submit").click();
                    }
                }
            });


            $('#dd').dialog({
                title: '对话框',
                modal: true,
                closed: true,

                buttons: [{
                    text: '确认',
                    iconCls: 'icon-ok',
                    handler: function () {
                        if ($("#Name").val() == "") {
                            $("#Prompt").html("*题目不能为空");
                        }
                        else {
                            $('#dd').dialog('close');
                            $("#Title").val($("#Name").val());
                            $('#submit').click();
                        }
                    }
                }, {
                    text: '取消',
                    handler: function () {
                        $('#dd').dialog('close');

                    }
                }]
            });
        });
        function open1() {
            $('#dd').dialog('open');
        }
        function close1() {
            $('#dd').dialog('close');
        }

    </script>
    <div>
        <center>
        <div>
            <h2>
                组卷方式选择</h2>
            <br />
        </div>
        <% using (Html.BeginForm())
           { %>
        <div>
            <%:Html.RadioButton("action", "1")%>4，6级试卷选择练习（历年4，6级真题）
            <%:Html.RadioButton("action", "2")%>按照题型组卷（针对题型强化练习）
            <%:Html.RadioButton("action", "3")%>随机生成一份试卷（系统自动生成一份模拟试卷练习）
            <%:Html.RadioButton("action", "4")%>按照知识点选择练习（针对知识点强化练习）
            <%:Html.RadioButton("action", "5")%>按照知识点弱项练习（系统根据你以往的练习针对你的知识点弱项生成一份试卷）
            <%:Html.RadioButton("action", "6")%>按照得分最大化练习（针对以往考题统计，给出考察频率较高的知识点，组成一份试卷）
            <input type="hidden" id="Title" name="Title" />
        </div>
        <div></div>
        <input id="Next" type="button" value="下一步" />
        <input type="submit" id="submit" style="display:none;"/>
        <%} %>
    </center>
    </div>
    <div style="margin-bottom: 10px;">
        <div id="dd" icon="icon-save" style="padding: 5px; width: 400px; height: 200px;">
            
            试卷名称：<input type="text" id="Name" style="width:200px;" /><span id="Prompt" style="color: Red;"></span>
           
        </div>
    </div>
</asp:Content>
