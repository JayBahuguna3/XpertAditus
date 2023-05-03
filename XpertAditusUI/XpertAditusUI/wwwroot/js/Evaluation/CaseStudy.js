
function SaveCaseStudyEvaluation() {
    var button = event.target;
    var score = $('#' + button.id + '_Score').val();
    if (!score) {
        alert('Please enter Score!');
        return;
    }
    var feedback = $('#' + button.id + '_Feedback').val();
    if (!feedback) {
        alert('Please enter Feedback!');
        return;
    }
    $.ajax({
        url: "/PACaseStudy/SaveScore/"+button.id+"/"+feedback+"/"+score,
        type: "GET",
        dataType: "json",
        contentType: false,
        processData: false,
        success: function (data) {
            window.location.href = "/PACaseStudy/PACaseStudySubmittedStudentList";
        },
        error: function (data) {
            alert(data.statusText);//window.location.reload();
        }
    })
}