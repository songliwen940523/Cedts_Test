<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<p class="ceInfo">
    系统根据您以往的练习，针对你的知识点弱项自动生成一份试卷，输入题量后，点击右下角的开始按钮进行练习。</p>
<h3>
    练习试卷题目：<input type="text" id="CeType4Title" name="CeType4Title" class="Title" /><span
        id="CeType4Prompt" style="color: Red;"></span></h3>
<div>
    请输入练习的题目数量（范围：1-30）：<input type="text" name="totalnum" id="txt" value="" onkeyup="value=value.replace(/\D/g,'')"
        onafterpaste="value=value.replace(/\D/g,'')" /><br />
    <input id="BadKnowledge" type="button" value="下一步" style="display: none;" />
</div>
