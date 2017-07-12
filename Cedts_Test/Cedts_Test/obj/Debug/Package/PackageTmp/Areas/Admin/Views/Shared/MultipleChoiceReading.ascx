<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
    <div>
        请选择题数：<select name="MultipleChoice_selectnum" id="MultipleChoice_selectnum" class="selectnum"
            style="width: 208px; margin-bottom: 0px;">
            <option>1</option>
            <option>2</option>
            <option>3</option>
            <option>4</option>
            <option>5</option>
        </select>
        <br />
        <br />
        <br />
        <fieldset>
            <legend>第1小题</legend>预设分数：<input type="text" id="MultipleChoice_scorequestion1" name="MultipleChoice_scorequestion1"
                class="scorequestion" /><br />
            答题估时：<input type="text" id="MultipleChoice_timequestion1" name="MultipleChoice_timequestion1"
                class="timequestion" /><br />
            难度：<select name="MultipleChoice_difficultquestion1" id="MultipleChoice_difficultquestion1"
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
            题目：&nbsp;<input class="question" type="text" id="MultipleChoice_Question1" name="MultipleChoice_Question1" /><br />
            知识点：<a href="#" class="classa" id="MultipleChoice_a1">请选择知识点</a><span id="MultipleChoice_span1"
                class="span"></span><input class="hidden" type="hidden" id="MultipleChoice_hidden1"
                    name="MultipleChoice_hidden1" /><br />
            选项：A:<input class="option" type="text" id="MultipleChoice_Option1" name="MultipleChoice_Option1" /><br />
            B:<input class="option" type="text" id="MultipleChoice_Option2" name="MultipleChoice_Option2" /><br />
            C:<input class="option" type="text" id="MultipleChoice_Option3" name="MultipleChoice_Option3" /><br />
            D:<input class="option" type="text" id="MultipleChoice_Option4" name="MultipleChoice_Option4" /><br />
            答案：
            <select style="width: 144px" name="MultipleChoice_D1" id="MultipleChoice_D1">
                <option>A</option>
                <option>B</option>
                <option>C</option>
                <option>D</option>
            </select>
            <br />
            答案解析：<input class="tip" type="text" id="MultipleChoice_tip1" name="MultipleChoice_tip1" />
        </fieldset>
    </div>
    <div id="MultipleChoice_divi">
    </div>
</div>
