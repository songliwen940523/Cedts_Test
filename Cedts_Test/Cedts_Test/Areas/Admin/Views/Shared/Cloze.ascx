<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
    <%for (int i = 0; i < 20; i++)
      {  %>
    <div>
        <fieldset style="width: 1026px">
            <legend>第<%:(i+1) %>小题</legend>预设分数：<input type="text" id="Cloze_scorequestion<%:(i+1) %>"
                name="Cloze_scorequestion<%:(i+1) %>" class="scorequestion" /><br />
            答题估时：<input type="text" id="Cloze_timequestion<%:(i+1) %>" name="Cloze_timequestion<%:(i+1) %>"
                class="timequestion" /><br />
            难度：<select name="Cloze_difficultquestion<%:(i+1) %>" id="Cloze_difficultquestion<%:(i+1) %>"
                class="difficultquestion" style="width: 144px">
                <option>0.1</option>
                <option>0.2</option>
                <option>0.3</option>
                <option>0.4</option>
                <option>0.5</option>
                <option>0.6</option>
                <option>0.7</option>
                <option>0.8</option>
                <option>0.9</option>
                <option>1</option>
            </select>
            <br />
            知识点：<a href="#" class="classa" id="Cloze_a<%:(i+1) %>">请选择知识点</a> <span id="Cloze_span<%:(i+1) %>"
                class="span"></span>
            <input class="hidden" type="hidden" id="Cloze_hidden<%:(i+1) %>" name="Cloze_hidden<%:(i+1) %>" /><br />
            选项： A:<input class="option" type="text" id="Cloze_Option<%:(i*4+1) %>" name="Cloze_Option<%:(i*4+1) %>" /><br />
            B:<input class="option" type="text" id="Cloze_Option<%:(i*4+2) %>" name="Cloze_Option<%:(i*4+2) %>" /><br />
            C:<input class="option" type="text" id="Cloze_Option<%:(i*4+3) %>" name="Cloze_Option<%:(i*4+3) %>" /><br />
            D:<input class="option" type="text" id="Cloze_Option<%:(i*4+4) %>" name="Cloze_Option<%:(i*4+4) %>" /><br />
            答案：&nbsp;
            <select name="Cloze_D<%:(i+1) %>" id="Cloze_D<%:(i+1) %>" style="width: 144px">
                <option>A</option>
                <option>B</option>
                <option>C</option>
                <option>D</option>
            </select>
            <br />
            答案解析：<input type="text" id="Cloze_tip<%:(i+1) %>" name="Cloze_tip<%:(i+1) %>" class="tip" />
        </fieldset>
    </div>
    <%} %>
</div>
