<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="../../Scripts/highcharts.js" type="text/javascript"></script>
<script src="../../Scripts/exporting.js" type="text/javascript"></script>
<style type="text/css">
    body
    {
        font-size: 12px;
    }
    .sec, .sec1
    {
        cursor: pointer;
        font-weight: bold;
        font-size: 14px;
        padding: 2px 0;
        border-bottom: 1px solid #CCC;
    }
    .sec
    {
        color: #FFFFFF;
        text-align: center;
        background-color: #990000;
    }
    .sec1
    {
        color: #990000;
        text-align: center;
        background-color: #FFFFFF;
    }
    div
    {
        line-height: 22px;
    }
</style>
<script type="text/javascript">
    $(function () {
        $.post("/Home/TotalSocreView", function (data) {
            Score = [];
            Count = [];
            for (var i = 0; i < data.Score.length; i++) {
                Score.push("'" + data.Score[i] + "'");
                Count.push(data.Count[i]);
            }
            var chart;

            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'tt1',
                    defaultSeriesType: 'spline',
                    marginTop: 20,
                    marginRight: 130,
                    marginBottom: 25
                },
                title: {
                    text: null
                },
                xAxis: {
                    categories: ['0-10分', '10-20分', '20-30分', '30-40分', '40-50分', '50-60分', '60-70分', '70-80分', '80-90分']
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: '人数'
                    },
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
               this.x + ': ' + this.y + '人';
                    }
                },
                series: [{
                    name: '人数',
                    data: data.Count
                }]
            });

        })

        $.post("/Home/ItemInfo", function (data) {
            var chart;
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'tt3',
                    defaultSeriesType: 'column'
                },
                title: {
                    text: null
                },
                xAxis: {
                    categories: [
            '快速阅读',
            '短对话',
            '长对话',
            '听力短文理解',
            '复合型听力',
            '阅读理解-选词填空',
            '阅读理解-选择题型',
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
                    name: "题型",
                    data: data
                }]
            });
        })

        $.post("/Home/KonwledgeView", function (data) {
            kplist = [];

            for (var i = 0; i < data.length; i++) {
                kp = [];
                kp.push("'" + data[i].KPName + "'");
                kp.push(data[i].UMi);
                kplist.push(kp);
            }
            var chart;
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'tt2',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: null
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.point.name + '</b>: ' + this.y + ' %';
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    type: 'pie',
                    name: 'Browser share',
                    data: kplist
                }]
            });
        })

    });
</script>
<center><h2 style="width: 700px">系统用户信息统计</h2></center>
<table border="0" cellpadding="4" cellspacing="1">
    <tr>
        <td class="sec" id="t1">
            成绩统计分布图
        </td>
        <td id="t2" class="sec1">
            知识点统计分布图
        </td>
        <td id="t3" class="sec1">
            题型统计分布图
        </td>
    </tr>
    <tr>
        <td colspan="5">
            <div id="tt1" style="width: 600px; height: 400px">
            </div>
            <div id="tt2" style="display: none; width: 600px; height: 400px;">
            </div>
            <div id="tt3" style="display: none; width: 600px; height: 400px">
            </div>
        </td>
    </tr>
</table>
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
    })
</script>
