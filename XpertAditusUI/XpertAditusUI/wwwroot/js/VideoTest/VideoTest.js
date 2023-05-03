
getPAQuestionnaire();

var remSeconds = 0;
var secondCounter = 0;
window.questionsList = [];
window.activeQuestion = null;
window.testInfo = null;
window.currentQuestionNo = 1;
window.timestart = true;
function formatNumber(number) {
    if (number < 10) return '0' + number;
    else return '' + number;
}

function startTick() {

    document.getElementById("secRemining").innerHTML = formatNumber(secondCounter);
    document.getElementById("minRemining").innerHTML = formatNumber(parseInt(remSeconds % 3600 / 60));
    document.getElementById("hrsRemining").innerHTML = formatNumber(parseInt(remSeconds / 3600));

    var _tick = setInterval(function () {

        if (remSeconds > 0) {
            remSeconds = remSeconds - 1;
            secondCounter = secondCounter - 1;

            document.getElementById("secRemining").innerHTML = formatNumber(secondCounter);
            document.getElementById("minRemining").innerHTML = formatNumber(parseInt(remSeconds % 3600 / 60));
            document.getElementById("hrsRemining").innerHTML = formatNumber(parseInt(remSeconds / 3600));

            if (secondCounter == 0) {
                secondCounter = 60;
            }
        }
        else {
            alert("Your exam time expired");
            SubmitTest()
        }
    }, 1000)

    var _tick1 = setInterval(function () {
        setRemainingTime();
    }, 5000)

}

function setRemainingTime() {
    $.ajax({
        url: window.location.origin + '/PATest/UpdateRemainingTime?candidateResultId=' + window.testInfo.model.pacandidateResultId,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            if (result.id == "101") {
                alert("Your exam time expired");
                SubmitTest()
            }
        }
    });
}

function displayDiv(TrainingContentId) {
    window.questionsList = [];
    window.activeQuestion = null;
    window.currentQuestionNo = 1;

    if ($('#btnNext').is(':visible') == false) {
        $("#btnNext").show();
    }
    getPAQuestionnaire(TrainingContentId);
    $("[id ^= 'btnTrainingContentName_']").removeClass("active");
    $("#btnTrainingContentName_" + TrainingContentId).addClass("active");
}

function getPAQuestionnaire(TrainingContentId) {
    if (TrainingContentId == undefined) {
        TrainingContentId = $("#txtDefaultTrainingContentId").val();
    }
    window.TrainingContentId = TrainingContentId;
    showSpinner();
    $('#btnPrevious').hide();
    var QuestionnaireHTML = "";
    var url = window.location.origin + '/PAInterview/GetActiveTestInfo?CourseId=' + window.CourseId + '&TrainingContentId=' + TrainingContentId + '&monthytestid=' + window.MonthlyTestId.toString();
    $.ajax({
        url: url,
        type: 'GET',
        success: function (result) {
            hideSpinner();
            if (result && !result.error) {
                window.testInfo = result;
                for (i = 0; i <= result.maxCount - 1; i++) {
                    var buttonId = i + 1;
                    QuestionnaireHTML += '<button class="btn btn-outline-primary" id="btn_' + buttonId + '" onclick= "getPAQuestion(' + buttonId + ', undefined , true)">' + buttonId + '</button>'
                }
                $("#divQuestionnaire").html(QuestionnaireHTML);
                for (j = 0; j < result.checkAnswerProvided.length; j++) {
                    if (result.checkAnswerProvided[j].answerProvided) {
                        $("#btn_" + result.checkAnswerProvided[j].questionOrder).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
                    }
                    else {
                        $("#btn_" + result.checkAnswerProvided[j].questionOrder).addClass("btn-danger").removeClass("btn-outline-primary");
                    }
                }

                var reMainingtime = new Date(result.model.remainingTime);
                var testEnd = new Date(result.model.testEnd);
                var diffSecond = Math.round((testEnd.getTime() - reMainingtime.getTime()) / 1000);
                remSeconds = diffSecond;
                secondCounter = remSeconds % 60
                if (secondCounter == 0) {
                    secondCounter = 59;
                }

                if (window.timestart) {
                    window.timestart = false;
                    startTick();
                    $("#btnTrainingContentName_" + TrainingContentId).addClass("active");
                }

                getPAListOfActiveTestQuestions(TrainingContentId);
                AddBorder();
            }
            else {

                alert(result.message);
                if (result.id == 1) {
                    window.location.href = "/SelectCourse/SelectCourse";
                }
            }

        }
    });
}

function getPAListOfActiveTestQuestions(TrainingContentId) {
    showSpinner();
    $.ajax({
        url: window.location.origin + '/PATest/getPAListOfActiveTestQuestions?candidateResultId=' + window.testInfo.model.pacandidateResultId + '&TrainingContentId=' + TrainingContentId,
        type: 'GET',
        success: function (result) {
            hideSpinner();
            if (result && !result.error && result.length > 0) {
                window.questionsList = result;
                window.activeQuestion = result[0];
                renderQuestion(result[0]);
            }
            else {
                getPAQuestion(-1, TrainingContentId);
            }
        }
    });
}

