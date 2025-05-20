"use strict";

var chatId;
var userId = '';
var productId = 0;

const formatDate = (date) => {
    const dd = String(date.getDate()).padStart(2, '0');
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const yyyy = date.getFullYear();
    const hh = String(date.getHours()).padStart(2, '0');
    const min = String(date.getMinutes()).padStart(2, '0');
    const ss = String(date.getSeconds()).padStart(2, '0');

    return `${dd}.${mm}.${yyyy} ${hh}:${min}:${ss}`;
};

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
            sentAt: formatDate(new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()))
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