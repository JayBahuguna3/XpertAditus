$(document).ready(function () {
    GetQualification();
    GetExperience();
    getCountries();
    getAddressInfo();
});

function HighestQualificationChanged() {
    if ($(event.target).is(':checked')) {
        $('[id^=chkHighestQualification]').attr('disabled', '');
        $(event.target).removeAttr('disabled');

    }
    else {
        $('[id^=chkHighestQualification]').removeAttr('disabled');

    }
}

function AddQualification() {
var i = parseInt($("#txtChronologicalQualificationOrder").val()) + 1 ; 
    var result = '<div id="divQualification' + i + '_Button" class="col-xl-12 btn " style="text-align:right; margin-bottom: 15px;">' +
                '<a class="button ripple-effect medium" onclick="RemoveQualification(' + i + ')" style ="color: white;"><i class="icon-material-outline-delete"></i> Remove</a>' +
        '</div>' 
        +'<div  id="divQualification' + i + '_Remove"  class="col-xl-12">'+
                            '<div class="submit-field">'+
                               ' <div>'+
        '<input id="chkHighestQualification_' + i + '" type="checkbox" onchange=HighestQualificationChanged()>'+
                                   ' <label for="" style="padding-left:5px; display:inline;">IsHighestQualification</label>'+
                               ' </div>'+
                          '  </div>'+
                        '</div>'+
                '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +

        '<h5>Qualification</h5>' +
        '<select id="txtQualification_' + i + '" name="EducationId" class="with-border form-control" required="">'
        + $('#txtQualification_0').html() +
                + `</select>`+
                //'<select id="txtQualification_' + i + '" type="text" name="Qualification" class="with-border form-control" value="" required>' +
                '<input id="txtQualificationId_' + i + '" type="text" name="Id" class="with-border form-control" style="display:none" value="">'+
                '<div class="invalid-feedback">' +
                'Please Enter Qualification' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                '<div class="submit-field">' +
                '<h5>University Name</h5>' +
                '<input id="txtUniversityName_' + i + '" type="text" name="UniversityName" class="with-border form-control" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter University Name' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                '<div class="submit-field">' +
                '<h5>Completion Year</h5>' +
                '<input id="txtCompletionYear_' + i + '" type="date" name="CompletionYear" class="with-border form-control" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter Completion Year' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                '<div class="submit-field">' +

        
                '<h5>Percentage</h5>' +
                '<input readonly id="txtPercentage_' + i + '" type="number" name="Percentage" class="with-border form-control" step="0.01" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter Percentage' +

                /*'<h5>Percentage</h5>' +
                '<input readonly id="txtPercentage_' + i + '" type="number" name="Percentage" class="with-border form-control" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter Percentage' +*/
                '</div>' +
                '</div>' +
                '</div>' +
                 '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                '<div class="submit-field">' +
                '<h5>Total Marks</h5>' +
                '<input id="txtTotalMarks_' + i + '" type="number" name="TotalMarks" class="with-border form-control" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter Total Marks' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                '<div class="submit-field">' +
                '<h5>Marks Obtained</h5>' +
                '<input id="txtMarksObtained_' + i + '" type="number" name="MarksObtained" class="with-border form-control" value="" required>' +
                '<div class="invalid-feedback">' +
                'Please Enter Marks Obtained' +
                '</div>' +
                '</div>' +
                '</div>';
                //'<div class="col-xl-6" style="display:none">' +
                //'<div class="submit-field">' +
                //'<h5>Chronological Order</h5>' +
                //'<input id="txtChronologicalQualificationOrder" name="ChronologicalQualificationOrder" type="text" class="with-border" value="0">' +
                //'</div>' +
                //'</div>';
    $("#txtChronologicalQualificationOrder").val(i);
    $("#divQualification").append(result)
    $('html,body').animate({ scrollTop: $("#divQualification" + i + "_Button").offset().top }, 'slow');

    //$("#txtPercentage_")(function () {
    //    if (i != (i - 1)) {
    //        var isDisabled = $(this).is(":checked");
        //}
        //var cb1 = $('#percentage_' + i).is(":checked");
        //var cb2 = $('#CGPA_' + i).is(":checked");

        //var marksObtained = document.getElementById("txtMarksObtained_" + i).value;
        //var totalMarks = document.getElementById("txtTotalMarks_" + i).value;

        //var calPercentage = 0;
        //var calPercentage = (marksObtained / totalMarks) * 100;
        //$('#txtPercentage_' + i).empty().val(calPercentage.toFixed(2));

        //for Percentage
        /*if (cb1 == true) {
            $('#txtPercentage_' + i).empty().val(calPercentage.toFixed(2));

            //$('#CGPA_' + i).attr("disabled", isDisabled);
        }
        else {
            $('#txtPercentage_' + i).val("");
            //$('#CGPA_' + i).attr("disabled", false);
        }*/

        //for CGPA
        /*if (cb2 == true) {
            calCGPA = calPercentage / 9.5;
            $('#txtCGPA_' + i).empty().val(calCGPA.toFixed(2));
            $('#percentage_' + i).attr("disabled", isDisabled);
        }
        else {

            $('#txtCGPA_' + i).val("");
            $('#percentage_' + i).attr("disabled", false);
        }*/

    //});
        //$("#txtMarksObtained_" + i).change(function () {
        //    var TotalMarks = $("#txtTotalMarks_" + i).val();
        //    var MarksObtained = $("#txtMarksObtained_" + i).val();
        //    if (Number(TotalMarks) <= Number(MarksObtained)) {
        //        alert("Marks Obtained should be less than Total Marks.");
        //    }
        //});
}