function SetColorButton() {
    if ($('input[name=answer]:checked').val() != undefined) {
        window.activeQuestion.optionSelected = parseInt($('input[name=answer]:checked').val());
        window.activeQuestion.answerProvided = "True";
        $("#btn_" + window.currentQuestionNo).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
        SavePAQuestionAnswer(window.activeQuestion.questionnaireResultId, window.activeQuestion.optionSelected);
    }
    else {
        $("#btn_" + window.currentQuestionNo).addClass("btn-danger").removeClass("btn-outline-primary");
    }
}


function getPAQuestion(questionOrder = -1, TrainingContentId, IsDriectQuestion) {
    if (TrainingContentId == undefined) {
        TrainingContentId = window.TrainingContentId;
    }

    if (questionOrder != -1) { // Driect Question No Condition
        var checkQuestionOrderPresent = false;

        for (i = 0; i < window.questionsList.length; i++) {
            if (window.questionsList[i].questionOrder == questionOrder) {
                if (IsDriectQuestion) {
                    SetColorButton();
                }
                window.activeQuestion = window.questionsList[i];
                checkQuestionOrderPresent = true
                break;
            }
        }

        if (checkQuestionOrderPresent) {
            renderQuestion(window.activeQuestion);
            AddBorder(questionOrder);
            HideShowPreviousNextButton(questionOrder);
            window.currentQuestionNo = questionOrder;
        }
        else {
            showSpinner();

            if (IsDriectQuestion) {
                SetColorButton();
            }


            $.ajax({
                url: window.location.origin + "/PATest/GetPAActiveTestQuestion?questionOrder=-1&candidateResultId=" + window.testInfo.model.pacandidateResultId + "&TrainingContentId=" + TrainingContentId + "&driectQuestionNo=" + questionOrder,
                type: 'GET',
                success: function (result) {
                    hideSpinner();
                    if (result && !result.error) {
                        window.questionsList.push(result);
                        window.activeQuestion = result;
                        renderQuestion(result);
                        AddBorder(questionOrder);
                        HideShowPreviousNextButton(questionOrder);
                        window.currentQuestionNo = questionOrder;
                    }
                    else {
                        if (result.id == "101") {
                            getPAQuestion(window.currentQuestionNo, TrainingContentId);
                        }
                        else {
                            alert(result.message);
                        }
                    }

                }
            });
        }
    }
    else {

        var checkQuestionOrderPresent = false;

        for (i = 0; i < window.questionsList.length; i++) {
            if (window.questionsList[i].questionOrder == questionOrder) {
                window.activeQuestion = window.questionsList[i];
                checkQuestionOrderPresent = true
                break;
            }
        }

        if (checkQuestionOrderPresent) {
            renderQuestion(window.activeQuestion);
        }
        else {
            showSpinner();
            $.ajax({
                url: window.location.origin + '/PATest/GetPAActiveTestQuestion?questionOrder=' + questionOrder + "&candidateResultId=" + window.testInfo.model.pacandidateResultId + "&TrainingContentId=" + TrainingContentId,
                type: 'GET',
                success: function (result) {
                    hideSpinner();
                    if (result && !result.error) {
                        window.questionsList.push(result);
                        window.activeQuestion = result;
                        renderQuestion(result);
                    }
                    else {
                        if (result.id == "101") {
                            getPAQuestion(window.currentQuestionNo, TrainingContentId);
                        }
                        else {
                            alert(result.message);
                        }
                    }

                }
            });
        }
    }

}

function renderQuestion(question) {
    //$("#txtQuestionNo").val(question.questionOrder);
    var QuestionsHTML = "";
    QuestionsHTML = '<div>"' + question.questionText + '"</div>' +
        '<div>' +
        '<div class="row">' +
        '<input type="radio" id="Option1" name="answer" value="1" style = "height:25px;">' +
        '<label style="padding-left: 10px;">' + question.option1 + '</label>' +
        '</div>' +
        '<div class="row">' +
        '<input type="radio" id="Option2" name="answer" value="2" style = "height:25px;">' +
        '<label style="padding-left: 10px;">' + question.option2 + '</label>' +
        '</div>' +
        '<div class="row">' +
        '<input type="radio" id="Option3" name="answer" value="3" style = "height:25px;">' +
        '<label style="padding-left: 10px;">' + question.option3 + '</label>' +
        '</div>' +
        '<div class="row">' +
        '<input type="radio" id="Option4" name="answer" value="4" style = "height:25px;">' +
        '<label style="padding-left: 10px;">' + question.option4 + '</label>' +
        '</div>' +
        '</div>';
    $("#divQuestions").html(QuestionsHTML);
    if (question.answerProvided) {
        $("#Option" + question.optionSelected).closest('span').addClass('checked');
        $("#Option" + question.optionSelected).attr("checked", true);
    }
}

