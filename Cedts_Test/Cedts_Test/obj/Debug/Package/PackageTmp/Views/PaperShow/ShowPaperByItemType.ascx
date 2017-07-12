<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Models.CEDTS_PartType>>" %>
<p class="ceInfo">
    针对题型强化练习，您可以选择不同的题型及题量组合来进行练习，设置完成后，点击右下角的开始按钮进行练习。</p>
<div class="itemTypeCustomize">
    <% using (Html.BeginForm())
       { %>
    <%: Html.ValidationSummary(true)%>
    <% foreach (var item in Model)
       { %>
    <%switch (item.PartTypeID)
      {%>
    <%case 1: %>
    <div class="partType">
        <h3>
            试题类型：<%: item.TypeName_CN%>
            <span class="en">
                <%: item.TypeName%></span></h3>
        <ul class="itemList">
            <li>
                <label>
                    <span class="en"></span>:</label>共
                <select name="1_1">
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
    <%break;%>
    <%case 2: %>
    <div class="partType">
        <h3>
            试题类型：<%: item.TypeName_CN%>
            <span class="en">
                <%: item.TypeName%></span></h3>
        <ul class="itemList">
            <li>
                <label>
                    短对话听力 <span class="en">Short Conversations</span>：</label>
                共
                <select name="2_1">
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
                <select name="2_2">
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
                <select name="2_3">
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
                <select name="2_4">
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
    <%break; %>
    <%case 3: %>
    <div class="partType">
        <h3>
            试题类型：<%: item.TypeName_CN%>
            <span class="en">
                <%: item.TypeName%></span></h3>
        <ul class="itemList">
            <li>
                <label>
                    阅读选词填空 <span class="en">Banked Cloze</span>：</label>
                共
                <select name="3_1">
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
                <select name="3_2">
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
    <%break; %>
    <%case 4: %>
    <div class="partType">
        <h3>
            试题类型：<%: item.TypeName_CN%>
            <span class="en">
                <%: item.TypeName%></span></h3>
        <ul class="itemList">
            <li>
                <label>
                    完型填空 <span class="en">Cloze</span>：</label>
                共
                <select name="4_1">
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
    <%break; %>
    <%} %>
    <% } %>
    <p>
        您也可以<a href="#link" class="btnStandard">载入标准试卷设置</a>，或 <a href="#link" class="btnClear">
            清空所有设置</a></p>
            <div style="display:none"> <input id="Itembtn" type="submit" value="下一步" /></div>
   
    <%} %>
</div>
