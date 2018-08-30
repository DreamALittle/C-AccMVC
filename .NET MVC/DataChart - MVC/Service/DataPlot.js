//申请监控
var ApplyChart = echarts.init(document.getElementById('ApplyChart'));
var PieOption = {
    title: {
        text: '今日码申请量监控',
        x: 'center'
    },
    legend: {
        orient: 'vertical',
        left: 'left',
        data: ['已审核', '已拒绝']
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    series: [
        {
            name: '申请记录',
            type: 'pie',
            radius: ['50%', '70%'],
            avoidLabelOverlap: false,
            label: {
                normal: {
                    show: false,
                    position: 'center'
                },
                emphasis: {
                    show: true,
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: [
                { value: 0, name: '已审核' },
                { value: 0, name: '已拒绝' }
            ],
        }
    ]
};
ApplyChart.setOption(PieOption);
UpdateApplyChartPlot();
function UpdateApplyChartPlot() {
    ApplyChart.showLoading();
    $.post('../webapi/DataPlot/GetApplyCount').done(function (data) {
        ApplyChart.hideLoading();
        ApplyChart.setOption({
            series: [
                {
                    name: '申请记录',
                    data: [
                        { value: data.data[0], name: '已审核'},
                        { value: data.data[1], name: '已拒绝' }
                    ]
                }
            ]
        });
    });
}

//上传监控
var UploadChart = echarts.init(document.getElementById('UploadChart'));
var RoseOption = {
    title: {
        text: '今日上传活动监控',
        x: 'center'
    },
    tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    legend: {
        orient: 'vertical',
        right: 'right',
        data: ['激活', '营销', '消费者', '资产', '资产领用']
    },
    series: [
        {
            name: '上传监控',
            type: 'pie',
            radius: [20, 110],
            center: ['50%', '60%'],
            roseType: 'radius',
            label: {
                normal: {
                    show: false
                },
                emphasis: {
                    show: true
                }
            },
            lableLine: {
                normal: {
                    show: false
                },
                emphasis: {
                    show: false
                }
            },
            data: [
                { value: 0, name: '激活' },
                { value: 0, name: '营销' },
                { value: 0, name: '消费者' },
                { value: 0, name: '资产' },
                { value: 0, name: '资产领用' }
            ]
        }
    ]
};
UploadChart.setOption(RoseOption);
UpdateUploadChartPlot();
function UpdateUploadChartPlot() {
    UploadChart.showLoading();
    $.post('../webapi/DataPlot/GetActiveCount').done(function (data) {
        UploadChart.hideLoading();
        UploadChart.setOption({
            series: [
                {
                    name: '上传监控',
                    data: [
                        { value: data.data[0], name: '激活' },
                        { value: data.data[1], name: '营销' },
                        { value: data.data[2], name: '消费者' },
                        { value: data.data[3], name: '资产' },
                        { value: data.data[4], name: '资产领用' }
                    ]
                }
            ]
        });
    });
}

//下载实时监控
var DownloadChart = echarts.init(document.getElementById('DownloadChart'));
var LineOption = {
    title: {
        text: '下载量实时监控',
    },
    tooltip: {
        trigger: 'axis'
    },
    xAxis: [
        {
            type: 'category',
            boundaryGap: true,
            data:[]
            //    (function () {
            //    var now = new Date();
            //    var res = [];
            //    var len = 10;
            //    while (len--) {         
            //        now.setMinutes(now.getMinutes() + 10);
            //        console.log(now);
            //        res.push(now.toLocaleTimeString().replace(/^\D*/, ''));                  
            //    }
            //    return res;
            //})()
        }
    ],
    yAxis: [
        {
            type: 'value',
            scale: true,
            min: 0,
        }
    ],
    series: [
        {
            name: '下载量',
            type: 'line',
            data: []
            //    (function () {
            //    var res = [];
            //    var len = 0;
            //    while (len < 10) {
            //        res.push((Math.random() * 10 + 5).toFixed(1) - 0);
            //        len++;
            //    }
            //    return res;
            //})()
        }
    ]
};
DownloadChart.setOption(LineOption);
UpdateDownloadChart(LineOption);
function UpdateDownloadChart(LineOption) {
    //获取新的统计数据
    UploadChart.showLoading();
    $.post('../webapi/DataPlot/GetDownloadTenMinCount').done(function (data) {
        console.log(data.data);
        UploadChart.hideLoading();
        //获取当前时间和图表数据
        axisData = (new Date()).toLocaleTimeString().replace(/^\D*/, '');
        var data1 = LineOption.series[0].data;
        if (LineOption.series[0].data.length > 10) {
            data1.shift();
        };
        data1.push(data.data);
        if (LineOption.xAxis[0].data.length > 10) {
            LineOption.xAxis[0].data.shift();
        };
        LineOption.xAxis[0].data.push(axisData);
        DownloadChart.setOption(LineOption);
    });
}

//下载渠道监控
var DownloadTotalChart = echarts.init(document.getElementById('DownloadTotalChart'));
var BarOption = {
    title: {
        text: '24小时内码下载分类统计',
        x: 'center'
    },
    xAxis: {
        type: 'category',
        splitLine: { show: false },
        data: ['总下载量', '接口下载', 'CMC内部下载']
    },
    yAxis: {
        type: 'value'
    },
    series: [
        {
            name: '总量',
            type: 'bar',
            stack: '总量',
            label: {
                normal: {
                    show: true,
                    position: 'inside'
                }
            },
            data: []
        }
    ]
};
DownloadTotalChart.setOption(BarOption);
UpdateDownloadTotalChart();
function UpdateDownloadTotalChart() {
    DownloadTotalChart.showLoading();
    $.post('../webapi/DataPlot/GetTotalDownload').done(function (data) {
        DownloadTotalChart.hideLoading();
        DownloadTotalChart.setOption({
            series: [
                {
                    name: '总量',
                    data: [
                        { value: data.data[0] },
                        { value: data.data[1] },
                        { value: data.data[2] }
                    ]
                }
            ]
        });
    });
};

//实时监控
setInterval(function () {
    UpdateApplyChartPlot();
    UpdateUploadChartPlot();
    UpdateDownloadChart(LineOption);
    UpdateDownloadTotalChart();
}, 60000*10);


//点击图标跳转页面
ApplyChart.on('click', function (params) {
    layui.use(['table', 'laypage', 'layer', 'form'], function () {
        var table = layui.table,
            form = layui.form,
            laypage = layui.laypage;
        var $ = layui.jquery, layer = layui.layer;
        content = layer.open({
            type: 2
            , title: '申请监控'
            , area: ['900px', '500px']
            , offset: [
                100,300,0,0
            ]
            , content: '../DataPlot/CodeApplyPlot'
        });     
    });
});
UploadChart.on('click', function (params) {
    layui.use(['table', 'laypage', 'layer', 'form'], function () {
        var table = layui.table,
            form = layui.form,
            laypage = layui.laypage;
        var $ = layui.jquery, layer = layui.layer;
        content = layer.open({
            type: 2
            , title: '上传监控'
            , area: ['900px', '500px']
            , offset: [
                100, 300, 0, 0
            ]
            , content: '../DataPlot/UploadPlot'
        });
    });
});
DownloadChart.on('click', function (params) {
    layui.use(['table', 'laypage', 'layer', 'form'], function () {
        var table = layui.table,
            form = layui.form,
            laypage = layui.laypage;
        var $ = layui.jquery, layer = layui.layer;
        content = layer.open({
            type: 2
            , title: '下载监控'
            , area: ['900px', '500px']
            , offset: [
                100, 300, 0, 0
            ]
            , content: '../DataPlot/DownloadRealTimePlot'
        });
    });
});
DownloadTotalChart.on('click', function (params) {
    layui.use(['table', 'laypage', 'layer', 'form'], function () {
        var table = layui.table,
            form = layui.form,
            laypage = layui.laypage;
        var $ = layui.jquery, layer = layui.layer;
        content = layer.open({
            type: 2
            , title: '下载监控'
            , area: ['900px', '500px']
            , offset: [
                100, 300, 0, 0
            ]
            , content: '../DataPlot/DownloadPlot'
        });
    });
});