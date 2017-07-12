<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
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
    听力题干时间间隔：<input type="text" class='interval' id="interval" name="interval" onkeypress="keyPress()" /><br />
    请选择题数：<select name="Complex_selectnum" id="Complex_selectnum" class="selectnum" style="width: 142px">
        <option>1</option>
        <option>2</option>
        <option>3</option>
        <option>4</option>
        <option>5</option>
        <option>6</option>
        <option>7</option>
        <option>8</option>
        <option>9</option>
        <option>10</option>
    </select>
    <br />
    <fieldset style="width: 400px">
        <legend>第1题</legend>预设分数：<input type="text" id="Complex_scorequestion1" name="Complex_scorequestion1"
            class="scorequestion" /><br />
        答题估时：<input type="text" id="Complex_timequestion1" name="Complex_timequestion1" class="timequestion" /><br />
        难度：<select name="Complex_difficultquestion1" id="Complex_difficultquestion1" class="difficultquestion"
            style="width: 144px">
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
        答案：&nbsp;<input type="text" id="Complex_textanswer1" name="Complex_textanswer1" class="answer" /><br />
        知识点：<a href="#" class="classa" id="Complex_a1">请选择知识点</a><span id="Complex_span1"
            class="span"></span><input class="hidden" type="hidden" id="Complex_hidden1" name="Complex_hidden1" /><br />
        答案解析：<input type="text" id="Complex_tip1" name="Complex_tip1" class="tip" /><br />
    </fieldset>
    <div id="Complex_divi">
    </div>
</div>
