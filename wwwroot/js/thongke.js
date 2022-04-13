$(document).ready(function () {
    let thang = $("#thang").val();
    let nam = $("#nam").val();

    getData(thang, nam);
});


function getData(thang, nam) {
    $.ajax({
        type: 'POST',
        url: '/Admin/MyAdmin/layThongKe',
        data: {
            thang: thang,
            nam: nam
        },
        success: function (result) {
            loadData(result);
        },
        error: function () {
            alert('Failed to receive the Data');
        }
    });
}
function loadData(data1) {

    const ngay = [];
    for(let i = 1; i <= data1.songay; i ++){
        ngay.push(i);
    }

    const data = {
        labels: ngay,
        datasets: [{
            label: 'Số đơn hàng',
            backgroundColor: '#f10021',
            borderColor: '#f10021',
            data: data1.donhang,
        },
        {
            label: 'Doanh thu',
            backgroundColor: '#ffe79a',
            borderColor: '#ffe79a',
            data: data1.doanhthu,
            yAxisID: 'match'
        }]
    };

    const config = {
        type: 'line',
        data: data,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        count: 10
                    }
                },
                match: {
                    beginAtZero: true,
                    type: 'linear',
                    position: 'right',
                    ticks: {
                    }
                },
            }
        }
    };

    const myChart = new Chart(
        document.getElementById('myChart'),
        config
    );
}
function changeData(){
    let thang = $("#thang").val();
    let nam = $("#nam").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/MyAdmin/layThongKe',
        data: {
            thang: thang,
            nam: nam
        },
        success: function (result) {
            document.getElementById("div_chart").innerHTML = "";
            document.getElementById("div_chart").innerHTML = '<canvas id="myChart"></canvas>';
            loadData(result);
        },
        error: function () {
            alert('Failed to receive the Data');
        }
    });
}