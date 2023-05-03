
function Send_JobApprovalMail(UserProfileId, JobId) {
    showSpinner();
    var userProfileId = UserProfileId;
    var jobId = JobId;
    var rootPath = window.location.origin + "/JobMasters/Send_JobApprovalMail?userProfileId=" + userProfileId + "&jobId=" + jobId;
    var request = $.ajax({
        url: rootPath,
        type: 'Post',
    });

    request.done(function (msg) {
        hideSpinner();
        if (msg.error) {
            alert(msg.message);
        }
        else {
            alert(msg.message);
            window.location.reload();
        }

    });
    request.fail(function (jqXHR, textStatus) {
        hideSpinner();
        alert("Request failed: " + textStatus);
    });

}
