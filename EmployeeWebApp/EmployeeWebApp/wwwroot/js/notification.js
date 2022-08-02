"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();
connection.on("sendToUser", (Title, Content) => {
    var heading = document.createElement("h3");
    heading.textContent = Title;
    var p = document.createElement("p");
    p.innerText = Content; var div = document.createElement("div");
    div.appendChild(heading);
    div.appendChild(p);

    document.getElementById("articleList").appendChild(div);
});
connection.start().catch(function (err) {
    return console.error(err.toString());
});

var badge = document.getElementById("badgecounter");
var count = parseInt(badge.innerText);
count++;
if (count < 100) {
    badge.textContent = count.toString();
} else if (count >= 100) {
    badge.textContent = "100+";
}
else {
    badge.textContent = "1";
}