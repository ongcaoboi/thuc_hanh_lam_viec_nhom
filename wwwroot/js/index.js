
$(document).ready(function(){
  $('#login_submit').on('click', function(){
    var login_name = $('#login_name').val();
    if(login_name == ""){
      alert('Vui lòng nhập tên đăng nhập!');
      return;
    }
    var login_pass = $('#login_pass').val();
    if(login_pass == ""){
      alert('Vui lòng nhập mật khẩu!');
      return;
    }
    var login_checkSave = $('#login_checkSave').prop('checked');
    $.ajax({
      type: 'POST',
      url: '/Home/Login',
      data: {
        login_name: login_name,
        login_pass: login_pass,
        login_checkSave: login_checkSave
      },
      success: function (result) {
        alert(result.message);
        if(result.position == "1"){
          location.reload();
        }
      },
      error: function () {
        alert('Failed to receive the Data');
      }
    });
  });
  $('#register_submit').on('click', function(){
    var register_name = $('#register_name').val();
    if(register_name == ""){
      alert('Vui lòng nhập tên đăng nhập!');
      return;
    }
    var register_pass = $('#register_pass').val();
    if(register_pass == ""){
      alert('Vui lòng nhập mật khẩu!');
      return;
    }
    var register_repass = $('#register_repass').val();
    if(register_repass != register_pass){
      alert('Nhập lại mật khẩu không chính xác!');
      return;
    }
    var register_fullname = $('#register_fullname').val();
    if(register_fullname == ""){
      alert('Vui lòng nhập họ tên!');
      return;
    }
    var register_sdt = $('#register_sdt').val();
    if(register_sdt == ""){
      alert('Vui lòng nhập số điện thoại!');
      return;
    }
    var register_email = $('#register_email').val();
    if(register_email == ""){
      alert('Vui lòng nhập email!');
      return;
    }
    $.ajax({
      type: 'POST',
      url: '/Home/Register',
      data: {
        name: register_name,
        pass: register_pass,
        fullname: register_fullname,
        sdt: register_sdt,
        email: register_email,
      },
      success: function (result) {
        alert(result.message);
        if(result.position == "1"){
          $('#register_name').val("");
          $('#register_pass').val("");
          $('#register_repass').val("");
          $('#register_fullname').val("");
          $('#register_sdt').val("");
          $('#register_email').val("");
          showLogin();
        }
      },
      error: function () {
        alert('Failed to receive the Data');
      }
    });
  });
  $('#repass_submit').on('click', function(){
    var old_pass = $('#old_pass').val();
    if(old_pass == ""){
      alert('Vui lòng nhập mật khẩu!');
      return;
    }
    var new_pass = $('#new_pass').val();
    if(new_pass == ""){
      alert('Vui lòng nhập mật khẩu mới');
      return;
    }
    if(new_pass == old_pass){
      alert('Mật khẩu mới không được trùng mật khẩu cũ');
      return;
    }
    var renew_pass = $('#renew_pass').val();
    if(renew_pass != renew_pass){
      alert('Nhập lại mật khẩu mới không chính xác!');
      return;
    }
    $.ajax({
      type: 'POST',
      url: '/Home/Repass',
      data: {
        old_pass: old_pass,
        new_pass: new_pass
      },
      success: function (result) {
        alert(result.message);
        if(result.position == "1"){
          $('#old_pass').val("");
          $('#new_pass').val("");
          $('#renew_pass').val("");
          hideRepass();
        }
      },
      error: function () {
        alert('Failed to receive the Data');
      }
    });
  });
});

window.onscroll = function (){scrollfunction()};
function scrollfunction (){
  if(document.body.scrollTop >20 || document.documentElement.scrollTop >20){
    document.getElementById('btn_go_to_top').style.display = 'block';
  }else{
    document.getElementById('btn_go_to_top').style.display = 'none';
  }
}
document.getElementById('btn_go_to_top').addEventListener('click', function (){
  document.body.scrollTop = 0;
  document.documentElement.scrollTop = 0;
});
function hideMenu(){
  var a = document.querySelectorAll('.account');
  a.forEach(function(Item) {
    Item.classList.remove('enable');
  });
}
function showLogin(){
  hideMenu();
  document.querySelector('#login').classList.add('enable');
}
function showRegister(){
  hideMenu();
  document.querySelector('#register').classList.add('enable');
}
function showRepass(){
  hideMenu();
  document.querySelector('#repass').classList.add('enable');
}
function hideLogin(){
  document.querySelector('#login').classList.remove('enable');
}
function hideRegister(){
  document.querySelector('#register').classList.remove('enable');
}
function hideRepass(){
  document.querySelector('#repass').classList.remove('enable');
}