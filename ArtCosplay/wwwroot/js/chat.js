"use strict";

var chatId;
var userId = '';
var productId = 0;

function addMessage(data) {
    console.log(data);
    var commentHTML =
    `<div class="message ${data.type}">
        <div class="message-bubble">
            ${data.content}
            <div class="message-time">${data.sentAt}</div>
        </div>
     </div>`

    console.log(commentHTML);

    const container = document.getElementById('messages-container');
    console.log(container);

    container.insertAdjacentHTML('beforeend', commentHTML);

    console.log(container);

}

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/Chat")
    .build();

connection.on("ReceiveMessage", function (message, user, chat) {
    if (chat === chatId || (user === userId && chatId == undefined)) {
        chatId = chat;
        let date = new Date();
        var data = {
            type: userId === user ? "outgoing" : "incoming",
            content: message,
            sentAt: date.toISOString().split('T')[0]
        };
        addMessage(data);
    }
});

connection.start()
    .then(() => console.log('Connected to SignalR hub'))
    .catch(err => console.error(err.toString()));

document.getElementById('send-button').addEventListener('click', (e) => {
    
    const message = document.getElementById('message-area').value;
    document.getElementById('message-area').value = '';

    if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke('SendMessage', message, productId)
            .catch(err => console.error('Send failed:', err.toString()));
    } else {
        console.error('Cannot send message - connection not established');
    }

    e.preventDefault();
});