function AddExprience() {
    var i = parseInt($("#txtChronologicalOrder").val()) + 1;
    var result =
        '<div id="divExperience' + i + '_Button" class="col-xl-12 btn " style="text-align:right; "margin-bottom:10px">' +
        '<a class="button ripple-effect medium" onclick="RemoveExperience(' + i + ')" style ="color: white;"><i class="icon-material-outline-delete"></i> Remove</a>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +
        '<h5>Company Name</h5>' +
        '<input id="txtCompanyName_' + i + '" type="text" name="CompanyName" class="with-border form-control" value="" required>' +
        '<input id="txtExperienceId_' + i + '" type="text" name="Id" class="with-border form-control" style="display:none" value="">' +
        '<div class="invalid-feedback">' +
        'Please Enter Company Name' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +
        '<h5>Start Date</h5>' +
        '<input id="txtStartDate_' + i + '" type="date" name="StartDate" class="with-border form-control" value="" required>' +
        '<div class="invalid-feedback">' +
        'Please Enter StartDate' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +
        '<h5>End Date</h5>' +
        '<input id="txtEndtDate_' + i + '" type="date" name="EndtDate" class="with-border form-control" value="" required>' +
        '<div class="invalid-feedback">' +
        'Please Enter End tDate' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +
        '<h5>Designation Name</h5>' +
        '<input id="txtDesignationName_'+ i + '" type="text" name="DesignationName" class="with-border form-control" value="" required>' +
        '<div class="invalid-feedback">' +
        'Please Enter Designation Name' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
        '<div class="submit-field">' +
        '<h5>Company Contact</h5>' +
        '<input id="txtCompanyContact_' + i + '" type="number" name="CompanyContact" class="with-border form-control" value="" required>' +
        '<div class="invalid-feedback">' +
        'Please Enter Company Contact' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div id="divExperience' + i + '_Remove" class="col-xl-6" style="visibility:hidden">'+
        '<div class="submit-field">' +
        '<h5>Chronological Order</h5>' +
        '<input id="txtChronologicalOrder_0" name="ChronologicalOrde" type="text" class="with-border form-control" value="0">' +
        '</div>' +
        '</div>';
    $("#txtChronologicalOrder").val(i);
    $("#divExperience").append(result)
    $('html,body').animate({ scrollTop: $("#divExperience" + i + "_Button").offset().top }, 'slow');
}


(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            var error = false;
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                else if (form.checkValidity() === true) {
                    if (form.id == "formQualification") {
                        saveQualification();
                        event.preventDefault();
                        error = true;
                    }
                    else if (form.id == "formExperience") {
                        saveExperience();
                        event.preventDefault();
                        error = true;
                    }
                    else if (form.id == "formAddress") {
                        saveAddress();
                        event.preventDefault();
                        error = true;
                    }
                }
                form.classList.add('was-validated');
                if (error) {
                    form.classList.remove('was-validated');
                }
            }, false);
        });
    }, false);
})();

