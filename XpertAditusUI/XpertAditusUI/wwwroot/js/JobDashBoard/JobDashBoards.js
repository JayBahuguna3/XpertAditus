/*$("#txtkeyword").change(filterData());*/

$("#txtkeyword").change(function () {
    filterData();
});

$("#findCity").change(function () {
    filterData();
});

function filterData() {
    //$("#divJobList").html("");
    var sortBy = parseInt($("#Sortby option:selected").val());
    var category = $("#Category option:selected").val();
    var city = $("#findCity").val();
    var keyword = $("#txtkeyword").val();
    var SalaryRange1 = $("#chk1")[0].checked;
    var SalaryRange2 = $("#chk2")[0].checked;
    var SalaryRange3 = $("#chk3")[0].checked;
    var SalaryRange4 = $("#chk4")[0].checked;
    var SalaryRange5 = $("#chk5")[0].checked;
    valueSkip = 0;
    var rootPath =
        window.location.origin + "/JobDashBoard/JobDashBoardsOperation?sortBy=" + sortBy + "&category=" + category + "&city=" + city +
        "&keyword=" + keyword + "&SalaryRange1=" + SalaryRange1 + "&SalaryRange2=" + SalaryRange2 + "&SalaryRange3=" + SalaryRange3 + "&SalaryRange4=" + SalaryRange4 + "&SalaryRange5=" + SalaryRange5 + "&valuetake=" + valueTake + "&valueskip=" + valueSkip;;
    $.ajax({
        url: rootPath,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            var html = "";
            for (i = 0; i < result.length; i++) {
                html +=
                    '<a href="/JobDashBoard/JobInfo?jobId=' + result[i].jobId + '" class="job-listing">' +
                    '<div class="job-listing-details">' +
                    '<div class="job-listing-company-logo">' +
                    '</div>' +
                    '<div class="job-listing-description">' +
                    '<h4 class="job-listing-company">' + result[i].companyName + '<span class="verified-badge" title="Verified Employer" data-tippy-placement="top"></span></h4>' +
                    '<h3 class="job-listing-title">' + result[i].jobDesignation + '</h3>' +
                    '<p class="job-listing-text">' + result[i].description + '</p>' +
                    '</div>' +
                    '<span class="bookmark-icon"></span>' +
                    '</div>' +
                    '<div class="job-listing-footer">' +
                    '<ul>' +
                '<li><i class="icon-material-outline-location-on"></i> ' + result[i].cityName + '</li>' +
                    '<li><i class="icon-material-outline-business-center"></i>' + result[i].workShift + '</li>' +
                    '<li><i class="icon-material-outline-account-balance-wallet"></i>' + result[i].ctc + '</li>' +
                    '<li><i class="icon-material-outline-access-time"></i>' + Math.ceil(Math.abs(new Date() - new Date(result[i].createdDate)) / (1000 * 60 * 60 * 24)) + ' days ago</li>' +
                    '</ul>' +
                    '</div>' +
                    '</a>';
            }
            $("#divJobList").html(html);

        }

    });
}


function pagination() {
    var sortBy = parseInt($("#Sortby option:selected").val());
    var category = $("#Category option:selected").val();
    var city = $("#findCity").val();
    var keyword = $("#txtkeyword").val();
    var SalaryRange1 = $("#chk1")[0].checked;
    var SalaryRange2 = $("#chk2")[0].checked;
    var SalaryRange3 = $("#chk3")[0].checked;
    var SalaryRange4 = $("#chk4")[0].checked;
    var SalaryRange5 = $("#chk5")[0].checked;
    var currentValue = valueSkip;
    IncreasedValue();
    var rootPath =
        window.location.protocol + "//" + window.location.host + "/JobDashBoard/JobDashBoardsOperation?sortBy=" + sortBy + "&category=" + category + "&city=" + city +
        "&keyword=" + keyword + "&SalaryRange1=" + SalaryRange1 + "&SalaryRange2=" + SalaryRange2 + "&SalaryRange3=" + SalaryRange3 + "&SalaryRange4=" + SalaryRange4 + "&SalaryRange5=" + SalaryRange5 + "&valuetake=" + valueTake + "&valueskip=" + currentValue;;
    $.ajax({
        url: rootPath,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {

            //var html = $("#divJobList").html();
            for (i = 0; i < result.length; i++) {
                html +=
                    '<a href="/JobDashBoard/JobInfo?jobId=' + result[i].jobId + '" class="job-listing">' +
                    '<div class="job-listing-details">' +
                    '<div class="job-listing-company-logo">' +
                    '</div>' +
                    '<div class="job-listing-description">' +
                    '<h4 class="job-listing-company">' + result[i].companyName + '<span class="verified-badge" title="Verified Employer" data-tippy-placement="top"></span></h4>' +
                    '<h3 class="job-listing-title">' + result[i].jobDesignation + '</h3>' +
                    '<p class="job-listing-text">' + result[i].description + '</p>' +
                    '</div>' +
                    '<span class="bookmark-icon"></span>' +
                    '</div>' +
                    '<div class="job-listing-footer">' +
                    '<ul>' +
                    //'<li><i class="icon-material-outline-location-on"></i> San Francissco</li>' +
                    '<li><i class="icon-material-outline-location-on"></i> ' + result[i].cityName + '</li>' +
                    '<li><i class="icon-material-outline-business-center"></i>' + result[i].workShift + '</li>' +
                    '<li><i class="icon-material-outline-account-balance-wallet"></i>' + result[i].ctc + '</li>' +
                    '<li><i class="icon-material-outline-access-time"></i>' + Math.ceil(Math.abs(new Date() - new Date(result[i].createdDate)) / (1000 * 60 * 60 * 24)) + ' days ago</li>' +
                    '</ul>' +
                    '</div>' +
                    '</a>';
            }
            $("#divJobList").html(html);
            paginationIsInProcess = false;
        },
        error: function (data) {
            paginationIsInProcess = false;
        }

    });
}

var valueSkip = 10;
var valueTake = 10;

    function IncreasedValue() {
        valueSkip += 10;
        return valueSkip;
    }
var paginationIsInProcess = false;
$(document).ready(function () {
    $('.simplebar-scroll-content').scroll(function () {

        let div = $(this).get(0);
        //console.log('scrollTop - ' + div.scrollTop)
        //console.log('clientHeight - ' + div.clientHeight)
        //console.log('scrollHeight - ' + div.scrollHeight)

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
    //filterData();
});

