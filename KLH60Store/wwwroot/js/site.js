// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
let $$ = (el) => document.getElementsByName(el);
let $$$ = (el) => document.querySelector(el);
$(() => {
    $$("Input.Role").forEach((e) => {
        e.addEventListener("click", (ev) => {
            let custForm = $$$("#customer-info");
            if (
                ev.target.value.localeCompare("customer", "en", {
                    sensitivity: "base",
                }) == 0
            ) {
                custForm.classList.remove("d-none");
            } else {
                custForm.classList.add("d-none");
            }
        });
    });
    //     let custButton = $$("Input.Role")[0];
    //     let custForm = $$$("#registerCust");
    //     custForm.addEventListener("submit", e => {
    //         e.stopPropagation();
    //         e.preventDefault();
    //         if (custButton.checked) {
    //             let valid = true;
    //             if ($("#Customer_FirstName").val() === '') {
    //                 console.log($("#Customer_FirstName").val());
    //                 $$$("#fnameVerify").innerHTML = "Your first name is required";
    //                 valid = false;
    //             }
    //             if ($("#Customer_LastName").val() === '') {
    //                 $$$("#lnameVerify").innerHTML = "Your last name is required";
    //                 valid = false;
    //             }
    //             if ($("#Customer_PhoneNumber").val() === '') {
    //                 $$$("#phoneVerify").innerHTML = "Your phone number is required";
    //                 valid = false;
    //             }
    //             if ($("#Customer_CreditCard").val() === '') {
    //                 $$$("#ccVerify").innerHTML = "Your credit card is required";
    //                 valid = false;
    //             }
    //             if (valid)
    //                 custForm.submit();
    //             else
    //                 return false;
    //         }
    //     });
});

//$("#registerCust").validate({
//    rules: {
//        "Customer.FirstName": "required",
//        "Customer.LastName": "required",
//        "Customer.PhoneNumber": "required",
//        "Customer.CreditCard": "required"
//    },
//    messages: {
//        "Customer.FirstName": "Please enter your First Name",
//        "Customer.LastName": "Please enter your Last Name",
//        "Customer.PhoneNumber": "Please enter your Phone Number",
//        "Customer.CreditCard": "Please enter your Credit Card"
//    }
//});