<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="../../Scripts/highcharts.js" type="text/javascript"></script>
<script src="../../Scripts/exporting.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        var TestID = $("#TestID").val();
        $.post("/PaperShow/Knowledge", { id: TestID }, function (data) {
            Name = [];
            SMi = [];
            UMi = [];
            for (var i = 0; i < data.length; i++) {
                Name.push("'" + data[i].KPName + "'");
                SMi.push(data[i].SMi);
                UMi.push(data[i].UMi);
            }
            $("#tt1").addClass("graph");
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
                        rotation: -35,
                        align: 'right',
                        style: { font: 'normal 12px Cambria, Cambria' }
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Mi'
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
               this.x + ': ' + this.y + '%';
                    }
                },
                series: [
                    { name: "该生本次测试样本知识点掌握率",
                        data: SMi
                    },
                    { name: "该生累计测试样本知识点掌握率",
                        data: UMi
                    }
                ]
            });
        })
        $.post("/PaperShow/CheckUser", function (date) {
            if (date != "1") {
                $("#tt2").addClass("graph");
                $.post("/PaperShow/Item", { id: TestID }, function (data) {
                    var chart;
                    chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'tt2',
                            defaultSeriesType: 'column'
                        },
                        title: {
                            text: null
                        },
                        xAxis: {
                            labels: {
                                rotation: -18,
                                align: 'right',
                                style: { font: 'normal 12px Cambria, Cambria' }
                            },
                            categories: [
            '快速阅读',
            '短对话',
            '长对话',
            '听力短文理解',
            '复合型听力',
            '阅读理解-选词填空',
            '阅读理解-选择题型',
            '完型填空',
            '信息匹配'
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
                            name: "四级考试中各题型所占百分比",
                            data: data.Fortysix
                        }, { name: "该生得分情况",
                            data: data.UScale
                        }]
                    });
                })
            }
            else {
                $("#Item").hide();
            }
        })
        $.post("/PaperShow/SingleKpAnalysis", { id: TestID }, function (date) {
            $("#Kp").append(date);
        })
        $.post("/PaperShow/KpList", { id: TestID }, function (data) {
            var info = "<table class=\"statisticTable\"><thead><tr><th>知识点名称</th><th>题数</th><th>正确率</th><th>掌握率</th></tr></thead>\n<tbody>";
            for (var i = 0; i < data.Num.length; i++) {
                info += "<tr><td>" + data.KpName[i] + "</td><td>" + data.Num[i] + "题</td><td>" + data.CorrectRate[i] + "%</td><td>" + data.KPMaster[i] + "%</td></tr>";
            }
            info += "</tbody></table>";
            $("#Kp").append(info);
        })

        $.post("/PaperShow/ItemList", { id: TestID }, function (data) {
            var info = "<table class=\"statisticTable\"><thead><tr><th>题型</th><th>总题数</th><th>答对题数</th></tr></thead>\n<tbody>";
            for (var i = 0; i < data.Num.length; i++) {
                info += "<tr><td>" + data.ItemName[i] + "</td><td>" + data.Num[i] + "题</td><td>" + data.CorrectRate[i] + "</td></tr>";
            }
            info += "</tbody></table>";
            $("#Item").append(info);
        })
        $.post("/PaperShow/Validation", { id: TestID }, function (date) {
            if (date == "1") {
                $("#tt3").addClass("graph");
                $.post("/PaperShow/SingleRateAnalysis", { id: TestID }, function (data) {
                    $("#SingleRateAnalysis").append(data);
                })
                $.post("/PaperShow/SamePaper", { id: TestID }, function (data) {
                    chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'tt3',
                            defaultSeriesType: 'spline',
                            marginTop: 20,
                            marginRight: 130,
                            marginBottom: 100
                        },
                        title: {
                            text: null
                        },
                        xAxis: {
                            categories: data.PaperName,
                            labels: {
                                rotation: -18,
                                align: 'right',
                                style: { font: 'normal 12px Cambria, Cambria' }
                            }
                        },
                        yAxis: {
                            min: 0,
                            title: {
                                text: '分'
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
               this.x + ': ' + this.y + '分';
                            }
                        },
                        series: [{ name: "同一试卷成绩分布图",
                            data: data.Score
                        }]
                    });
                })
            }
        });
    })
</script>
<h2>
    <%:ViewData["PaperName"] %>详细分析</h2>
<input type="hidden" id="TestID" value='<%:ViewData["TestID"] %>' />
<div class="statisticItem">
    <div id="tt3">
    </div>
    <div id="SingleRateAnalysis" class="info">
    </div>
</div>
<div class="statisticItem">
    <div id="tt2">
    </div>
    <div id="Item" class="info">
    </div>
</div>
<div class="statisticItem">
    <div id="tt1">
    </div>
    <div id="Kp" class="info">
    </div>
</div>
