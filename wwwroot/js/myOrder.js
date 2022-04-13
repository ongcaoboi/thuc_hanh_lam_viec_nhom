function hideOrderDetails(){
  var a = document.querySelectorAll('.order__details');
  a.forEach(function(Item) {
    Item.classList.remove('enable');
  });
}
function showOrderDetails(id){
  document.querySelector('#order-details-'+id).classList.add('enable');
}