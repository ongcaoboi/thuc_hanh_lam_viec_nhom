$(document).ready(function(){
    $('#complate').on("click", function(){
        var price_ = document.getElementById('price').innerHTML;
        if(price_ == ""|| price_ == "0" || price_ == 0){
            alert("Không có sản phẩm nào để thanh toán cả!");
            return;
        }
        var fullName = $("#fullName").val();
        if(fullName == ""){
            alert("Vui lòng nhập họ tên!");
            return;
        }
        var address = $("#address").val();
        if(fullName == ""){
            alert("Vui lòng địa chỉ!");
            return;
        }
        var phone = $("#phone").val();
        if(fullName == ""){
            alert("Vui lòng nhập số điện thoại!");
            return;
        }
        var note = $("#note").val();
        if(fullName == ""){
            alert("Vui lòng nhập ghi chú!");
            return;
        }
        var isCheckout = confirm("Bạn xác nhận thanh toán!");
        if(!isCheckout){
            return;
        }
        $.ajax({
            url : '/Checkout/CreateOrder',
            type : 'post',
            data : {
                fullName : fullName,
                phone : phone,
                address : address,
                note : note
            },
            success : function(result){
                if(result.position == "1"){
                    window.location.href = '/Checkout/Complate';
                }else{
                    alert(result.message);
                }
            },
            error : function(){
                alert('Failed to receive the Data');
            }
        });
    });
});