$("#btnBir").click(function () {
    //alert("button"); // Remove this line if it worked
    $("#iki").hide();
    $("#uc").hide();
    $("#bir").show();

    //document.getElementById("btnBir").style.backgroundColor = "#d4ca68";
    //document.getElementById("btnIki").style.backgroundColor = "white";
    //document.getElementById("btnBir").style.color = "white";
    //document.getElementById("btnIki").style.color = "#aeaecb";

    document.getElementById("btnBir").style.backgroundColor = "#d4ca68";
    document.getElementById("btnUc").style.backgroundColor = "white";
    document.getElementById("btnIki").style.backgroundColor = "white";
    document.getElementById("btnBir").style.color = "white";
    document.getElementById("btnUc").style.color = "#aeaecb";
    document.getElementById("btnIki").style.color = "#aeaecb";
});

$("#btnIki").click(function () {
    //alert("button"); // Remove this line if it worked
    $("#bir").hide();
    $("#uc").hide();
    $("#iki").show();

    //document.getElementById("btnIki").style.backgroundColor = "#d4ca68";
    //document.getElementById("btnBir").style.backgroundColor = "white";
    //document.getElementById("btnIki").style.color = "white";
    //document.getElementById("btnBir").style.color = "#aeaecb";


    document.getElementById("btnIki").style.backgroundColor = "#d4ca68";
    document.getElementById("btnUc").style.backgroundColor = "white";
    document.getElementById("btnBir").style.backgroundColor = "white";
    document.getElementById("btnIki").style.color = "white";
    document.getElementById("btnUc").style.color = "#aeaecb";
    document.getElementById("btnBir").style.color = "#aeaecb";
});

$("#btnUc").click(function () {
    //alert("button"); // Remove this line if it worked
    $("#bir").hide();
    $("#iki").hide();
    $("#uc").show();
    document.getElementById("btnUc").style.backgroundColor = "#d4ca68";
    document.getElementById("btnIki").style.backgroundColor = "white";
    document.getElementById("btnBir").style.backgroundColor = "white";
    document.getElementById("btnUc").style.color = "white";
    document.getElementById("btnIki").style.color = "#aeaecb";
    document.getElementById("btnBir").style.color = "#aeaecb";
});


var elements = document.getElementsByClassName("chat_list")
jQuery(".chat_list").click(function () {
    var contentPanelId = jQuery(this).attr("id");
    for (var i = 0; i < elements.length; i++) {
        elements[i].classList.remove("active_chat");
    }
    $("#" + contentPanelId).toggleClass("active_chat");
});



var count = $('.time_date').length;
var count2 = $('.incoming_msg').length;
var count3 = $('.outgoing_msg').length;
var c = count2 + count3;

$("div").animate({ scrollTop: $('#og' + c).offset().top }, 1000);
$("div").animate({ scrollTop: $('#ic' + c).offset().top }, 1000);