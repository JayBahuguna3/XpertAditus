var password = document.getElementById("txtPsw");
var letter = document.getElementById("letter");
var capital = document.getElementById("capital");
var number = document.getElementById("number");
var length = document.getElementById("length");
var alphanumeric = document.getElementById("alphanumeric");


password.onfocus = function () {
    document.getElementById("message").style.display = "block";
}

// When the user clicks outside of the password field, hide the message box
password.onblur = function () {
    document.getElementById("message").style.display = "none";
}

password.onkeyup = function () {
    // Validate lowercase letters
    var lowerCaseLetters = /[a-z]/g;
    if (password.value.match(lowerCaseLetters)) {
        letter.classList.remove("invalid");
        letter.classList.add("valid");
    } else {
        letter.classList.remove("valid");
        letter.classList.add("invalid");
    }

    // Validate capital letters
    var upperCaseLetters = /[A-Z]/g;
    if (password.value.match(upperCaseLetters)) {
        capital.classList.remove("invalid");
        capital.classList.add("valid");
    } else {
        capital.classList.remove("valid");
        capital.classList.add("invalid");
    }

    // Validate numbers
    var numbers = /[0-9]/g;
    if (password.value.match(numbers)) {
        number.classList.remove("invalid");
        number.classList.add("valid");
    } else {
        number.classList.remove("valid");
        number.classList.add("invalid");
    }

    // Validate length
    if (password.value.length >= 8) {
        length.classList.remove("invalid");
        length.classList.add("valid");
    } else {
        length.classList.remove("valid");
        length.classList.add("invalid");
    }

    var alphanumerics = /[!"#$%&'()*+,-./:;<=>?@@[\]^_`{|}~]/g;
    if (password.value.match(alphanumerics)) {
        alphanumeric.classList.remove("invalid");
        alphanumeric.classList.add("valid");
    } else {
        alphanumeric.classList.remove("valid");
        alphanumeric.classList.add("invalid");
    }
}

var Confirmpassword = document.getElementById("txtConfirmPsw");
var cletter = document.getElementById("ConfirmPswletter");
var ccapital = document.getElementById("ConfirmPswcapital");
var cnumber = document.getElementById("ConfirmPswnumber");
var clength = document.getElementById("ConfirmPswlength");
var calphanumeric = document.getElementById("ConfirmPswalphanumeric");

Confirmpassword.onfocus = function () {
    document.getElementById("Confirmmessage").style.display = "block";
}

// When the user clicks outside of the password field, hide the message box
Confirmpassword.onblur = function () {
    document.getElementById("Confirmmessage").style.display = "none";
}

Confirmpassword.onkeyup = function () {
    // Validate lowercase letters
    var lowerCaseLetters = /[a-z]/g;
    if (Confirmpassword.value.match(lowerCaseLetters)) {
        cletter.classList.remove("invalid");
        cletter.classList.add("valid");
    } else {
        cletter.classList.remove("valid");
        cletter.classList.add("invalid");
    }

    // Validate capital letters
    var upperCaseLetters = /[A-Z]/g;
    if (Confirmpassword.value.match(upperCaseLetters)) {
        ccapital.classList.remove("invalid");
        ccapital.classList.add("valid");
    } else {
        ccapital.classList.remove("valid");
        ccapital.classList.add("invalid");
    }

    // Validate numbers
    var numbers = /[0-9]/g;
    if (Confirmpassword.value.match(numbers)) {
        cnumber.classList.remove("invalid");
        cnumber.classList.add("valid");
    } else {
        cnumber.classList.remove("valid");
        cnumber.classList.add("invalid");
    }

    // Validate length
    if (Confirmpassword.value.length >= 8) {
        clength.classList.remove("invalid");
        clength.classList.add("valid");
    } else {
        clength.classList.remove("valid");
        clength.classList.add("invalid");
    }

    var alphanumerics = /[!"#$%&'()*+,-./:;<=>?@@[\]^_`{|}~]/g;
    if (password.value.match(alphanumerics)) {
        calphanumeric.classList.remove("invalid");
        calphanumeric.classList.add("valid");
    } else {
        calphanumeric.classList.remove("valid");
        calphanumeric.classList.add("invalid");
    }
}

function comparePaswword() {
    var Password = $("#txtPsw").val();
    var ConfimPassword = $("#txtConfirmPsw").val();

    if (Password != ConfimPassword) {
        $("#ComaprePassword").show();
    }
    else {
        GenearteOTP("Generate");
    }
}

(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {

            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                else if (form.checkValidity() === true) {
                    if (form.id == "formRegister") {
                        comparePaswword();
                        event.preventDefault();
                    }
                    else if (form.id == "formOtp") {
                        GenearteOTP("Check");
                        event.preventDefault();
                    }
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();

function GenearteOTP(validationType) {
    clickAndDisable("btnRegister");
    showSpinner();
    var person = new Object();

    if (validationType == "Check") {
        person.MobileOtp = $("#txtMobileOtp").val();
        //person.EmailOtp = $("#txtEmailOtp").val();
    }

    person.Type = $("#employeeType option:selected").val();
    person.FirstName = $("#txtFirstName").val();
    person.MiddleName = $("#txtMiddleName").val();
    person.Lastname = $("#txtLastName").val();
    person.UserName = $("#txtUsername").val();
    person.Email = $("#txtEmail").val();
    person.Password = $("#txtConfirmPsw").val();
    person.MobileNo = $("#txtPhoneNo").val();
    person.validationType = validationType;
    var rootPath = window.location.protocol + "//" + window.location.host + "/api/Registration";
    var error = ""

    $.ajax({
        url: rootPath,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(person),
        success: function (result) {
            removeDisable("btnRegister");
            hideSpinner();
            if (result == true) {
                HideShowmDiv()
                $("#divError").html("");
                if (validationType == "Check") {
                    if ($("#employeeType option:selected").val() == "College") {
                        var domain = $("#txtAiLeadsDomain").val();
                        var rootPath1 = "https://" + domain + "/CollegeProfile/CreateUser?tokan=0120dc4a-7d68-4f5a-9ec3-e4bbca384c73&userName=" + person.UserName + "&Password=" + person.Password + "&Email=" + person.Email + "&mobileNo=" + person.MobileNo;
                        $.ajax({
                            url: rootPath1,
                            type: 'GET',
                            async: false,
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (result) {
                            }
                        });
                    }
                    alert("Register successfully");
                    window.location = window.location.origin + '/Identity/Account/Login';
                }
            }
            else {
                $("#divError").html(result);
                $("#divError").show();
                $('html,body').animate({ scrollTop: 0 });
            }
        },
        error: function (result) {
            removeDisable("btnRegister");
            hideSpinner();
            alert(result);
        }
    });
}

function HideShowmDiv() {
    $("#divRegister").css({
        'display': 'none',
        'opacity': '0',
        'transition': 'visibility 0s linear 0.33s, opacity 0.33s linear'
    });

    $("#divOtp").show();

    $('html,body').animate({ scrollTop: 0 });
}