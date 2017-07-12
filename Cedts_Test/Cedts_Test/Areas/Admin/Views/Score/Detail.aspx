<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	查看详情
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" value='<%:ViewData["UserID"] %>' class="StudID"/>
    <script type="text/javascript">
        $(function () {
            var UserID = $(".StudID").val();
            $.post("/Score/UserScore", {UserID: UserID}, function (data) {
                var Score = [];
                var Name = [];
                for (var i = 0; i < data.Score.length; i++) {
                    Score.push(data.Score[i]);
                    Name.push("'" + data.TypeName[i] + "'");

                }
                var chart;

                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'tt1',
                        defaultSeriesType: 'spline',
                        marginTop: 20,
                        marginRight: 130,
                        marginBottom: 100
                    },
                    title: {
                        text: null
                    },
                    xAxis: {
                        categories: Name,
                        labels: {
                            rotation: -18,
                            align: 'right',
                            style: { font: 'normal 12px Cambria, Cambria' }
                        }
                    },
                    yAxis: {
                        title: {
                            text: '%'
                        },
                        min: 0,
                        plotLines: [{
                            value: 0,
                            width: 1,
                            color: '#808080'
                        }]
                    },
                    legend: {
                        enabled: false
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' +
               this.x + ': ' + this.y + '&';
                        }
                    },
                    series: [{
                        name: '得分率',
                        data: data.Score
                    }]
                });

            })

            $.post("/Score/UserKPInfo", { UserID: UserID } , function (data) {

                var Name = [];
                var SMi = [];
                var UMi = [];
                for (var i = 0; i < data.length; i++) {
                    Name.push("'" + data[i].KPName + "'");
                    SMi.push(data[i].SMi);
                    UMi.push(data[i].UMi);
                }
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'tt2',
                        defaultSeriesType: 'spline',
                        marginTop: 20,
                        marginRight: 130,
                        marginBottom: 100
                    },
                    title: {
                        text: null
                    },
                    xAxis: {
                        categories: Name,
                        labels: {
                            rotation: -35,
                            align: 'right',
                            style: { font: 'normal 12px Cambria, sans-serif' }
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: '%'
                        },
                        plotLines: [{
                            value: 0,
                            width: 1,
                            color: '#808080'
                        }]
                    },
                    legend: {
                        layout: 'vertical',
                        align: 'right',
                        verticalAlign: 'top',
                        x: -10,
                        y: 20,
                        borderWidth: 0

                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' +
               this.x + ': ' + this.y + '%';
                        }
                    },
                    series: [
                    { name: "该生累计样本知识点掌握率",
                        data: UMi
                    },
                    { name: "全体用户测试样本知识点掌握率",
                        data: SMi
                    }
                ]
                });
            })
            $.post("/Score/UserItemInfo", { UserID: UserID }, function (data) {
                var chart;
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'tt3',
                        defaultSeriesType: 'column',
                        marginTop: 20,
                        marginRight: 130
                    },
                    title: {
                        text: null
                    },
                    xAxis: {
                        labels: {
                            rotation: -18,
                            align: 'right',
                            style: { font: 'normal 12px Cambria, sans-serif' }
                        },
                        categories: [
                        '快速阅读',
                        '短对话',
                        '长对话',
                        '听力短文理解',
                        '复合型听力',
                        '阅读理解-选词填空',
                        '阅读理解-选择题',
                        '阅读理解-信息匹配',
                        '完型填空'
                     ]
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: '正确率 (%)'
                        }
                    },
                    tooltip: {
                        formatter: function () {
                            return '' +
               this.x + ': ' + this.y + ' %';
                        }
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0
                        }
                    },
                    series: [{
                        name: "四级考试各题型分数占比",
                        data: data.Fortysix
                    }, { name: "全体用户平均得分率",
                        data: data.SScale
                    }, { name: "该生各题型得分率",
                        data: data.UScale
                    }]
                });
            })
        });
    </script>
<div class="studscore">
    <table border="0" cellpadding="4" cellspacing="1">
        <tr>
            <td class="sec" id="t1">
                近期试卷得分率走势图
            </td>
            <td id="t2" class="sec1">
                知识点掌握情况分布图
            </td>
            <td id="t3" class="sec1">
                题型得分率分布图
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <div id="tt1" style="width: 600px; height: 350px">
                </div>
                <div id="tt2" style="display: none; width: 600px; height: 350px;">
                </div>
                <div id="tt3" style="display: none; width: 600px; height: 350px">
                </div>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">
    $("#t1").click(function () {
        $("#tt2").hide();
        $("#tt3").hide();
        $("#tt1").show();
        $("#t1").css("color", "#FFFFFF");
        $("#t2").css("color", "#990000");
        $("#t3").css("color", "#990000");
        $("#t1").css("background-color", "#990000");
        $("#t2").css("background-color", "#FFFFFF");
        $("#t3").css("background-color", "#FFFFFF");
        $("#tt").css("background-color", "#FFFFFF");
        $("#KpAnalysis").hide();
        $("#QuestionAnalysis").hide();
        $("#rateAnalysis").show();
    })
    $("#t2").click(function () {
        $("#tt1").hide();
        $("#tt3").hide();
        $("#tt2").show();
        $("#t2").css("color", "#FFFFFF");
        $("#t1").css("color", "#990000");
        $("#t3").css("color", "#990000");
        $("#t2").css("background-color", "#990000");
        $("#t1").css("background-color", "#FFFFFF");
        $("#t3").css("background-color", "#FFFFFF");
        $("#rateAnalysis").hide();
        $("#QuestionAnalysis").hide();
        $("#KpAnalysis").show();
    })
    $("#t3").click(function () {
        $("#tt1").hide();
        $("#tt2").hide();
        $("#tt3").show();
        $("#t3").css("color", "#FFFFFF");
        $("#t2").css("color", "#990000");
        $("#t1").css("color", "#990000");
        $("#t3").css("background-color", "#990000");
        $("#t2").css("background-color", "#FFFFFF");
        $("#t1").css("background-color", "#FFFFFF");
        $("#rateAnalysis").hide();
        $("#QuestionAnalysis").show();
        $("#KpAnalysis").hide();
    })
</script>
</asp:Content>