function RemoveQualification(id) {
    showSpinner();

    var QualificationId = $("#txtQualificationId_" + id).val();
    var Name = $("#txtQualification_" + id).val();
    var CompletionYear = $("#txtCompletionYear_" + id).val();
    var UniversityName = $("#txtUniversityName_" + id).val();
    var Percentage = $("#txtPercentage_" + id).val();
    var TotalMarks = $("#txtTotalMarks_" + id).val();
    var MarksObtained = $("#txtMarksObtained_" + id).val();
   

    if (id == 0) {
        $("#txtQualificationId_0").val("");
        $("#txtQualification_0").val("");
        $("#txtUniversityName_0").val("");
        $("#txtCompletionYear_0").val("");
        $("#txtPercentage_0").val("");
        $("#txtTotalMarks_0").val("");
        $("#txtMarksObtained_0").val("");
        $("#chkHighestQualification_0").attr("checked", false);
        hideSpinner();
    }

    if (QualificationId != "") {
        var selectedData = [];
        selectedData.push({
            "QualificaitonId": QualificationId,
            "Name": Name,
            "UniversityName": UniversityName,
            "CompletionYear": CompletionYear,
            "Percentage": 0,
            "TotalMarks": 0,
            "MarksObtained": 0,
        });
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Qualification";
        $.ajax({
            url: rootPath,
            type: 'DELETE',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(selectedData),
            success: function (result) {
                hideSpinner();
                alert("Remove successfully");
            }
        });
    }
    if (id != 0) {
        $("[id ^= 'divQualification" + id + "']").remove();
        hideSpinner();
    }
}

function RemoveExperience(id) {
    showSpinner()
    var ExperienceId = $("#txtExperienceId_" + id).val();
    var CompanyName = $("#txtCompanyName_" + id).val();
    var StartDate = $("#txtStartDate_" + id).val();
    var EndDate = $("#txtEndtDate_" + id).val();
    var DesignationName = $("#txtDesignationName_" + id).val();
    var CompanyContact = $("#txtCompanyContact_" + id).val();

    var selectedData = [];

    if (id == 0) {
        $("#txtExperienceId_0").val("");
        $("#txtCompanyName_0").val("");
        $("#txtStartDate_0").val("");
        $("#txtEndtDate_0").val("");
        $("#txtDesignationName_0").val("");
        $("#txtCompanyContact_0").val("");
        hideSpinner();
    }

    selectedData.push({
        "ExperienceId": ExperienceId,
        "CompanyName": CompanyName,
        "StartDate": StartDate,
        "EndDate": EndDate,
        "DesignationName": DesignationName,
        "CompanyContact": CompanyContact,
    });

    if (ExperienceId != "") {
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Experience";
        $.ajax({
            url: rootPath,
            type: 'DELETE',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(selectedData),
            success: function (result) {
                hideSpinner();
                alert("Remove successfully");
                
            }
        });
    }
    if (id != 0) {
        $("[id ^= 'divExperience" + id + "']").remove();
        hideSpinner();
    }
}

