<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
    <div>
        选择题数目：<select name="SkimmingAndScanning_selectnum" id="SkimmingAndScanning_selectnum"
            class="selectnum" style="width: 137px">
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
        <br />
        <br />
        <fieldset style="width: 400px">
            <legend>第1小题</legend>预设分数：<input type="text" id="SkimmingAndScanning_scorequestion1"
                name="SkimmingAndScanning_scorequestion1" class="scorequestion" /><br />
            答题估时：<input type="text" id="SkimmingAndScanning_timequestion1" name="SkimmingAndScanning_timequestion1"
                class="timequestion" /><br />
            难度：<select name="SkimmingAndScanning_difficultquestion1" id="SkimmingAndScanning_difficultquestion1"
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
            题目：&nbsp;<input class="question" type="text" id="SkimmingAndScanning_Question1" name="SkimmingAndScanning_Question1" /><br />
            知识点：<a href="#" class="classa" id="SkimmingAndScanning_a1">请选择知识点</a><span id="SkimmingAndScanning_span1"></span>
            <input class="hidden" type="hidden" id="SkimmingAndScanning_hidden1" name="SkimmingAndScanning_hidden1" /><br />
            选项：&nbsp; A:<input class="option" type="text" id="SkimmingAndScanning_Option1" name="SkimmingAndScanning_Option1" /><br />
            B:<input class="option" type="text" id="SkimmingAndScanning_Option2" name="SkimmingAndScanning_Option2" /><br />
            C:<input class="option" type="text" id="SkimmingAndScanning_Option3" name="SkimmingAndScanning_Option3" /><br />
            D:<input class="option" type="text" id="SkimmingAndScanning_Option4" name="SkimmingAndScanning_Option4" /><br />
            答案：&nbsp;<select style="width: 144px" name="SkimmingAndScanning_D1">
                <option>A</option>
                <option>B</option>
                <option>C</option>
                <option>D</option>
            </select>
            <br />
            答案解析：<input type="text" id="SkimmingAndScanning_tip1" name="SkimmingAndScanning_tip1"
                class="tip" />
        </fieldset>
    </div>
    <div id="SkimmingAndScanning_divi">
    </div>
</div>
<div>
    填空题数目：<select name="selectnum1" id="selectnum1" class="selectnum1" style="width: 142px">
        <option>0</option>
        <option>1</option>
        <option>2</option>
        <option>3</option>
        <option>4</option>
        <option>5</option>
    </select>
    <br />
    <br />
</div>
<div id="divb">
</div>
