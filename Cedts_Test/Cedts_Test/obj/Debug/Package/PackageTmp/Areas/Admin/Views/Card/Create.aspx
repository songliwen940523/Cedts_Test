<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Card>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create</h2>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>批量生产卡</legend>
        <div class="editor-label" style=" display:none">
            <%: Html.LabelFor(model => model.CardKind) %>：
        </div>
        <div class="editor-field" style=" display:none">
            <%: Html.DropDownList("CardKind", ViewData["CardKind"] as SelectList, new { id = "CardKind" })%>
            <%: Html.ValidationMessageFor(model => model.CardKind) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.CardType) %>：
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("CardType", ViewData["CardType"] as SelectList, new { id = "CardType" })%>
            <%: Html.ValidationMessageFor(model => model.CardType) %>
        </div>
        <div class="editor-label" style=" display:none">
            <%: Html.LabelFor(model => model.Discount) %>：
        </div>
        <div class="editor-field" style=" display:none">
            <%: Html.TextBoxFor(model => model.Discount, 
            new { 
                t_value="",
                o_value="",
                onkeypress=@"if(!this.value.match(/^[\+\-]?\d*?\.?\d*?$/))this.value=this.t_value;else this.t_value=this.value;if(this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?)?$/))this.o_value=this.value",
                onkeyup=@"if(!this.value.match(/^[\+\-]?\d*?\.?\d*?$/))this.value=this.t_value;else this.t_value=this.value;if(this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?)?$/))this.o_value=this.value",
                onblur=@"if(!this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?|\.\d*?)?$/))this.value=this.o_value;else{if(this.value.match(/^\.\d+$/))this.value=0+this.value;if(this.value.match(/^\.$/))this.value=0;this.o_value=this.value}"})%>
            <%: Html.ValidationMessageFor(model => model.Discount) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.PartnerID) %>：
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("PartnerID")%>
            <%: Html.ValidationMessageFor(model => model.PartnerID) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Money) %>：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Money, new { 
                t_value="",
                o_value="",
                onkeypress=@"if(!this.value.match(/^[\+\-]?\d*?\.?\d*?$/))this.value=this.t_value;else this.t_value=this.value;if(this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?)?$/))this.o_value=this.value",
                onkeyup=@"if(!this.value.match(/^[\+\-]?\d*?\.?\d*?$/))this.value=this.t_value;else this.t_value=this.value;if(this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?)?$/))this.o_value=this.value",
                onblur=@"if(!this.value.match(/^(?:[\+\-]?\d+(?:\.\d+)?|\.\d*?)?$/))this.value=this.o_value;else{if(this.value.match(/^\.\d+$/))this.value=0+this.value;if(this.value.match(/^\.$/))this.value=0;this.o_value=this.value}"})%>
            <%: Html.ValidationMessageFor(model => model.Money) %>
        </div>
        <div class="editor-label">
            生成的数量：
        </div>
        <div class="editor-field">
            <input type="text" name="num" id="num" onkeyup="value=value.replace(/\D/g,'')" onafterpaste="value=value.replace(/\D/g,'')" />
        </div>
        <p>
            <input type="submit" value="提交" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返回", "Index") %>
    </div>
</asp:Content>
