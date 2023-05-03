PaymentInfo = null;
//Step 1 - Validation
function StartPaymentProcess() {

    var request = $.ajax({
        url: "/Payment/ValidateCourseAvailability?courseid=" + $("#course_id").val(),
        method: "GET",
    });

    request.done(function (response) {
        if (response.error) {
            alert(response.message);
        }
        else {
            checkCompleteTrainingAndTest();
        }
    
        //OpenPaymentGateway();
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Validation failed: " + textStatus);
    });


}

function checkCompleteTrainingAndTest() {
    var request = $.ajax({
        url: "/Payment/checkCompleteTrainingAndTest",
        method: "GET",
    });

    request.done(function (response) {
        if (response.error) {
            alert(response.message);
        }
        else {
            GetPaymentInfo();
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Validation failed: " + textStatus);
    });
}

//Step 2
function GetPaymentInfo() {

    var request = $.ajax({
        url: "/Payment/GetPaymentInfo",
        method: "GET",
    });

    request.done(function (msg) {
        PaymentInfo = msg;
        OpenPaymentGateway();
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Request failed: " + textStatus);
    });

}
//Step 3
function OpenPaymentGateway() {
    var ammt = $("#course_fee").val();
    var course_id = $("#course_id").val();

    var calAmmt = ammt * 100;
    var options = {
        "key": PaymentInfo.key, // Enter the Key ID generated from the Dashboard
        "amount": calAmmt, // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise or INR 500.
        "currency": PaymentInfo.currency,
        "name": PaymentInfo.name,
        "description": PaymentInfo.description,
        "image": PaymentInfo.image,
        //"order_id": "order_9A33XWu170gUtm",//This is a sample Order ID. Create an Order using Orders API. (https://razorpay.com/docs/payment-gateway/orders/integration/#step-1-create-an-order). Refer the Checkout form table given below
        "handler": function (response) {
            SavePaymentInfo(response);
        },
        "prefill": {
            "name": PaymentInfo.prefill_name,
            "email": PaymentInfo.prefill_email,
            "contact": PaymentInfo.prefill_contact
        },
        "notes": {
            "address": PaymentInfo.notes_add
        },
        "theme": {
            "color": PaymentInfo.theme_color
        }
    };
    var rzp1 = new Razorpay(options);
    rzp1.open();
}
//Step 4
function SavePaymentInfo(response) {
    var request = $.ajax({
        url: "/Payment/SavePaymentDetails?amount=" + $("#course_fee").val() + "&transactionId=" + response.razorpay_payment_id + "&courseid=" + $("#course_id").val(),
        method: "POST"
    });

    request.done(function (msg) {
        //alert(msg);
        if (msg.error) {
            alert(msg.message);
        }
        else {
            alert(msg.message);
            DownloadPaymentInvoice(msg.id);
        }
            
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Request failed: " + textStatus);
    });
}

//Step 5 download receipt
function DownloadPaymentInvoice(paymentId) {
    showSpinner();
    $.ajax({
        url: '/Payment/DownloadPaymentInvoice?paymentid=' + paymentId,
        type: 'Get',
        xhrFields:
        {
            responseType: 'blob'
        },
        
    })
        .done(function (result) {
            hideSpinner();
            var blob = new Blob([result], { type: "application/octet-stream" });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Invoice.pdf";
            link.click();

            //redirect to home page
            window.location.href = "/";
        });
}
