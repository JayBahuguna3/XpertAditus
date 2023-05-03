'use strict';

function PermissionChanged(result) {
    if (result.state == 'granted') {
        StartRecording();
    } else if (result.state == 'prompt') {
        alert("Please allow microphone permission.");
    } else if (result.state == 'denied') {
        alert("Microphone permission Denied or not found.");
    }
}
$(document).ready(function () {
    let start = document.getElementById('btnStart');
    let stop = document.getElementById('btnStop');
    let uploadButton = document.querySelector('button#upload');
    
    start.addEventListener('click', function (ev) {
        StartRecording();
        uploadButton.disabled = true;
    })
    stop.addEventListener('click', function (ev) {
        mediaRecorder.stop();
        uploadButton.disabled = false;

    });


    downloadButton.addEventListener('click', () => {
       // const blob = new Blob(recordedBlobs, { type: 'audio/mp3' });
       // const url = window.URL.createObjectURL(audioData);
        //var myfile = new File([audioData], 'Candidate_Audio');
       // var file = [myfile];
        let questionID = document.getElementById('candidate_questionId').textContent;
        var formData = new FormData();
        formData.append('fname', 'test.mp3');
        formData.append('data', audioData);
        formData.append('Question_Id', questionID);
        $.ajax({
            url: "/AudioAssessment/UploadAudio",
            type: "POST",
            dataType: "json",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                window.location.reload();
            },
            error: function (data) {
                window.location.reload();
            }
        })
       
    });

})
let start = document.getElementById('btnStart');
start.addEventListener('click', function (ev) {

    navigator.permissions.query({ name: 'microphone' }).then(function (result) {
        PermissionChanged(result);
        result.onchange = function (result) {
            PermissionChanged(result);
        };
    });

})
var mediaRecorder = {};
let audioData = "";
function StartRecording() {
    let audioIN = { audio: true };
    navigator.mediaDevices.getUserMedia(audioIN)
        .then(function (mediaStreamObj) {
            let audio = document.querySelector('audio');
            if ("srcObject" in audio) {
                audio.srcObject = mediaStreamObj;
            }
            else {
                audio.src = window.URL
                    .createObjectURL(mediaStreamObj);
            }
            audio.onloadedmetadata = function (ev) {
                audio.play();
            };
            
            let playAudio = document.getElementById('adioPlay');
            mediaRecorder = new MediaRecorder(mediaStreamObj);
            
            mediaRecorder.ondataavailable = function (ev) {
                dataArray.push(ev.data);
            }
            let dataArray = [];
            mediaRecorder.onstop = function (ev) {
                mediaStreamObj.getTracks().forEach(function (track) { track.stop() });

                audioData = new Blob(dataArray,
                    { 'type': 'audio/mp3;' });
                dataArray = [];
                let audioSrc = window.URL
                    .createObjectURL(audioData);
                playAudio.src = audioSrc;
            }
            mediaRecorder.start();
        })
        .catch(function (err) {
            console.log(err.name, err.message);
        });
}

