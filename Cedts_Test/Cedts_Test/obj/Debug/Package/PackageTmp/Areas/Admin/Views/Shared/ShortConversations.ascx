<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
    <div>
        <div id="soundfile">
            <input type="hidden" id="filename" name="filename" value="" />
            <h3>
                请选择听力文件：
            </h3>
            <input type="file" name="uploadify" id="uploadify" />
            <div id="fileQueue">
            </div>
        </div>
        <fieldset style="width: 400px">
            
            <br />
            题目：&nbsp;&nbsp;
            <input type="text" id="ShortConversations_Question1" name="ShortConversations_Question1"
                class="question" /><br />
            时间间隔：
            <input type="text" class='interval' id="interval_1" name="interval_1" onkeypress="keyPress()" /><br />
            知识点： <a href="#" class="classa" id="ShortConversations_a1">请选择知识点</a><span id="ShortConversations_span1"
                class="span"></span><input class="hidden" type="hidden" id="ShortConversations_hidden1"
                    name="ShortConversations_hidden1" /><br />
            选项：<br />
            A<input type="text" id="ShortConversations_Option1" name="ShortConversations_Option1"
                class="option" /><br />
            B<input type="text" id="ShortConversations_Option2" name="ShortConversations_Option2"
                class="option" /><br />
            C<input type="text" id="ShortConversations_Option3" name="ShortConversations_Option3"
                class="option" /><br />
            D<input type="text" id="ShortConversations_Option4" name="ShortConversations_Option4"
                class="option" /><br />
            答案：&nbsp;
            <select style="width: 144px" id="ShortConversations_D1" name="ShortConversations_D1">
                <option>A</option>
                <option>B</option>
                <option>C</option>
                <option>D</option>
            </select>
            <br />
            答案解析：<input type="text" id="ShortConversations_tip1" name="ShortConversations_tip1"
                class="tip" /><br />
        </fieldset>
    </div>
</div>