function prevQuestion() {
    if (window.currentQuestionNo > 1) {
        if ($('input[name=answer]:checked').val() != undefined) {
            window.activeQuestion.optionSelected = parseInt($('input[name=answer]:checked').val());
            window.activeQuestion.answerProvided = "True";
            $("#btn_" + window.currentQuestionNo).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
            SavePAQuestionAnswer(window.activeQuestion.questionnaireResultId, window.activeQuestion.optionSelected);
        }
        else {
            $("#btn_" + window.currentQuestionNo).addClass("btn-danger").removeClass("btn-outline-primary");
        }
        window.currentQuestionNo = window.currentQuestionNo - 1;
    }
    else {
        if ($('input[name=answer]:checked').val() != undefined) {
            window.activeQuestion.optionSelected = parseInt($('input[name=answer]:checked').val());
            window.activeQuestion.answerProvided = "True";
            $("#btn_" + window.currentQuestionNo).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
            SavePAQuestionAnswer(window.activeQuestion.questionnaireResultId, window.activeQuestion.optionSelected);
        }
        else {
            $("#btn_" + window.currentQuestionNo).addClass("btn-danger").removeClass("btn-outline-primary");
        }
    }

    HideShowPreviousNextButton(window.currentQuestionNo);
    getPAQuestion(window.currentQuestionNo);
    AddBorder();
}

function nextQuestion() {
    if (window.currentQuestionNo < window.testInfo.maxCount) {

        if ($('input[name=answer]:checked').val() != undefined) {
            window.activeQuestion.optionSelected = parseInt($('input[name=answer]:checked').val());
            window.activeQuestion.answerProvided = "True";
            $("#btn_" + window.currentQuestionNo).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
            SavePAQuestionAnswer(window.activeQuestion.questionnaireResultId, window.activeQuestion.optionSelected);
        }
        else {
            $("#btn_" + window.currentQuestionNo).addClass("btn-danger").removeClass("btn-outline-primary");
        }
        window.currentQuestionNo = window.currentQuestionNo + 1;
    }
    else {
        if ($('input[name=answer]:checked').val() != undefined) {
            window.activeQuestion.optionSelected = parseInt($('input[name=answer]:checked').val());
            window.activeQuestion.answerProvided = "True";
            $("#btn_" + window.currentQuestionNo).addClass("btn-success").removeClass("btn-outline-primary").removeClass("btn-danger");
            SavePAQuestionAnswer(window.activeQuestion.questionnaireResultId, window.activeQuestion.optionSelected);
        }
        else {
            $("#btn_" + window.currentQuestionNo).addClass("btn-danger").removeClass("btn-outline-primary");
        }
    }

    HideShowPreviousNextButton(window.currentQuestionNo);
    getPAQuestion(window.currentQuestionNo);
    AddBorder();
}

function Submit() {
    if (confirm("Are you sure, want to finish the test, once submit the test score card  will be generate based on the test.")) {
        SubmitTest();
        //CandidateResult();
        //window.location.href = "/";
    } else {
    }
}

function SavePAQuestionAnswer(questionaireResultId, selectedOption) {
    var selectedOption = selectedOption.toString();
    var rootPath = window.location.protocol + "//" + window.location.host + "/PATest/UpdatePAQuestionaireResult?questionaireResultId=" + questionaireResultId + "&selectedOption=" + selectedOption;
    $.ajax({
        url: rootPath,
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {

        }
    });
}

function SubmitTest() {
    candidateResultId = window.questionsList[0].candidateResultId;
    var rootPath = window.location.protocol + "//" + window.location.host + "/PATest/CompletePATest?candidateResultId=" + window.testInfo.model.pacandidateResultId;
    $.ajax({
        url: rootPath,
        type: 'Get',
        async: false,
        success: function (result) {
            alert(result.message);
            window.location.href = "/";
        }
    });
}


function AddBorder(btnId) {
    $("[id ^= 'btn_']").css("border-color", "");
    if (btnId == undefined) {
        $('#btn_' + window.currentQuestionNo).css("border-color", "black");
    }
    else {
        $('#btn_' + btnId).css("border-color", "black");
    }
}

function HideShowPreviousNextButton(questionOrder) {
    if (questionOrder == 1) {
        $('#btnPrevious').hide();
        if ($('#btnNext').is(':visible') == false) {
            $('#btnNext').show();
        }
    }
    else if (questionOrder == window.testInfo.maxCount) {
        $('#btnNext').hide();
        if ($('#btnPrevious').is(':visible') == false) {
            $('#btnPrevious').show();
        }
    }
    else {
        $('#btnPrevious').show();
        $('#btnNext').show();
    }
}

