function dePriceVnd(str){
  return Number(str.replaceAll(",", ""));
}
function enPriceVnd(num){
  return num.toLocaleString();
}