$(document).ready(function(){
  reloadPrice();
  $("#checkout").on("click", function(){
    var price_ = document.getElementById('price').innerHTML;
    if(price_ == ""|| price_ == "0" || price_ == 0){
      alert("Không có sản phẩm nào để thanh toán cả!");
      return false;
    }
  });
});
function quantity(num_ , num, id){
  var quantity = document.getElementById('quantity-'+num_);
  var max = Number(quantity.getAttribute('max'));
  var min = Number(quantity.getAttribute('min'));
  var qa = quantity.value;
  if(num == -1){
    if(qa > min){
      if(id != "" || id != null, id !== undefined)
      {
        $.ajax({
          url : "/Cart/SubProduct",
          type : "post",
          data : {
            idProduct : id
          },
          success : function (result){
            console.log(result.message);
            if(result.position == "0"){
              return;
            }
          },
          error : function (){
            alert('Failed to receive the Data');
            return;
          }
        });
      }else{
        alert("Đã có lỗi xảy ra! Vui lòng thử lại!");
        return;
      }
      quantity.value--;
    }
  }else if(num == 1){
    if(qa < max){
      if(id != "" || id != null, id !== undefined)
      {
        $.ajax({
          url : "/Cart/AddProduct",
          type : "post",
          data : {
            idProduct : id
          },
          success : function (result){
            console.log(result.message);
            if(result.position == "0"){
              return;
            }
          },
          error : function (){
            alert('Failed to receive the Data');
            return;
          }
        });
      }else{
        alert("Đã có lỗi xảy ra! Vui lòng thử lại!");
        return;
      }
      quantity.value++;
    }
  }
  reloadPriceItem(num_);
  
  reloadPrice();
}
function reloadPriceItem(num){
  var price_item = document.getElementById('price-item-'+num).innerHTML;
  var sum_item = dePriceVnd(price_item) * document.getElementById('quantity-'+num).value;
  document.getElementById('sum-item-'+num).innerHTML = enPriceVnd(sum_item);
}
function removeCartItem(num, id){
  var my_obj = document.getElementById("item-"+num);
  var quantity = document.getElementById("quantity-"+num).value;
  var isRemove = confirm("Bạn muốn xoá sản phẩm này khỏi giỏ hàng!");
  if(!isRemove){
    return;
  }
  if(id != "" || id != null, id !== undefined || quantity != "" || quantity != null, quantity !== undefined)
  {
    $.ajax({
      url : "/Cart/RemoveProduct",
      type : "post",
      data : {
        idProduct : id,
        sl : quantity
      },
      success : function (result){
        alert(result.message);
        if(result.position == "0"){
          return;
        }
      },
      error : function (){
        alert('Failed to receive the Data');
        return;
      }
    });
  }else{
    alert("Đã có lỗi xảy ra! Vui lòng thử lại!");
    return;
  }
  my_obj.remove();
  reloadPrice();
}
function reloadPrice(){
  var num_arr = document.getElementById('numArr').innerHTML;
  var sum_price = 0; 
  for(var i = 1 ; i < num_arr; i++){
    if(document.getElementById('sum-item-'+i)){
      reloadPriceItem(i);
      sum_price += dePriceVnd(document.getElementById('sum-item-'+i).innerHTML);
    }
  }
  document.getElementById('sum-price').innerHTML = enPriceVnd(sum_price);
  sum_price = dePriceVnd(document.getElementById('sum-price').innerHTML);
  var shipping_price = dePriceVnd(document.getElementById('shipping-price').innerHTML);
  var tax = Number(document.getElementById('tax').innerHTML);
  document.getElementById('price').innerHTML = enPriceVnd(sum_price + shipping_price + (sum_price/100)*tax);
  if(sum_price == 0){
    document.getElementById('tax').innerHTML = 0;
    document.getElementById('price').innerHTML = 0;
    document.getElementById('shipping-price').innerHTML = 0;
  }
}