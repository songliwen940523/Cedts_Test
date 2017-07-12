<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	按知识点发布作业
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var s1;
        $.post("/AssignHomework/SelectKnowledge", { uperID: "" }, function (data) {
            var select1 = "<div><select name='slect1' class='select1'>";
            var option = "<option>--请选择--</option>";
            $.each(data, function (i, n) {
                option += "<option value=" + n.KnowledgeID + ">" + n.Title + "</option>";
            });
            select1 = select1 + option + "</select>";
            var select2 = "<span><select name='slect2' class='select2'><option>--请选择--</option></select>";
            var numstring = "请选择题目数量：";
            numstring += "<select name='slect3' class='select3'>";
            var numoptions = "<option>1</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option><option>12</option><option>13</option><option>14</option><option>15</option><option>16</option><option>17</option><option>18</option><option>19</option><option>20</option>";
            numstring += numoptions + "</select></span>";
            var btnD = "<input type='button' class='btnDel' value='移除' /></div>";
            s1 = select1 + "&nbsp;&nbsp;&nbsp;" + select2 + numstring + btnD;
            $("#div1").append(s1);
        }, "json");
        $(function () {
            $.ajaxSetup({
                async: false
            });
            $(".select1").live("change", function () {
                var temp = "";
                var id = $(this).val();

                if (id != "--请选择--") {
                    $(this).next('span').remove();
                    $.post("/AssignHomework/SelectKnowledge", { uperID: id }, function (data) {
                        var select2 = "<span><select name='slect2' class='select2'>";
                        var option = "<option>--请选择--</option>";
                        $.each(data, function (i, n) {
                            var istrue = true;
                            $(".select2").find('option:selected').each(function () {
                                if ($(this).val() == n.KnowledgeID) {
                                    istrue = false;
                                }
                            });
                            if (istrue)
                                option += "<option value=" + n.KnowledgeID + ">" + n.Title + "</option>";
                        });
                        select2 += option + "</select>";
                        var numstring = "请选择题目数量：";
                        numstring += "<select name='slect3' class='select3'>";
                        var numoptions = "<option>1</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option><option>12</option><option>13</option><option>14</option><option>15</option><option>16</option><option>17</option><option>18</option><option>19</option><option>20</option>";
                        numstring += numoptions + "</select></span>";
                        temp = select2 + numstring;
                    }, "json");
                    $(this).parent("div").append(temp);
                    $(this).parent("div").children("span").insertAfter($(this));
                }
                else {
                    $(this).next('span').children("select").get(0).options.length = 1;
                }
            });

            $("#btn").click(function () {
                $("#div1").append(s1);
            })

            $(".btnDel").live("click", function () {
                $(this).parent("div").empty().remove();
            });

            $('.btnAssign').click(function (e) {
                $.ajaxSetup({
                    async: true
                });
                e.preventDefault();
                var select2 = "";
                var select3 = "";
                $(".select2").each(function () {
                    if ($(this).val() == "" || $(this).val() == "--请选择--") {
                        return false;
                    }
                    select2 += $(this).val() + ",";
                    select3 += $(this).next("select").val() + ",";
                })
                
                if (select2 == "" || select3 == "") {
                    closett();
                    alert("请选择知识点！");
                    return false;
                }
                var totle = select3.split(',');
                var s = 0;
                for (var i = 0; i < totle.length - 1; i++) {
                    s += Number(totle[i]);
                }
                if (s >= 150) {
                    alert("题目数量不能超过150！");
                }
                else {
                    opentt();
                    $.post("/AssignHomework/CreateByTypeThree", { select2: select2, select3: select3 }, function (data) {
                        if (data == "0") {
                            alert("发布失败！");
                        }
                        else {
                            alert("发布成功！");
                        }
                        window.location.href = "/Admin/AssignHomework/Index";
                    },"text");
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
        })
    </script>
<div class="chooseTask">
    <div class="ceBody">
        <p class="ceInfo">
            您可以添加不同的知识点进行强化练习，设置完成后，点击右下角的发布按钮进行练习。</p>
        <br />
        <input type="button" id="btn" value="添加知识点" />
        <div id="div1">
        </div>
        <input id="SelectByKnowledge" type="button" value="下一步" style="display: none;" />
    </div>
    <div class="ceFoot">
        <a href="#link" class="btnAssign">发布</a>
    </div>
    <div id="pp">
    </div>
</div>



</asp:Content>
