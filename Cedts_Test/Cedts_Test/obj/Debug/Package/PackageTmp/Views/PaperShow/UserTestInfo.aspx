<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    个人练习中心
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(function () {
            var UserName = $("#UserName").val();
            $.post("/PaperShow/RateAnalysis", function (data) {
                $("#rateAnalysis").append(data);
            })
            $.post("/PaperShow/KpAnalysis", function (data) {
                $("#KpAnalysis").append(data);
            })
        })
        $.post("/PaperShow/QuestionAnalysis", function (data) {
            $("#QuestionAnalysis").append(data);
        })
    </script>
    <div class="body wrp">
        <div class="sysGraph">
            <%:Html.Action("UserStatistics","Home")%>
        </div>
        <div class="sysAdvices">
            <h2>
                学习建议</h2>
            <ul class="advices">
                <li>
                    <div id="rateAnalysis">
                    </div>
                    <%--<strong>词汇能力：</strong><%:ViewData["Vocabulary"]%>--%>
                </li>
                <li>
                    <div id="KpAnalysis" style="display: none;">
                    </div>
                    <%-- <strong>语法能力：</strong><%:ViewData["Grammar"]%></p>--%>
                </li>
                <li>
                    <div id="QuestionAnalysis">
                    </div>
                    <%--<p><strong>理解能力：</strong><%:ViewData["Comprehend"]%></p>--%>
                </li>
            </ul>
            <p>
                <a href="#link" class="btnQuickExercise" title="随机生成一份试卷进行练习">快速练习</a><a href="#link"
                    class="btnCustomizeExercise" title="选择一种练习方式">自定练习</a></p>
        </div>
        <div class="sysRecords">
            <h2>
                最近试卷练习列表</h2>
            <!--<%:ViewData["status"] %>-->
            <%:Html.Action("TestRecord","PaperShow") %>
        </div>
        <div class="chooseExercise">
            <div class="close"><a class="jqmClose">x</a></div>
            <div class="ceHead">
                <h2>
                    请选择一种练习方式</h2>
            </div>
            <ul class="ceMenu">
                <li>四级试卷选择练习</li>
                <li>按照题型组卷</li>
                <li>按照知识点选择练习</li>
                <li>按照知识点弱项练习</li>
                <li>按照得分最大化练习</li>
            </ul>
            <div class="ceBody">
                <div class="ceTab" id="CeType1">
                    <%:Html.Action("CeType1","PaperShow") %>
                </div>
                <div class="ceTab" id="CeType2">
                    <p class="ceInfo">
                        针对题型强化练习，您可以选择不同的题型及题量组合来进行练习，设置完成后，点击右下角的开始按钮进行练习。</p>
                    <div class="itemTypeCustomize">
                        <div class="partType">
                            <h3>
                                练习试卷题目：
                                <input type="text" id="CeType2Title" name="CeType2Title" class="Title" /><span id="CeType2Propmt"
                                    style="color: Red;"></span></h3>
                        </div>
                        <div class="partType">
                            <h3>
                                试题类型：快速阅读 <span class="en">Reading Comprehension Skimming and Scanning</span></h3>
                            <ul class="itemList">
                                <li>
                                    <label>
                                        快速阅读 <span class="en">Skimming and Scanning</span>：</label>
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
                                    题</li>
                            </ul>
                        </div>
                        <div class="partType">
                            <h3>
                                试题类型：听力 <span class="en">Listening Comprehension</span></h3>
                            <ul class="itemList">
                                <li>
                                    <label>
                                        短对话听力 <span class="en">Short Conversations</span>：</label>
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
                                    题</li>
                                <li>
                                    <label>
                                        长对话听力 <span class="en">Long Conversations</span>：</label>
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
                                    <label>
                                        短文听力理解 <span class="en">Short Passages</span>：</label>
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
                                    题</li>
                                <li>
                                    <label>
                                        复合型听力 <span class="en">Compound Dictation</span>：</label>
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
                                    题</li>
                            </ul>
                        </div>
                        <div class="partType">
                            <h3>
                                试题类型：阅读理解 <span class="en">Reading Comprehension (Reading in Depth)</span></h3>
                            <ul class="itemList">
                                <li>
                                    <label>
                                        阅读选词填空 <span class="en">Banked Cloze</span>：</label>
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
                                    题</li>
                                <li>
                                    <label>
                                        阅读选择题型 <span class="en">Multiple Choice</span>：</label>
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
                                    题</li>
                                <li>
                                    <label>
                                        阅读信息匹配 <span class="en">Information Matching</span>：</label>
                                    共
                                    <select id="InfoMat">
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
                            </ul>
                        </div>
                        <div class="partType">
                            <h3>
                                试题类型：完型填空 <span class="en">Cloze</span></h3>
                            <ul class="itemList">
                                <li>
                                    <label>
                                        完型填空 <span class="en">Cloze</span>：</label>
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
                                    题</li>
                            </ul>
                        </div>
                        <p>
                            您也可以<a href="#link" class="btnStandard">载入标准试卷设置</a>，或 <a href="#link" class="btnClear">
                                清空所有设置</a></p>
                    </div>
                    <%-- <%:Html.Action("ShowPaperbyItemType","PaperShow") %>--%>
                </div>
                <div class="ceTab" id="CeType3">
                    <%:Html.Action("ShowPaperByKnowledge","PaperShow")%>
                </div>
                <div class="ceTab" id="CeType4">
                    <%:Html.Action("ShowPaperByBadKnowledge","PaperShow")%>
                </div>
                <div class="ceTab" id="CeType5">
                    <p class="ceInfo">
                        针对以往考题统计，给出考察频率较高的知识点，组成一份试卷，点击右下角的开始按钮进行练习。</p>
                    <h3>
                        练习试卷题目：<input type="text" id="CeType5Title" /><span id="CeType5Prompt" style="color: Red;"></span></h3>
                </div>
            </div>
            <div class="ceFoot">
                <a href="#link" class="btnStart">开始</a>
            </div>
        </div>
        <div style="float: right; width: 30%;">
            <input type="hidden" id="UserName" value='<%:ViewData["UserName"]%>' />
            <input type="hidden" id="UserRole" value="<%:ViewData["UserRole"] %>" />
            <input type="hidden" id="PaperID" value="<%:ViewData["PaperID"] %>" />
        </div>
    </div>
    <div id="pp">
    </div>
    <div id="ll" style="color: Red;">
        <center>
            <h3>
                欢迎您使用本系统，在您正式使用本系统之前，我们需要对你的英语水平和知识点掌握情况有一个初步的了解。以下将给您一套测试样卷，请您认真完成。</h3>
            <input type="button" id="Test" value="确定" /></center>
    </div>
</asp:Content>
