function UpdateTrainingStatusComplete(trainingid, courseid) {
    var request = $.ajax({
        url: "/CandidateTrainingContent/MarkCandidateTrainingComplete/" + courseid +'/'+trainingid,
        method: "GET",
    });

    request.done(function (response) {
        if (response.error) {
            alert(response.message);
        }
        else {
            alert(response.message);
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Unable to mark complete, please connect with Admin. " + textStatus);
    });
}