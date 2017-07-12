(function ($) {
    function Check() {
        var num = Number($("#txt").val());
        if (num > 30 || num < 1) {
            alert("请输入1-30的数字！");
            return false;
        }
        return true;
    }
    $(document).ready(function () {
        $("#BadKnowledge").click(function () {
            var temp = Check();
            if (temp) {
                opentt();
                var Num = $("#txt").val();
                var Title4 = $("#CeType4Title").val();
                $.post("/papershow/ShowPaperByBadKnowledge", { Num: Num, Title: Title4 },
                function (data) {
                    window.location.href = "/papershow/PaperShow";
                }, "text");
            }
        })

        $("#SelectByKnowledge").click(function () {

            var select2 = "";
            var select3 = "";
            $(".select2").each(function () {
                if ($(this).val() == "" || $(this).val() == "--请选择--") {
                    return false;
                }
                select2 += $(this).val() + ",";
                select3 += $(this).next("select").val() + ",";
            })
            if (select2 == "" || select3 == "") {
                closett();
                alert("请选择知识点！");
                return false;
            }

            var Title3 = $("#CeType3Title").val();
            var totle = select3.split(',');
            var s = 0;
            for (var i = 0; i < totle.length - 1; i++) {
                s += Number(totle[i]);
            }
            if (s >= 150) {
                alert("题目数量不能超过150！");
            }
            else {
                opentt();
                $.post("/papershow/showpaperbyknowledge", { select2: select2, select3: select3, Title3: Title3 },
                function (data) {
                    window.location.href = "/papershow/papershow";
                }, "text");
            }
        })

        $('#ll').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true
        });
        function openll() {
            $('#ll').dialog('open');
        }
        var UserName = $("#UserName").val();
        $.post("/PaperShow/SelectPaper", { UserName: UserName }, function (data) {
            if (data == "1") {
                openll();
            }
        })

        $("#Test").click(function () {
            window.location.href = '../../PaperShow/Exerciser/' + $("#PaperID").val();
        })
        $('#pp').dialog({
            title: '',
            width: 400,
            height: 100,
            modal: true,
            closed: true,
            onBeforeOpen: function () {
                $("#pp").empty().append("<h1><center>正在组卷中，请稍后~~~！<br /><img src='../../../../Images/06.gif' /></center></h1>");
            },
            onBeforeClose: function () { }
        });
        function opentt() {
            $('#pp').dialog('open');
        }
        function closett() {
            $('#pp').dialog('close');
        }


        var Cooike = document.cookie.split('; ');
        for (var ii = 0; ii < Cooike.length; ii++) {
            var atrr = Cooike[ii].split("=");
            if (atrr[0] == ".ASPXAUTH" && atrr[1] != "") {
                var Tourist = $("#ViewName").html();
                if (Tourist == "游客" || Tourist == "") {
                }
                else {
                    $("#loginForm").hide();
                }
                $("#before").hide();
                $("#after").show();
            }
        }
        if ($('.slides').length > 0) {
            $('.slides').slides({
                preload: true,
                play: 5000,
                pause: 2500,
                generatePagination: false,
                effect: 'fade',
                hoverPause: true
            });
        }
        if ($('.loginForm').length > 0) {
            //var $h4 = $('.loginForm h4').eq(0);
            var $quickLogin = $('.btnQuickLogin');
            var $form = $('.loginForm form').eq(0);
            var $btnLogon = $('.loginForm .btnLogon').eq(0);
            $quickLogin.click(function (e) {
                e.stopPropagation();
                if ($form.is(':hidden')) {
                    $form.fadeIn(200, function () {
                        $form.find('input:first').select().focus();
                    });
                }
            });
            $form.click(function (e) { e.stopPropagation(); });
            $form.bind('keypress', function(e) {
                if(e.keyCode==13){
                    $('#btnLogon').click();
                }
            });
            $('body').click(function () {
                if ($form.is(':visible')) {
                    $form.fadeOut(200);
                }
            });

            $('#btnLogon').click(function (e) {
                e.preventDefault();
                //TODO 登录校验提交等
                //页面中的表单校验错误信息暂未处理，待整合后再调整CSS等
                var Account = $("#UserAccount").val();
                var Password = $("#UserPassword").val();
                $.post("../Account/CheckAccount", { Account: Account }, function (data) {
                    if (data == "False") {
                        alert("帐号不存在！");
                    }
                    else {
                        $.post("../Account/AjaxCheck", { Account: Account, PassWord: Password }, function (data) {
                            if (data == "True") {
                                $("#submit").click();
                            }
                            else {
                                alert("密码错误！");
                            }
                        })
                    }
                })

            })

            $(".btnRegNow").click(function () {//注册
                window.location.href = '/Account/Register';
            })

            $(".Forget").click(function () {//忘记密码
                window.location.href = "/Account/NewRetrievePassword";
            })
        }
        if ($('.tbRecords tbody tr').length > 0) {
            $('.tbRecords tbody tr').hover(function () { //暂停的练习，鼠标经过时显示继续做题
                if ($(this).find('.pstat.paused').length) {
                    $(this).find('.pstat.paused *').toggle();
                }
            });

            $('.tbRecords').tablesorter({ //练习记录的表排序功能
                'sortList': [[1, 1]],
                'headers': { 4: { sorter: false} },
                'cssAsc': 'sortbyasc',
                'cssDesc': 'sortbydes',
                'cssHeader': 'sortable'
            });
        }
        if ($('.chooseExercise').length > 0) {
            $('.chooseExercise').appendTo($('body'));
            $('.chooseExercise').jqm({ // 初始化弹出框参数
                onShow: function (hash) { hash.w.fadeIn(300); },
                onHide: function (hash) { hash.w.fadeOut(300); hash.o.fadeOut(600); }
            });
            $('.btnCustomizeExercise').click(function (e) { //自定练习 按钮
                e.preventDefault();
                if ($("#UserRole").val() == "体验用户") {
                    alert("请先登陆！");
                }
                else {
                    $('.chooseExercise').jqmShow();
                }
            });
            $('.btnQuickExercise').click(function (e) { //快速练习 按钮，随机生成试卷，并自动命名（无需用户操作即开始练习）
                e.preventDefault();
                //TODO
                if ($("#UserRole").val() == "体验用户") {
                    window.location.href = '/PaperShow/QuickExperience';
                }
                else {
                    window.location = '/PaperShow/ShowPaperByRand';
                }
            });

            var chooseExcercise = function (id) { //选择练习类型函数
                $('.ceTab').hide();
                $('#CeType' + id).show();
            }

            $('.ceMenu li').each(function (ind) { // 选择练习类型菜单
                $(this).click(function () {
                    chooseExcercise(ind + 1);
                    $(this).siblings('.current').removeClass('current');
                    $(this).addClass('current');
                });
            });
            $('.ceMenu li').first().click(); //初始状态选中第一种类型


            $('.cetList li').click(function () { // 四六级列表
                $(this).siblings().removeClass('chosen');
                $(this).addClass('chosen');
                //TODO 记录当前选中的试卷，以便点击开始时提交
            }).hover(function () { $(this).toggleClass('hover'); });

            $('.itemTypeCustomize .btnStandard').click(function (e) { //载入默认试题设置
                e.preventDefault();
                var standard = [1, 8, 2, 3, 1, 1, 2, 1, 0]; //注意：这是value数组，不是index数组，虽然此处它们相同
                $.each($('.itemTypeCustomize select'), function (i) {
                    $(this).val(standard[i]);
                });
            });

            $('.itemTypeCustomize .btnClear').click(function (e) { //清空设置按钮
                e.preventDefault();
                $.each($('.itemTypeCustomize select'), function (i) {
                    $(this).val(0);
                });
            });
            $('#testinfo').click(function (e) {
                e.preventDefault();
                window.location.href = '../../PaperShow/UserTestInfo';
            })
            $('.btnStart').click(function (e) { //开始按钮
                e.preventDefault();
                //TODO 需要判断当前是那种类型做不同的数据提交
                var $btnStart = $(this);

                $('.ceTab').each(function () {
                    if ($(this).css("display") != "none") {
                        var type = $(this).attr("id");
                        switch (type) {
                            case "CeType1":
                                var PaperID;
                                $('.cetList li').each(function () {
                                    var s = $(this).attr("class");
                                    if (s == "chosen") {
                                        PaperID = $(this).attr("id");
                                    }
                                })
                                if (typeof PaperID == "undefined") {
                                    alert("请选择试卷！");
                                }
                                else {
                                    opentt();
                                    var href = "/PaperShow/Exerciser/" + PaperID;
                                    window.location.href = href;
                                }
                                break;
                            case "CeType2":
                                var sspc = Number($("#SspcChoice").val());
                                var slpo = Number($("#SlpoChoice").val());
                                var llpo = Number($("#LlpoChoice").val());
                                var rlpo = Number($("#RlpoChoice").val());
                                var lpc = Number($("#LpcChoice").val());
                                var rpo = Number($("#RpoChoice").val());
                                var rpc = Number($("#RpcChoice").val());
                                var infomat = Number($("#InfoMat").val());
                                var cp = Number($("#CpChoice").val());
                                var title = $("#CeType2Title").val();
                                $("#CeType2Propmt").html("");
                                if (title == "") {
                                    $("#CeType2Propmt").html("题目不能为空！");
                                }
                                else if (slpo == "0" && sspc == "0" && llpo == "0" && rlpo == "0" && lpc == "0" && rpo == "0" && rpc == "0" && cp == "0" && infomat == "0") {
                                    alert("请选择题数！");
                                }
                                else {
                                    opentt();
                                    $.post("/PaperShow/ShowPaperByItemType", { Title: title, Sspc: sspc, Slpo: slpo, Llpo: llpo, Rlpo: rlpo, Lpc: lpc, Rpo: rpo, Rpc: rpc, InfoMat: infomat, Cp: cp }, function () {
                                        window.location.href = "/PaperShow/PaperShow";
                                    })
                                }
                                break;
                            case "CeType3":
                                var title = $("#CeType3Title").val();
                                $("#CeType3Propmt").html("");
                                if (title == "") {
                                    $("#CeType3Propmt").html("题目不能为空！");
                                }
                                else {
                                    $("#SelectByKnowledge").click();
                                }
                                break;
                            case "CeType4":
                                var title4 = $("#CeType4Title").val();
                                $("#CeType4Propmt").html("");
                                if (title4 == "") {
                                    $("#CeType4Propmt").html("题目不能为空！");
                                }
                                else {
                                    $("#BadKnowledge").click();
                                }
                                break;
                            case "CeType5":
                                var title5 = $("#CeType5Title").val();
                                $("#CeType5Propmt").html("");
                                if (title5 == "") {
                                    $("#CeType5Propmt").html("题目不能为空！");
                                }
                                else {
                                    opentt();
                                    $.post("/PaperShow/ShowPaperByKnowledgeCover", { title: title5 }, function (data) {
                                        window.location.href = "/PaperShow/PaperShow";
                                    })
                                }
                                break;
                            default:
                                break;
                        }
                    }
                });
                //                $btnStart.text('请稍候...');
                //                $('<div class="globalMask"/>').appendTo($('body')).click(function (e) {
                //                    console.log('x');
                //                    e.stopPropagation();
                //                    e.preventDefault();
                //                }).width($('body').outerWidth()).height($('body').outerHeight());
            });
        }
    });
})(jQuery);