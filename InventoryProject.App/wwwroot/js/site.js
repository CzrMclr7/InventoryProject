"use strict"
let baseUrl = $("#txtBaseUrl").val(); 
const defaultUserProfile = "/Files/Images/UserPics/default.png"

function readUrl(input, container) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            container.css('background-image', 'url(' + e.target.result + ')');
            container.hide();
            container.fadeIn(650);
        }
        reader.readAsDataURL(input.files[0]);
    }
}