var lower = /[a-z][A-Z][0-9]/g;
var emailVal = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
var valid1 = false;
var valid2 = false;
var valid3 = false;

function validationE(input) {
    if (input.match(emailVal) && input.length >= 8) {
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

    if (valid1 && valid2 && valid3) {
        document.getElementById("SignupSubmit").removeAttribute("disabled");
    }
    else {
        document.getElementById("SignupSubmit").setAttribute("disabled", true);
    }
}