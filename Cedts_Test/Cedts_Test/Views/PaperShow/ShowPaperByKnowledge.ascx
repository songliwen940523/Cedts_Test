<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript">
    var s1;
    $.post("/papershow/SelectKnowledge", { uperID: "" }, function (data) {
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
                $.post("/papershow/SelectKnowledge", { uperID: id }, function (data) {
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
    })
    
</script>
<p class="ceInfo">
    您可以添加不同的知识点进行强化练习，设置完成后，点击右下角的开始按钮进行练习。</p>
<h3>
    练习试卷题目：
    <input type="text" id="CeType3Title" name="CeType3Title" class="Title" /><span id="CeType3Propmt"
        style="color: Red;"></span></h3>
<input type="button" id="btn" value="添加知识点" />
<div id="div1">
</div>
<input id="SelectByKnowledge" type="button" value="下一步" style="display: none;" />