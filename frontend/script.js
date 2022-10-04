'use strict';

var cookie = JSON.parse("{" + document.cookie + "}");
if ("user-name" in cookie) {
    
} else {
    document.getElementById("user-creation-modal").classList.remove("hidden");
    document.getElementById("user-name").focus();
}