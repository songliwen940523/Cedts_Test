<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_Paper>" %>
<fieldset>
    <legend>试卷基本信息：</legend>
    <br />
    <center>
        试卷标题：<%: Model.Title %><br />
        试卷类型：<%: Model.Type %><br />
        试卷估时：<%: Model.Duration %>分钟<br />
        试卷难度：<%: String.Format("{0:F}", Model.Difficult) %><br />
        试卷总分：<%: Model.Score %>分<br />
        试卷描述：<%: Model.Description %><br />
    </center>
</fieldset>
