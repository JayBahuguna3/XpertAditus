
function SaveCaseStudyEvaluation() {
    var button = event.target;
    var score = $('#'+button.id + '_Score').val();
    var feedback = $('#' +button.id + '_Feedback').val();
    $.ajax({
        url: "/PAInterview/SaveScore/"+button.id+"/"+feedback+"/"+score,
        type: "GET",
        dataType: "json",
        contentType: false,
        processData: false,
        success: function (data) {
            debugger;//
            window.location.reload();
        },
        error: function (data) {
            alert(data.statusText);//window.location.reload();
        }
    })
}