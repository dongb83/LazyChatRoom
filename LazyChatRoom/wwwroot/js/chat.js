"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub?isChat=true").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
let userinfo = JSON.parse(sessionStorage["userinfo"])
let userinfoString = sessionStorage["userinfo"]

connection.on("ReceiveMessage", function (user, avatar, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var img = "<img style='width:30px;height:30px' src='" + avatar + "' />";
    var encodedMsg = img + user + " says " + msg;
    var div = document.createElement("div");
    div.innerHTML = encodedMsg;
    document.getElementById("messagesList").appendChild(div);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

async function start() {
    try {
        await connection.start();
        var encodedMsg = "<span style='color:#666'>重新连接</span>";
        var li = document.createElement("li");
        li.innerHTML = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.onclose(async () => {
    await start();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = userinfoString;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});