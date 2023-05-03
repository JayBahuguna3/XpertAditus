
//$(document).ready(function () {
//  /*  $(".chosen").chosen();*/
//    var ssClient = new SlimSelect({
//        select: '#ddLocation',
//        placeholder: 'Select Client'
//    });
//});

var valueSkip = 10;
var valueTake = 10;

function IncreasedSkipValue() {
    valueSkip += 10;
    return valueSkip;
}

var paginationIsInProcess = false;

$(document).ready(function () {
    $('.simplebar-scroll-content').scroll(function () {

        let div = $(this).get(0);
        if (div.scrollTop + div.clientHeight >= div.scrollHeight - div.scrollHeight * .2 && paginationIsInProcess == false) {
            paginationIsInProcess = true;
            pagination();
        }
    });
    $('.simplebar-scroll-content').on('touchmove', function () {

        let div = $(this).get(0);
        console.log('scrollTop - ' + div.scrollTop)
        console.log('clientHeight - ' + div.clientHeight)
        console.log('scrollHeight - ' + div.scrollHeight)
        if (div.scrollTop + div.clientHeight >= div.scrollHeight - div.scrollHeight * .2 && paginationIsInProcess == false) {
            paginationIsInProcess = true;
            pagination();
        }
    });
});

function pagination() {
    showSpinner();
    var location = $("#ddLocation option:selected").val();
    var sortBy = parseInt($("#Sortby option:selected").val());
    var category = $("#jobsSearch option:selected").val();
    var keyword = "";

    IncreasedSkipValue();
    var rootPath =
        window.location.protocol + "//" + window.location.host + "/JobDashBoard/CandidateDashBoardOperation?sortBy=" + sortBy + "&category=" + category +
        "&keyword=" + keyword + "&location=" + location + "&valuetake=" + valueTake + "&valueskip=" + valueSkip;

    $.ajax({
        url: rootPath,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            var html = "";
            for (i = 0; i < result.length; i++) { 
                var photoPath = result[i].photoPath == null ? "/images/user-avatar-big-01.jpg" : result[i].photoPath;
                html +=
                        '<div class="freelancer">'+
                        '<div class="freelancer-overview">'+
                        '<div class="freelancer-overview-inner">'+
                        '<span class="bookmark-icon"></span>'+
                        '<div class="freelancer-avatar">'+
                        '<div class="verified-badge"></div>'+
                        '<a href="single-freelancer-profile.html"><img src="/' + photoPath + '" alt=""></a>' +
                        '</div>'+
                        '<div class="freelancer-name">'+
                         '<h4><a href="#">' + result[i].candidateName + '<img class="flag" src="' + photoPath +'" alt="" data-tippy-placement="top" data-tippy="" data-original-title="United Kingdom"></a></h4>'+
                        '<span>Candidate</span>'+
                        '</div>'+
                        '</div>'+
                        '</div>'+
                        '<div class="freelancer-details">' +
                        '<div class="freelancer-details-list">'+
                        '<ul>'+
                        '<li>Location <strong><i class="icon-material-outline-location-on"></i>'+ result[i].cityName +'</strong></li>'+
                        '<li>Score<strong>'+ result[0].scrore +'</strong></li>'+
                        '<li>Course Completed <strong>' + result[0].courseName + '</strong></li>'+
                        '</ul>'+
                        '</div>'+
                        '<a onclick=Navigate(' + result[i].userProfileId +') class="button button-sliding-icon ripple-effect" style="width: 190px;">View Profile <i class="icon-material-outline-arrow-right-alt"></i></a>'+
                        '</div>'+
                        '</div>';
            }
            paginationIsInProcess = false;
            $("#divCandidateList").append(html);
            hideSpinner();
        },
        error: function (data) {
            hideSpinner();
            paginationIsInProcess = false;
        }
    });
}

function filterData() {
    showSpinner();
    valueSkip = 0;
    valueTake = 10;

    var location = $("#ddLocation option:selected").val();
    var sortBy = parseInt($("#Sortby option:selected").val());
    var category = $("#jobsSearch option:selected").val();
    var keyword = "";

    var rootPath =
        window.location.protocol + "//" + window.location.host + "/JobDashBoard/CandidateDashBoardOperation?sortBy=" + sortBy + "&category=" + category +
        "&keyword=" + keyword + "&location=" + location + "&valuetake=" + valueTake + "&valueskip=" + valueSkip;

    $.ajax({
        url: rootPath,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            var html = "";
            for (i = 0; i < result.length; i++) {
                var photoPath = result[i].photoPath == null ? "/images/user-avatar-big-01.jpg" : result[i].photoPath;
                html +=
                    '<div class="freelancer">' +
                    '<div class="freelancer-overview">' +
                    '<div class="freelancer-overview-inner">' +
                    '<span class="bookmark-icon"></span>' +
                    '<div class="freelancer-avatar">' +
                    '<div class="verified-badge"></div>' +
                    '<a href="single-freelancer-profile.html"><img src="/'+ photoPath +'" alt=""></a>' +
                    '</div>' +
                    '<div class="freelancer-name">' +
                    '<h4><a href="#">' + result[i].candidateName + '<img class="flag" src="' + photoPath + '" alt="" data-tippy-placement="top" data-tippy="" data-original-title="United Kingdom"></a></h4>' +
                    '<span>Candidate</span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="freelancer-details">' +
                    '<div class="freelancer-details-list">' +
                    '<ul>' +
                    '<li>Location <strong><i class="icon-material-outline-location-on"></i>' + result[i].cityName + '</strong></li>' +
                    '<li>Score<strong>' + result[0].scrore + '</strong></li>' +
                    '<li>Course Completed <strong>' + result[0].courseName + '</strong></li>' +
                    '</ul>' +
                    '</div>' +
                    '<a onclick=Navigate("' + result[i].userProfileId + '") class="button button-sliding-icon ripple-effect" style="width: 190px;">View Profile <i class="icon-material-outline-arrow-right-alt"></i></a>' +
                    '</div>' +
                    '</div>';
            }
            paginationIsInProcess = false;
            $("#divCandidateList").html(html);
            hideSpinner()
        },
        error: function (data) {
            hideSpinner()
            paginationIsInProcess = false;
        }
    });
}

function Navigate(id) {
    debugger;
    let userProfileID = id;
    var formData = new FormData();
    formData.append('UserProfile_ID', userProfileID);
    $.ajax({
        url: "/CandidateDetail/CandidateDetail",
        type: "POST",
        dataType: "json",
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            window.location.href = "/CandidateDetail/CandidateDetail";
        },
        error: function (data) {
            window.location.href = "/CandidateDetail/CandidateDetail";
        }
    });
    window.location.href = "/CandidateDetail/CandidateDetail";
}