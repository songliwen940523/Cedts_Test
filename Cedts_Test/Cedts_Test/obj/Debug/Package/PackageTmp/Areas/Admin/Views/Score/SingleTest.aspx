<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	详细报告
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function () {
            var TestID = $("#TestID").val();
            var PaperID = $("#PaperID").val();
            var UserID = $("#UserID").val();
            var PaperState = $("#PaperState").val();
            $.post("/Score/Suggestions", { UserID: UserID, PaperID: PaperID }, function (data) {
                $("#Suggestions").append(data);
            })
            if (TestID != "") {
                if (PaperState == 8) {
                    $.post("/Score/Knowledge", { UserID: UserID, TestID: TestID }, function (data) {
                        Name = [];
                        SMi = [];
                        UMi = [];
                        for (var i = 0; i < data.length; i++) {
                            Name.push("'" + data[i].KPName + "'");
                            SMi.push(data[i].SMi);
                            UMi.push(data[i].UMi);
                        }
                        $("#tt1").addClass("graph");
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
                                    rotation: -35,
                                    align: 'right',
                                    style: { font: 'normal 12px Cambria, Cambria' }
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
                            { name: "该生本次测试样本知识点掌握率",
                                data: SMi
                            },
                            { name: "该生累计测试样本知识点掌握率",
                                data: UMi
                            }
                        ]
                        });
                    })
                }
                $.post("/Score/Item", { UserID: UserID, TestID: TestID }, function (data) {
                    $("#tt2").addClass("graph");
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
                            categories: data.ItemName
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
                            name: "该试卷中各题型所占百分比",
                            data: data.Fortysix
                        }, { name: "该生得分情况",
                            data: data.UScale
                        }]
                    });
                })
                if (PaperState == 8) {
                    $.post("/Score/KpList", { UserID: UserID, TestID: TestID }, function (data) {
                        var info = "<table class=\"statisticTable\"><thead><tr><th>知识点名称</th><th>题数</th><th>正确率</th><th>掌握率</th></tr></thead>\n<tbody>";
                        for (var i = 0; i < data.Num.length; i++) {
                            info += "<tr><td>" + data.KpName[i] + "</td><td>" + data.Num[i] + "题</td><td>" + data.CorrectRate[i] + "%</td><td>" + data.KPMaster[i] + "%</td></tr>";
                        }
                        info += "</tbody></table>";
                        $("#Kp").append(info);
                    })
                }
                $.post("/Score/ItemList", { UserID: UserID, TestID: TestID }, function (data) {
                    var info = "<table class=\"statisticTable\"><thead><tr><th>题型</th><th>总题数</th><th>答对题数</th></tr></thead>\n<tbody>";
                    for (var i = 0; i < data.Num.length; i++) {
                        info += "<tr><td>" + data.ItemName[i] + "</td><td>" + data.Num[i] + "题</td><td>" + data.CorrectRate[i] + "</td></tr>";
                    }
                    info += "</tbody></table>";
                    $("#Item").append(info);
                })
            }
        })
    </script>
    <input type="hidden" id="TestID" value='<%:ViewData["TestID"] %>' />
    <input type="hidden" id="PaperID" value='<%:ViewData["PaperID"] %>' />
    <input type="hidden" id="UserID" value='<%:ViewData["UserID"] %>' />
    <input type="hidden" id="PaperState" value='<%:ViewData["PaperState"] %>' />
    <div class="generalRemark infoItem">
        <h2>
            整体评价：</h2>
        <div id="Suggestions">
        </div>
    </div>
    <%if (ViewData["TestID"] != "")
      {%>
        <div class="userStatistic infoItem">
            <h2>
                <%:ViewData["PaperName"] %>详细分析</h2>
            
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
        </div>
    <%} %>
</asp:Content>
