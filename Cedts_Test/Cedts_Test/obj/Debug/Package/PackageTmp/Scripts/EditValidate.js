function check() {
    var istrue = true;
    $(".interval").each(function () {
        if ($(this).val() == "") {
            alert("时间间隔不能为空");
            istrue = false;
            return istrue;
        }
    });
    if (!istrue) {
        return false;
    }
    if ($.trim($("#score").val()) == "") {
        alert("请设置分数！");
        istrue = false;
        return istrue;
    }
    if (!istrue) {
        return false;
    }
    if ($.trim($("#time").val()) == "") {
        alert("请设置做题时间!");
        istrue = false;
        return istrue;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".scorequestion").each(function () {
        if ($(this).val() == "") {
            alert("请设置小题分数");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".timequestion").each(function () {
        if ($(this).val() == "") {
            alert("请设置小题时间");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".difficultquestion").each(function () {
        if ($(this).val() == "") {
            alert("请设置小题难度");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".hidden").each(function () {
        if ($(this).val() == "") {
            alert("知识点不能为空!");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".question").each(function () {
        if ($(this).val() == "") {
            alert("题目内容不能为空。");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".option").each(function () {
        if ($(this).val() == "") {
            alert("选项内容不能为空。");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    if (!$(".answer").each(function () {
        if ($(this).val() == "") {
            alert("答案内容不能为空。");
            istrue = false;
            return istrue;
        }
        return istrue;
    })) {
        return false;
    }
    if (!istrue) {
        return false;
    }
    var num = 0.0;
    $(".scorequestion").each(function () {
        num = num + Number($.trim($(this).val())) * 10;
    });
    if (Number($.trim($("#score").val())) * 10 != num) {
        alert("小题分数总和不等于试题分数，请重新设置分数");
        return false;
    }
    if (!istrue) {
        return false;
    }
    var time = 0;
    $(".timequestion").each(function () {
        time = time + Number($.trim($(this).val())) * 10;
    });
    if (Number($.trim($("#time").val())) * 10 != time) {
        alert("小题时间总和不等于试题时间之和,请重新设置时间");
        return false;
    }
    if (!istrue) {
        return false;
    }
    var difficult = 0.0;
    $(".difficultquestion").each(function () {
        difficult = difficult + Number($.trim($(this).val())) * 10;
    });
    if (Number($.trim($("#QuestionCount").val())) * 10 * parseFloat($.trim($("#difficult").val())) < difficult) {
        alert("小题难度与试题难度设置不符，请重新设置小题或试题的难度");
        return false;
    }
    return istrue;
}
function keyPress() {
    var keyCode = event.keyCode;
    if ((keyCode >= 48 && keyCode <= 57)) {
        event.returnValue = true;
    }
    else {
        event.returnValue = false;
    }
}