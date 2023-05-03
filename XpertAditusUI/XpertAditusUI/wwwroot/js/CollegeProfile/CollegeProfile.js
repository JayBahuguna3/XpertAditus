ClassicEditor
    .create(document.querySelector('#About'))
    .catch(error => {
        console.error(error);
    })

ClassicEditor
    .create(document.querySelector('#Reviews'))
    .catch(error => {
        console.error(error);
    })

function getsDistricts() {
    var district = "<option value="+''+"> Select District </option>";
    var stateId = $("#StateId option:selected").val();
    if (stateId != 0) {
        var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetDistricts";
        $.ajax({
            url: rootPath,
            type: 'POST',
            data: {
                Id: stateId.toString()
            },
            success: function (result) {
                for (i = 0; i <= result.length - 1; i++) {
                    district += "<option value=" + result[i].districtId + ">" + result[i].name + "</option>"
                }
                $("#DistrictId").empty();
                $("#DistrictId").append(district);
            }
        });
    }
}

function getCities() {
    var city = "<option value="+''+"> Select City </option>";
    var districtId = $("#DistrictId option:selected").val();
    if (districtId != 0) {
        var rootPath = window.location.protocol + "//" + window.location.host + "/GetAddressInfo/GetCities";
        $.ajax({
            url: rootPath,
            type: 'POST',
            data: {
                Id: districtId.toString()
            },
            success: function (result) {
                for (i = 0; i <= result.length - 1; i++) {
                    city += "<option value=" + result[i].cityId + ">" + result[i].name + "</option>"
                }
                $("#CityId").empty();
                $("#CityId").append(city);
            }
        });
    }
}