function saveQualification() {
    showSpinner();
    var count = parseInt($("#txtChronologicalQualificationOrder").val());
    var highestQualificationmark = false;
    var selectedData = [];
    for (i = 0; i <= count; i++) {
        if ($("#divQualification" + i + "_Remove").html() != undefined) {
        var highestQualification = $("#chkHighestQualification_" + i).is(':checked');
        if (highestQualification == true && highestQualificationmark == false) {
            highestQualificationmark = true;
        }
        else if (highestQualification == true && highestQualificationmark == true) {
            hideSpinner();
            alert("You can not check multiple highest qualification checkBox");
            return false;
        }
        var QualificaitonId = $("#txtQualificationId_" + i).val();
        var EducationId = $("#txtQualification_" + i).val();
        var UniversityName = $("#txtUniversityName_" + i).val();
        var CompletionYear = $("#txtCompletionYear_" + i).val();
        var Percentage = $("#txtPercentage_" + i).val();
        if (Percentage == "")
            Percentage = null;
        var TotalMarks = $("#txtTotalMarks_" + i).val();
        var MarksObtained = $("#txtMarksObtained_" + i).val();
        if (parseInt(TotalMarks) < parseInt(MarksObtained)) {
            hideSpinner();
            alert("Marks Obtained should be less than Total Marks.");
            return false;
        }
        Percentage = (MarksObtained / TotalMarks) * 100;
        $('#txtPercentage_' + i).empty().val(Percentage.toFixed(2));

        if (QualificaitonId == "") {
            QualificaitonId = "00000000-0000-0000-0000-000000000000";
        }

        selectedData.push({
            "QualificaitonId": QualificaitonId,
            "EducationId": EducationId,
            "UniversityName": UniversityName,
            "CompletionYear": CompletionYear,
            "Percentage": Percentage,
            "TotalMarks": TotalMarks,
            "MarksObtained": MarksObtained,
            "IsHighestQualification": highestQualification,
            "Name": EducationId,

        });
    }
}

    if (selectedData.length > 0) {
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Qualification";
        var error = ""

        $.ajax({
            url: rootPath,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(selectedData),
            success: function (result) {
                hideSpinner();
                alert("Save successfully");
                window.location.reload();
            }
        });
    }
    
}

    function saveExperience() {
        showSpinner();
        var count = parseInt($("#txtChronologicalOrder").val());
        var selectedData = [];
        for (i = 0; i <= count; i++) {
            var ExperienceId = $("#txtExperienceId_" + i).val();
            var CompanyName = $("#txtCompanyName_" + i).val();
            var StartDate = $("#txtStartDate_" + i).val();
            var EndDate = $("#txtEndtDate_" + i).val();
            var DesignationName = $("#txtDesignationName_" + i).val();
            var CompanyContact = $("#txtCompanyContact_" + i).val();

            var CcLen = CompanyContact.length;

            if (StartDate > EndDate) {
                alert("End Date should be greater than Start Date.");
                hideSpinner();
                return false;
            }
            if (CcLen != 10) {
                alert("Invalid Mobile number");
                hideSpinner();
                return false;
            }


            if (ExperienceId == "") {
                ExperienceId = "00000000-0000-0000-0000-000000000000";
            }

            selectedData.push({
                "ExperienceId": ExperienceId,
                "CompanyName": CompanyName,
                "StartDate": StartDate,
                "EndDate": EndDate,
                "DesignationName": DesignationName,
                "CompanyContact": CompanyContact,
            });

        }

        if (selectedData.length > 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/api/Experience";
            var error = ""

            $.ajax({
                url: rootPath,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(selectedData),
                success: function (result) {
                    hideSpinner();
                    alert("Save successfully");
                }
            });
        }
    }

    function GetQualification() {
        var HTML = "";
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Qualification";
        $.ajax({
            url: rootPath,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function (result) {
                for (i = 0; i <= result.length - 1; i++) {
                    if (i == 0) {
                        var now = new Date(result[i].completionYear);
                        var day = ("0" + now.getDate()).slice(-2);
                        var month = ("0" + (now.getMonth() + 1)).slice(-2);

                        var completionYear = now.getFullYear() + "-" + (month) + "-" + (day);
                        if (result[i].isHighestQualification)
                            $("#chkHighestQualification_0").attr('checked', '')
                        $("#txtQualificationId_0").val(result[i].qualificaitonId);
                        $("#txtQualification_0").val(result[i].name);
                        $("#txtUniversityName_0").val(result[i].universityName);
                        $("#txtCompletionYear_0").val(completionYear);
                        $("#txtPercentage_0").val(result[i].percentage);
                        $("#txtTotalMarks_0").val(result[i].totalMarks);
                        $("#txtMarksObtained_0").val(result[i].marksObtained);
                        $("#txtChronologicalQualificationOrder").val(result.length - 1);
                    }
                    else {

                        var now = new Date(result[i].completionYear);
                        var day = ("0" + now.getDate()).slice(-2);
                        var month = ("0" + (now.getMonth() + 1)).slice(-2);

                        var completionYear = now.getFullYear() + "-" + (month) + "-" + (day);

                        HTML +=
                            '<div id="divQualification' + i + '_Button" class="col-xl-12 btn " style="text-align:center; margin-bottom: 15px;">' +
                            '<a class="button ripple-effect medium" onclick="RemoveQualification(' + i + ')" style ="color: white;"><i class="icon-material-outline-delete"></i> Remove</a>' +
                        '</div>' 

                        +'<div  id="divQualification' + i + '_Remove"  class="col-xl-12">' +
                        '<div class="submit-field">' +
                        ' <div>' +
                        '<input onchange=HighestQualificationChanged()  id="chkHighestQualification_' + i + '" type="checkbox" ' + (result[i].isHighestQualification ? "checked" : "''")  + '>' +
                        ' <label for="" style="padding-left:5px; display:inline;">IsHighestQualification</label>' +
                        ' </div>' +
                        '  </div>' +
                        '</div>' +
                        '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +

                            '<div class="submit-field">' +
                            '<h5>Qualification</h5>' +
                          //  '<input id="txtQualification_' + i + '" type="text" name="Qualification" class="with-border form-control" value="' + result[i].name + '" required>' +
                        `<select  id="txtQualification_` + i + `" class="educationselect with-border form-control" required="" value="` + result[i].educationId +`">` +
                        $('#txtQualification_0').html() + `

                        </select>` +
                        '<input id="txtQualificationId_' + i + '" type="text" name="Id" class="with-border form-control" style="display:none" value="' + result[i].qualificaitonId + '">' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Qualification' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>University Name</h5>' +
                            '<input id="txtUniversityName_' + i + '" type="text" name="UniversityName" class="with-border form-control" value="' + result[i].universityName + '" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter University Name' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Completion Year</h5>' +
                            '<input id="txtCompletionYear_' + i + '" type="date" name="CompletionYear" class="with-border form-control" value="' + completionYear + '" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Completion Year' +
                            '</div>' +
                            '</div>' +
                        '</div>' +



                            /*'<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">';*/

                        /*if (result[i].percentage != 0) {
                            HTML +=
                                //'<input checked id="percentage_' + i + '" class="checkbox1" type="checkbox" name="checkbox1" style=" height: 40px; width: 20px;"/>' +
                                '<h5>Percentage</h5>' +
                            '<input readonly id="txtPercentage_' + i + '" type="text" name="Percentage" class="with-border form-control" value=" " ' + result[i].percentage + ' required>';
                        }
                        else {
                            HTML +=
                                //'<input disabled id="percentage_' + i + '" class="checkbox1" type="checkbox" name="checkbox1" style=" height: 40px; width: 20px;"/>' +
                                '<h5>Percentage</h5>' +
                            '<input readonly id="txtPercentage_' + i + '" type="text" name="Percentage" class="with-border form-control" value="0" required>';
                        }


                        HTML +=
                            '<div class="invalid-feedback">' +
                            'Please Enter Percentage' +*/


                        /*if (result[i].TotalMarks != null && result[i].MarksObtained != null) {
                            HTML +=
                                ///'<input checked id="percentage_' + i + '" class="checkbox1" type="checkbox" name="checkbox1" style=" height: 40px; width: 20px;"/>' +
                                '<h5>Percentage</h5>' +
                                '<input readonly id="txtPercentage_' + i + '" type="text" name="Percentage" class="with-border form-control" value="' + result[i].percentage + '" required>';
                        }
                        else {
                            HTML +=
                                //'<input disabled id="percentage_' + i + '" class="checkbox1" type="checkbox" name="checkbox1" style=" height: 40px; width: 20px;"/>' +
                                '<h5>Percentage</h5>' +
                                '<input readonly id="txtPercentage_' + i + '" type="text" name="Percentage" class="with-border form-control" value="0" required>';
                        }


                        HTML +=
                            '<div class="invalid-feedback">' +
                            'Please Enter Percentage' +*/


                            '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                        '<div class="submit-field">' +
                        //'<input id="txtPercentage_' + i + '" type="number" name="Percentage" class="with-border form-control" value="0" >' +
                            '<h5>Percentage</h5>' +
                        '<input readonly id="txtPercentage_' + i + '" type="number" name="Percentage" class="with-border form-control" value="' + result[i].percentage + '" step="0.01" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Percentage' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Total Marks</h5>' +
                            '<input id="txtTotalMarks_' + i + '" type="number" name="TotalMarks" class="with-border form-control" value="' + result[i].totalMarks + '" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Total Marks' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divQualification' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Marks Obtained</h5>' +
                            '<input id="txtMarksObtained_' + i + '" type="number" name="MarksObtained" class="with-border form-control" value="' + result[i].marksObtained + '" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Marks Obtained' +
                            '</div>' +
                            '</div>' +
                            '</div>';
                    }

                }
                $("#divQualification").append(HTML);
                $('.educationselect').each(function (i, obj) {
                    $(obj).val($(obj).attr('value'))
                });
                if ($('[id^=chkHighestQualification]:checked').length > 0) {
                    $('[id^=chkHighestQualification]:not(:checked)').attr('disabled', '');
                }
                
                
            }
        });
    }


    function GetExperience() {
        var HTML = "";
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Experience";
        $.ajax({
            url: rootPath,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function (result) {
                for (i = 0; i <= result.length - 1; i++) {
                    if (i == 0) {

                        var startDatenow = new Date(result[i].startDate);
                        var startDateday = ("0" + startDatenow.getDate()).slice(-2);
                        var startDatemonth = ("0" + (startDatenow.getMonth() + 1)).slice(-2);
                        var startDate = startDatenow.getFullYear() + "-" + (startDatemonth) + "-" + (startDateday);

                        var endDatenow = new Date(result[i].endDate);
                        var endDatenowday = ("0" + endDatenow.getDate()).slice(-2);
                        var endDatenowmonth = ("0" + (endDatenow.getMonth() + 1)).slice(-2);
                        var endDate = endDatenow.getFullYear() + "-" + (endDatenowmonth) + "-" + (endDatenowday);

                        $("#txtExperienceId_0").val(result[i].experienceId);
                        $("#txtCompanyName_0").val(result[i].companyName);
                        $("#txtStartDate_0").val(startDate);
                        $("#txtEndtDate_0").val(endDate);
                        $("#txtDesignationName_0").val(result[i].designationName);
                        $("#txtCompanyContact_0").val(result[i].companyContact);
                        $("#txtChronologicalOrder").val(result.length - 1);
                    }
                    else {

                        var startDatenow = new Date(result[i].startDate);
                        var startDateday = ("0" + startDatenow.getDate()).slice(-2);
                        var startDatemonth = ("0" + (startDatenow.getMonth() + 1)).slice(-2);
                        var startDate = startDatenow.getFullYear() + "-" + (startDatemonth) + "-" + (startDateday);

                        var endDatenow = new Date(result[i].endDate);
                        var endDatenowday = ("0" + endDatenow.getDate()).slice(-2);
                        var endDatenowmonth = ("0" + (endDatenow.getMonth() + 1)).slice(-2);
                        var endDate = endDatenow.getFullYear() + "-" + (endDatenowmonth) + "-" + (endDatenowday);

                        HTML +=
                            '<div id="divExperience' + i + '_Button" class="col-xl-12 btn " style="text-align:right; "margin-bottom:10px">' +
                            '<a class="button ripple-effect medium" onclick="RemoveExperience(' + i + ')" style ="color: white;"><i class="icon-material-outline-delete"></i> Remove</a>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Company Name</h5>' +
                            '<input id="txtCompanyName_' + i + '" type="text" name="CompanyName" class="with-border form-control" value="' + result[i].companyName + '" required>' +
                            '<input id="txtExperienceId_' + i + '" type="text" name="Id" class="with-border form-control" style="display:none" value="' + result[i].experienceId + '">' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Company Name' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Start Date</h5>' +
                        '   <input id="txtStartDate_' + i + '" type="date" name="StartDate" class="with-border form-control" value="' + startDate + '" max="9999-12-31" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter StartDate' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>End Date</h5>' +
                        '<input id="txtEndtDate_' + i + '" type="date" name="EndtDate" class="with-border form-control" value="' + endDate + '"max="9999-12-31" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter End tDate' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Designation Name</h5>' +
                            '   <input id="txtDesignationName_' + i + '" type="text" name="DesignationName" class="with-border form-control" value="' + result[i].designationName + '" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Designation Name' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6 form-group">' +
                            '<div class="submit-field">' +
                            '<h5>Company Contact</h5>' +
                        '<input id="txtCompanyContact_' + i + '" type="number" name="CompanyContact" class="with-border form-control" value="' + result[i].companyContact + '" maxlength="10" required>' +
                            '<div class="invalid-feedback">' +
                            'Please Enter Company Contact' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div id="divExperience' + i + '_Remove" class="col-xl-6" style="visibility:hidden">' +
                            '<div class="submit-field">' +
                            '<h5>Chronological Order</h5>' +
                            '<input id="txtChronologicalOrder_' + i + '" name="ChronologicalOrde" type="text" class="with-border form-control" value="' + i + '">' +
                            '</div>' +
                            '</div>';
                    }
                }
                $("#divExperience").append(HTML);
            }
        });
    }

    function saveAddress() {
        showSpinner();
        var selectedData = [];

        for (i = 0; i <= 1; i++) {
            var type = "";

            if (i == 0) {
                type = "home"
            }
            else {
                type = "office"
            }

            selectedData.push({
                "AddressId": $("#txt" + type + "AddressId").val() == "" ? "00000000-0000-0000-0000-000000000000" : $("#txt" + type + "AddressId").val(),
                "Line1": $("#" + type + "Line_1").val(),
                "Line2": $("#" + type + "Line_2").val(),
                "Line3": $("#" + type + "Line_3").val(),
                "CountryId": $("#" + type + "country option:selected").val() == "0" ? "00000000-0000-0000-0000-000000000000" : $("#" + type + "country option:selected").val(),
                "StateId": $("#" + type + "state option:selected").val() == "0" ? "00000000-0000-0000-0000-000000000000" : $("#" + type + "state option:selected").val(),
                "DistrictId": $("#" + type + "district option:selected").val() == "0" ? "00000000-0000-0000-0000-000000000000" : $("#" + type + "district option:selected").val(),
                "CityId": $("#" + type + "city option:selected").val() == "0" ? "00000000-0000-0000-0000-000000000000" : $("#" + type + "city option:selected").val(),
                "AddressType": i == 0 ? "Home" : "Office",
            });
        }

        if (selectedData.length > 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/api/Address";
            var error = ""

            $.ajax({
                url: rootPath,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(selectedData),
                success: function (result) {
                    hideSpinner();
                    alert("Address Saved Successfully!");
                    window.location.href = "/Home/Index ";
                }
            });
        }
    }

    function getCountries() {
        var countries = ""
        var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetCountries";
        $.ajax({
            url: rootPath,
            type: 'GET',
            async: false,
            success: function (result) {
                if (typeof result == 'string')
                    result = JSON.parse(result);
                for (i = 0; i <= result.length - 1; i++) {
                    countries += "<option value=" + result[i].countryId + ">" + result[i].name + "</option>"
                }
                $("#homecountry").append(countries);
                $("#officecountry").append(countries);
            }
        });
    }

    function getHomesStates() {
        var states = "";
        var countryId = $("#homecountry option:selected").val();
        if (countryId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetStates";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: countryId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        states += "<option value=" + result[i].stateId + ">" + result[i].name + "</option>"
                    }
                    $("#homestate").append(states);
                }
            });
        }
    }

    function getHomesDistricts() {
        var district = "";
        var stateId = $("#homestate option:selected").val();
        if (stateId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetDistricts";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: stateId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        district += "<option value=" + result[i].districtId + ">" + result[i].name + "</option>"
                    }
                    $("#homedistrict").empty();
                    $("#homedistrict").append(district);
                }
            });
        }
    }

    function getHomesCitys() {
        var city = "";
        var districtId = $("#homedistrict option:selected").val();
        if (districtId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetCities";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: districtId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        city += "<option value=" + result[i].cityId + ">" + result[i].name + "</option>"
                    }
                    $("#homecity").empty();
                    $("#homecity").append(city);
                }
            });
        }
    }

    function getOfficeStates() {
        var states = "";
        var countryId = $("#officecountry option:selected").val();
        if (countryId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetStates";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: countryId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        states += "<option value=" + result[i].stateId + ">" + result[i].name + "</option>"
                    }
                    $("#officestate").append(states);
                }
            });
        }
    }

    function getOfficeDistricts() {
        var district = "";
        var stateId = $("#officestate option:selected").val();
        if (stateId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetDistricts";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: stateId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        district += "<option value=" + result[i].districtId + ">" + result[i].name + "</option>"
                    }
                    $("#officedistrict").empty();
                    $("#officedistrict").append(district);
                }
            });
        }
    }

    function getOfficeCitys() {
        var city = "";
        var districtId = $("#officedistrict option:selected").val();
        if (districtId != 0) {
            var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetCities";
            $.ajax({
                url: rootPath,
                type: 'POST',
                data: {
                    Id: districtId.toString()
                },
                success: function (result) {
                    if (typeof result == 'string')
                        result = JSON.parse(result);
                    for (i = 0; i <= result.length - 1; i++) {
                        city += "<option value=" + result[i].cityId + ">" + result[i].name + "</option>"
                    }
                    $("#officecity").empty();
                    $("#officecity").append(city);
                }
            });
        }
    }

    function getAddressInfo() {
        var rootPath = window.location.protocol + "//" + window.location.host + "/api/Address";

        var homestate = "";
        var officestate = "";

        var homedistrict = "";
        var officedistrict = "";

        var homecity = "";
        var officecity = "";

        $.ajax({
            url: rootPath,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function (result) {
                for (i = 0; i <= result.addressInfo.length - 1; i++) {
                    if (result.addressInfo[i].addressType == "Office") {
                        $("#txtofficeAddressId").val(result.addressInfo[i].addressId);
                        $("#officeLine_1").val(result.addressInfo[i].line1);
                        $("#officeLine_2").val(result.addressInfo[i].line2);
                        $("#officeLine_3").val(result.addressInfo[i].line3);
                        if (result.addressInfo[i].countryId != null) {
                            $("#officecountry").val(result.addressInfo[i].countryId);
                        }

                        if (result.officeAddress != null) {
                            if (result.officeAddress.stateName != null) {
                                officestate += "<option selected value=" + result.addressInfo[i].stateId + ">" + result.officeAddress.stateName + "</option>";
                                $("#officestate").append(officestate);
                            }

                            if (result.officeAddress.districtName != null) {
                                officedistrict += "<option selected value=" + result.addressInfo[i].districtId + ">" + result.officeAddress.districtName + "</option>";
                                $("#officedistrict").append(officedistrict);
                            }

                            if (result.officeAddress.cityName != null) {
                                officecity += "<option selected value=" + result.addressInfo[i].cityId + ">" + result.officeAddress.cityName + "</option>";
                                $("#officecity").append(officecity);
                            }
                        }
                    }
                    else if (result.addressInfo[i].addressType == "Home") {
                        $("#txthomeAddressId").val(result.addressInfo[i].addressId);
                        $("#homeLine_1").val(result.addressInfo[i].line1);
                        $("#homeLine_2").val(result.addressInfo[i].line2);
                        $("#homeLine_3").val(result.addressInfo[i].line3);

                        if (result.addressInfo[i].countryId != null) {
                            $("#homecountry").val(result.addressInfo[i].countryId);
                        }

                        if (result.homeAddress != null) {
                            if (result.homeAddress.stateName != null) {
                                homestate += "<option selected value=" + result.addressInfo[i].stateId + ">" + result.homeAddress.stateName + "</option>";
                                $("#homestate").append(homestate);
                            }

                            if (result.homeAddress.districtName != null) {
                                homedistrict += "<option selected value=" + result.addressInfo[i].districtId + ">" + result.homeAddress.districtName + "</option>";
                                $("#homedistrict").append(homedistrict);
                            }

                            if (result.homeAddress.cityName != null) {
                                homecity += "<option selected value=" + result.addressInfo[i].cityId + ">" + result.homeAddress.cityName + "</option>";
                                $("#homecity").append(homecity);
                            }
                        }
                    }
                }
            }
        });
    }
