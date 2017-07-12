<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
    <div>
        <br />
        <%string[] str = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" }; %>
        选项:
        <%for (int i = 0; i < 15; i++)
          { %>
        <%:str[i] %><input class="option" type="text" id="BankedCloze_Option<%:(i+1).ToString() %>"
            name="BankedCloze_Option<%:(i+1).ToString() %>" /><br />
        <%} %>
        <%for (int j = 0; j < 10; j++)
          { %>
        <fieldset>
            <legend>第<%:(j+1).ToString() %>小题</legend>预设分数：<input type="text" id="BankedCloze_scorequestion<%:(j+1).ToString() %>"
                name="BankedCloze_scorequestion<%:(j+1).ToString() %>" class="scorequestion" /><br />
            答题估时：<input type="text" id="BankedCloze_timequestion<%:(j+1).ToString() %>" name="BankedCloze_timequestion<%:(j+1).ToString() %>"
                class="timequestion" /><br />
            难度：<select name="BankedCloze_difficultquestion<%:(j+1).ToString() %>" id="BankedCloze_difficultquestion<%:(j+1).ToString() %>"
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
            答案:&nbsp;<input class="anwser" type="text" id="BankedCloze_textanswer<%:(j+1).ToString() %>"
                name="BankedCloze_textanswer<%:(j+1).ToString() %>" /><br />
            知识点：<a href="#" class="classa" id="BankedCloze_a<%:(j+1).ToString() %>">请选择知识点</a><span
                id="BankedCloze_span<%:(j+1).ToString() %>" class="sapn"></span><input class="hidden"
                    type="hidden" id="BankedCloze_hidden<%:(j+1).ToString() %>" name="BankedCloze_hidden<%:(j+1).ToString() %>" /><br />
            答案解析：<input class="tip" type="text" id="BankedCloze_tip<%:(j+1).ToString() %>" name="BankedCloze_tip<%:(j+1).ToString() %>" /><br />
        </fieldset>
        <%} %>
    </div>
</div> 