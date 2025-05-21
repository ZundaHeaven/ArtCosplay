function deleteUser(e) {
    var row = e.currentTarget.parentElement.parentElement;
    var requestUrl = e.currentTarget.getAttribute("request-url");
    var varType = e.currentTarget.getAttribute('var-type');
    var Id = e.currentTarget.getAttribute("entity-id");

    fetch(requestUrl, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: varType === 'int' ? parseInt(Id) : Id
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            alert(json['message']);
        }
        else {
            row.remove();
        }
    })
    .catch(error => {
        console.log(error);
        alert('Ошибка во время удаления пользователя!');
    });
}

var deleteButton = Array.from(document.getElementsByName('delete-btn'));
deleteButton.forEach(element => {
    element.addEventListener("click", deleteUser);
});