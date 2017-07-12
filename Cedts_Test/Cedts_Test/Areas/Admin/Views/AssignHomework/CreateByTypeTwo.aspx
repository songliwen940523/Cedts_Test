<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	按题型发布作业
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="chooseTask">
    <div class="ceBody">
        <p class="ceInfo">
            针对题型强化练习，您可以选择不同的题型及题量组合来进行练习，设置完成后，点击右下角的开始按钮进行练习。
        </p><br />
        <div class="itemTypeCustomize">            
            <div class="partType">
                <h3>
                    试题类型：快速阅读 <span class="en">Reading Comprehension Skimming and Scanning</span>
                </h3>
                <ul class="itemList">
                    <li>
                        <label>快速阅读 <span class="en">Skimming and Scanning</span>：</label>
                        共
                            <select id="SspcChoice">
                                <option selected="selected" value="0">0</option>
                                <option value="1">1</option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                                <option value="6">6</option>
                                <option value="7">7</option>
                                <option value="8">8</option>
                            </select>
                         题
                     </li>
                 </ul>
            </div>
            <div class="partType">
                <h3>
                    试题类型：听力 <span class="en">Listening Comprehension</span>
                </h3>
                <ul class="itemList">
                    <li>
                        <label>短对话听力 <span class="en">Short Conversations</span>：</label>
                        共
                            <select id="SlpoChoice">
                                <option selected="selected" value="0">0</option>
                                <option value="1">1</option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                                <option value="6">6</option>
                                <option value="7">7</option>
                                <option value="8">8</option>
                            </select>
                        题
                    </li>
                    <li>
                        <label>长对话听力 <span class="en">Long Conversations</span>：</label>
                        共
                        <select id="LlpoChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题</li>
                    <li>
                        <label>短文听力理解 <span class="en">Short Passages</span>：</label>
                        共
                        <select id="RlpoChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                    <li>
                        <label>复合型听力 <span class="en">Compound Dictation</span>：</label>
                        共
                        <select id="LpcChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                </ul>
            </div>
            <div class="partType">
                <h3>
                    试题类型：阅读理解 <span class="en">Reading Comprehension (Reading in Depth)</span>
                </h3>
                <ul class="itemList">
                    <li>
                        <label>阅读选词填空 <span class="en">Banked Cloze</span>：</label>
                        共
                        <select id="RpoChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                    <li>
                        <label>阅读选择题型 <span class="en">Multiple Choice</span>：</label>
                        共
                        <select id="RpcChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                    <li>
                        <label>信息匹配题型 <span class="en">Infomation Matching</span>：</label>
                        共
                        <select id="InfoMatch">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                </ul>
            </div>
            <div class="partType">
                <h3>
                    试题类型：完型填空 <span class="en">Cloze</span></h3>
                <ul class="itemList">
                    <li>
                        <label>完型填空 <span class="en">Cloze</span>：</label>
                        共
                        <select id="CpChoice">
                            <option selected="selected" value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                        </select>
                        题
                    </li>
                </ul>
            </div>
            <p>
                您也可以<a href="#link" class="btnStandard">载入标准试卷设置</a>，或 <a href="#link" class="btnClear">
                    清空所有设置</a>
            </p>
        </div>
    </div>
    <div class="ceFoot">
        <a href="#link" class="btnAssign">发布</a>
    </div>
    <div id="pp">
    </div>
 </div>
 <script language="javascript" type="text/javascript">
     $('.itemTypeCustomize .btnStandard').click(function (e) { //载入默认试题设置
         e.preventDefault();
         var standard = [0, 8, 2, 3, 1, 1, 2, 1, 0]; //注意：这是value数组，不是index数组，虽然此处它们相同
         $.each($('.itemTypeCustomize select'), function (i) {
             $(this).val(standard[i]);
         });
     });

     $('.itemTypeCustomize .btnClear').click(function (e) { //清空设置按钮
         e.preventDefault();
         $.each($('.itemTypeCustomize select'), function (i) {
             $(this).val(0);
         });
     });
     $('.btnAssign').click(function (e) {
         e.preventDefault();
         var sspc = Number($("#SspcChoice").val());
         var slpo = Number($("#SlpoChoice").val());
         var llpo = Number($("#LlpoChoice").val());
         var rlpo = Number($("#RlpoChoice").val());
         var lpc = Number($("#LpcChoice").val());
         var rpo = Number($("#RpoChoice").val());
         var rpc = Number($("#RpcChoice").val());
         var cp = Number($("#CpChoice").val());
         var im = Number($("#InfoMatch").val());
         if (slpo == "0" && sspc == "0" && llpo == "0" && rlpo == "0" && lpc == "0" && rpo == "0" && rpc == "0" && im == "0" && cp == "0") {
             alert("请选择题数！");
         }
         else {
             opentt();
             $.post("/AssignHomework/CreateByTypeTwo", { Sspc: sspc, Slpo: slpo, Llpo: llpo, Rlpo: rlpo, Lpc: lpc, Rpo: rpo, Rpc: rpc, Im: im, Cp: cp }, function (data) {
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
 </script>
</asp:Content>
