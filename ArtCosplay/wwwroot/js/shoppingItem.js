var returnUrl = '/Home/ShopPage';
function deleteProduct(e) {
    fetch('/Product/Put', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: parseInt(e.target.getAttribute('item-id'))
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            alert('Ошибка удаления!');
        }
        else {
            window.location.replace(returnUrl);
        }
    })
    .catch(error => {
        console.log(error);
    });
}

var productDelete = document.getElementById('delete-item');

productDelete.addEventListener("click", deleteProduct);