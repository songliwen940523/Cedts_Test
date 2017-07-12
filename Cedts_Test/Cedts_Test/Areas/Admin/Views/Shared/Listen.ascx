<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_AssessmentItem>" %>
<div>
    <div>
        <div id="soundfile">
            <input type="hidden" id="filename" name="filename" value="" />
            <h3>
                请选择听力文件：（第一个文件为题目题干，后面的为每小题的文件）
            </h3>
            <input type="file" name="uploadify" id="uploadify" /><div id="fileQueue">
            </div>
        </div>
        听力题干时间间隔：<input type="text" class='interval' id="interval" name="interval" onkeypress="keyPress()" /><br />
        请选择题数：<select name="Listen_selectnum" id="Listen_selectnum" class="selectnum" style="width: 137px">
            <option>1</option>
            <option>2</option>
            <option>3</option>
            <option>4</option>
            <option>5</option>
        </select><br />
        <br />
        <fieldset style="width: 400px">
            <legend>第1小题：</legend>预设分数：<input type="text" id="Listen_scorequestion1" name="Listen_scorequestion1"
                class="scorequestion" /><br />
            答题估时：<input type="text" id="Listen_timequestion1" name="Listen_timequestion1" class="timequestion" /><br />
            难度：<select name="Listen_difficultquestion1" id="Listen_difficultquestion1" class="difficultquestion"
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
            题目：&nbsp;&nbsp;
            <input type="text" id="Listen_Question1" name="Listen_Question1" class="question" /><br />
            时间间隔：
            <input type="text" class='interval' id="interval_1" name="interval_1" onkeypress="keyPress()" /><br />
            知识点：<a href="#" class="classa" id="Listen_a1">请选择知识点</a><span id="Listen_span1" class="span"></span><input
                class="hidden" type="hidden" id="Listen_hidden1" name="Listen_hidden1" /><br />
            选项：<br />
            A<input type="text" id="Listen_Option1" name="Listen_Option1" class="option" /><br />
            B<input type="text" id="Listen_Option2" name="Listen_Option2" class="option" /><br />
            C<input type="text" id="Listen_Option3" name="Listen_Option3" class="option" /><br />
            D<input type="text" id="Listen_Option4" name="Listen_Option4" class="option" /><br />
            答案：&nbsp;
            <select style="width: 144px" id="Listen_D1" name="Listen_D1">
                <option>A</option>
                <option>B</option>
                <option>C</option>
                <option>D</option>
            </select>
            <br />
            答案解析：<input type="text" id="Listen_tip1" name="Listen_tip1" class="tip" /><br />
        </fieldset>
    </div>
    <div id="Listen_divi">
    </div>
</div>
