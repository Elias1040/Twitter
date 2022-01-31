// $(document).ready(function () {
//     if (localStorage.getItem("my_app_name_here-quote-scroll") != null) {
//         $(window).scrollTop(localStorage.getItem("my_app_name_here-quote-scroll"));
//     }

//     $(window).on("scroll", function() {
//         localStorage.setItem("my_app_name_here-quote-scroll", $(window).scrollTop());
//     });

//   });

  
//#region validation
var lower = /^(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])[a-zA-Z0-9]{8,100}$/;
var emailVal = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
var valid1 = false;
var valid2 = false;
var valid3 = false;
var valid4 = false;

function fNameVal(input) {
    if (input.length >= 100) {
        document.getElementById("psw").value = input.slice(0, -1);
    }
    if (input.length > 0) {
        document.getElementById("name").src = "check.png"
        document.getElementById("name").setAttribute("width", "20px");
        document.getElementById("name").setAttribute("height", "20px");
        document.getElementById("name").setAttribute("style", "display: inline-block!important");
        valid4 = true;
    }
    else if (input == 0) {
        document.getElementById("email").setAttribute("style", "display: none!important");
        valid4 = false
    }
    if (valid1 && valid2 && valid3) {
        document.getElementById("SignupSubmit").removeAttribute("disabled");
    }
    else {
        document.getElementById("SignupSubmit").setAttribute("disabled", true);
    }
}

function validationE(input) {
    if (input.match(emailVal)) {
        document.getElementById("email").src = "check.png"
        document.getElementById("email").setAttribute("width", "20px");
        document.getElementById("email").setAttribute("height", "20px");
        document.getElementById("email").setAttribute("style", "display: inline-block!important");
        valid1 = true;
    }
    else if (input == 0) {
        //document.getElementById("SignupSubmit").setAttribute("disabled", true);
        document.getElementById("email").setAttribute("style", "display: none!important");
        valid1 = false;
    }
    else {
        //document.getElementById("SignupSubmit").setAttribute("disabled", true);
        document.getElementById("email").src = "close.png"
        document.getElementById("email").setAttribute("width", "14px");
        document.getElementById("email").setAttribute("height", "14px");
        document.getElementById("email").setAttribute("style", "display: inline-block!important");
        valid1 = false;
    }
    if (valid1 && valid2 && valid3) {
        document.getElementById("SignupSubmit").removeAttribute("disabled");
    }
    else {
        document.getElementById("SignupSubmit").setAttribute("disabled", true);
    }
}

function validationP(input) {
    if (input.length >= 100) {
        document.getElementById("psw").value = input.slice(0, -1);
    }

    if (input.match(lower) && input.length >= 8) {
        document.getElementById("password").src = "check.png"
        document.getElementById("password").setAttribute("width", "20px");
        document.getElementById("password").setAttribute("height", "20px");
        document.getElementById("password").setAttribute("style", "display: inline-block!important");
        valid2 = true;
    }
    else if (input == 0) {
        //document.getElementById("SignupSubmit").setAttribute("disabled", true);
        document.getElementById("password").setAttribute("style", "display: none!important");
        valid2 = false;
    }
    else {
        document.getElementById("password").src = "close.png"
        document.getElementById("password").setAttribute("width", "14px");
        document.getElementById("password").setAttribute("height", "14px");
        document.getElementById("password").setAttribute("style", "display: inline-block!important");
        valid2 = false;
    }
    if (valid1 && valid2 && valid3) {
        document.getElementById("SignupSubmit").removeAttribute("disabled");
    }
    else {
        document.getElementById("SignupSubmit").setAttribute("disabled", true);
    }
}

function validationCP(input) {
    if (input.match(document.getElementById("psw").value) && input.length == document.getElementById("psw").value.length && valid2) {
        document.getElementById("cPassword").src = "check.png"
        document.getElementById("cPassword").setAttribute("width", "20px");
        document.getElementById("cPassword").setAttribute("height", "20px");
        document.getElementById("cPassword").setAttribute("style", "display: inline-block!important");
        valid3 = true;
    }
    else if (input == 0) {
        //document.getElementById("SignupSubmit").setAttribute("disabled", true);
        document.getElementById("cPassword").setAttribute("style", "display: none!important");
        valid3 = false;
    }
    else {
        //document.getElementById("SignupSubmit").setAttribute("disabled", true);
        document.getElementById("cPassword").src = "close.png"
        document.getElementById("cPassword").setAttribute("width", "14px");
        document.getElementById("cPassword").setAttribute("height", "14px");
        document.getElementById("cPassword").setAttribute("style", "display: inline-block!important");
        valid3 = false;
    }

    if (valid1 && valid2 && valid3 && valid4) {
        document.getElementById("SignupSubmit").removeAttribute("disabled");
    }
    else {
        document.getElementById("SignupSubmit").setAttribute("disabled", true);
    }
}
//#endregion
//#region show/hide image
$("#post").keypress(function (e) {
    if(e.which === 13 && !e.shiftKey) {
        e.preventDefault();
    
        $(this).closest("form").submit();
    }
});

function showPreview(event, id){
    if(event.target.files.length > 0){
      var src = URL.createObjectURL(event.target.files[0]);
      var preview = document.getElementById(id + "-preview");
      var deletePre = document.getElementById("delete");
      preview.src = src;
      preview.style.display = "block";
      deletePre.style.display = "block";
    }
  }

  function hidePreview(){  
      var preview = document.getElementById("file-ip-1-preview");
      var deletePre = document.getElementById("delete");
      var imgInput = document.getElementById("file-ip-1");
      preview.style.display = "none";
      deletePre.style.display = "none"
      imgInput.value = "";
      preview.src = "";
  }

  function showButton(element){
      element.style.display = "block";
  }
 
  function hideButton(element){
    element.style.display = "none";
}
//#endregion
// var base64string = document.getElementById("image").src;
// var cleanBase64 = base64string.slice(22);
// var decode = atob(cleanBase64);
// console.log(cleanBase64);
// console.log(decode);

//#region modal
// var myModal = document.getElementById('exampleModal')
// var myInput = document.getElementById('myInput')

// myModal.addEventListener('shown.bs.modal', function () {
//   myInput.focus()
// })

function load(idk) {
    if (idk != null) {
        console.log(idk)
        $('#exampleModalLong').modal('show');
    }
    else {
        console.log("empty")
    }
}


// function load(idk){
//     if (idk != null) {
//         $('#myModal').modal('show');
//     }
//     else{
//         console.log("empty")
//     }
// }
//#endregion

