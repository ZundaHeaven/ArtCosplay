function isEmptyOrSpaces(str){
    return str === null || str.match(/^ *$/) !== null;
}

var user;
fetch('/User/GetCurrentUser', {method : 'GET'})
    .then((response) => response.text())
    .then((text) => {
        user = JSON.parse(text);
    })
    .catch(error =>
        console.error(error)
    );

function createComment(data) {
  const commentHTML = `
    <div class="comment-item">
      <img src="${data.avatar}" alt="Аватар" class="comment-avatar">
      <div class="comment-content">
        <div class="comment-header">
          <span class="comment-user">${data.username}</span>
          <span class="comment-time">${data.time}</span>
          <i class="fa-solid fa-trash discussion-action" name="delete-comment" style="margin-left: auto;" comment-id="${data.commentId}"></i>
        </div>
        <p class="comment-text">${data.text}</p>
        <div class="comment-actions">
          <div name="like-comment" class="comment-action" comment-id="${data.commentId}">
            <i class="far fa-thumbs-up"></i>
            <span>${data.likes}</span>
          </div>
          <div class="comment-action">
            <i class="fas fa-reply"></i>
            <span>Ответить</span>
          </div>
        </div>
      </div>
    </div>
  `;

    const container = document.getElementById('comment-list');
    container.insertAdjacentHTML('afterbegin', commentHTML);

    const newComment = container.firstElementChild;

    const likeBtn = newComment.querySelector('[name="like-comment"]');
    likeBtn.addEventListener('click', likeComment);

    const deleteBtn = newComment.querySelector('[name="delete-comment"]');
    deleteBtn.addEventListener('click', deleteComment);
}

function addComment()
{
    if(user === undefined)
    {
        window.location.replace("/User/Registration");
        return;
    }

    contentBox = document.getElementById('comment-textarea');
    sendButton = document.getElementById('comment-send');
    validationError = document.getElementById('validation-error');
    commentHeader = document.getElementById('comments-title');
    commentHeaderPost = document.getElementById('post-comments-title');
    
    if(isEmptyOrSpaces(contentBox.value))
    {
        validationError.textContent = 'Заполните секцию комментария!';
        return;
    }

    sendButton.disabled = true;
    
    fetch('/Comment/Discussion', {
        method : 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: document.getElementById('discussion-id').value,
            Content : contentBox.value
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] != 'success') {
            validationError.textContent = json['message'];
        }
        else {
            

            var data = {
                avatar: user.avatarUrl,
                username: user.userName,
                time: new Date().toISOString().replace('T', ' ').slice(0, 19),
                text: contentBox.value,
                commentId: json['commentId'],
                likes: 0
            };

            createComment(data);
            commentHeader.innerText = `Комментарии (${document.getElementsByClassName('comment-item').length})`;
            commentHeaderPost.innerText = `${document.getElementsByClassName('comment-item').length} комментария`
            contentBox.value = null;

            validationError.textContent = '';
            
            noComments = document.getElementById('no-comments');
            if(noComments) noComments.remove();
        }
    })
    .catch(error => {
        console.log(error);
        validationError.textContent = 'Ошибка во время добавления комментария!';
  });
  sendButton.disabled = false;
}

function likeComment(event)
{
    if(user === undefined)
    {
        window.location.replace("/User/Registration");
        return;
    }

    commentId = event.currentTarget.getAttribute("comment-id");
    likeShow = event.currentTarget.children[1];
    event.currentTarget.classList.toggle('liked');

    fetch('/Comment/Like', {
        method : 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: parseInt(commentId)
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            event.currentTarget.classList.toggle("liked");
        }
        else
        {
            likeShow.textContent = json['likesCount'];
        }
    })
    .catch(error => {
        console.log(error);
        validationError.textContent = 'Ошибка во время добавления комментария!';
    });
}

function likeDiscussion(event)
{
    if(user === undefined)
    {
        window.location.replace("/User/Registration");
        return;
    }

    likeShow = event.currentTarget.children[1];
    event.currentTarget.classList.toggle("liked");
    fetch('/Discussion/Like', {
        method : 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: parseInt(document.getElementById('discussion-id').value)
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            event.currentTarget.classList.toggle("liked");
        }
        else
        {
            likeShow.textContent = json['likesCount'];
        }
    })
    .catch(error => {
        console.log(error);
    });
}

function deleteComment(event)
{
    if(user === undefined)
    {
        window.location.replace("/User/Registration");
        return;
    }

    commentId = event.currentTarget.getAttribute("comment-id");
    commentHeader = document.getElementById('comments-title');
    commentHeaderPost = document.getElementById('post-comments-title');
    container = document.getElementById('comment-list');

    fetch('/Comment/Delete', {
        method : 'DELETE',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: parseInt(commentId)
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            alert('Ошибка удаления!');
        }
        else
        {
            const ancestorToRemove = event.target.closest('.comment-item');
            if (ancestorToRemove) ancestorToRemove.remove();
            commentHeader.textContent = `Комментарии (${container.children.length})`;
            commentHeaderPost.textContent = `${container.children.length} комментарии`;
            
            if(container.children.length === 0)
            {
                element = document.createElement('span');
                element.textContent = 'Нет ещё комментариев!';
                element.setAttribute('id', 'no-comments');
                container.appendChild(element);
            }
        }
    })
    .catch(error => {
        console.log(error);
    });
}

function deletePost()
{
    if(user === undefined)
    {
        window.location.replace("/User/Registration");
        return;
    }

    fetch('/Discussion/Delete', {
        method : 'DELETE',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify({
            Id: parseInt(document.getElementById('discussion-id').value)
        })
    })
    .then(response => response.json())
    .then(json => {
        if (json['status'] !== 'success') {
            alert('Ошибка удаления!');
        }
        else
        {
            window.location.replace("/Home/DiscusPage");
        }
    })
    .catch(error => {
        console.log(error);
    });
}

var button = document.getElementById('comment-send');
button.onclick = addComment;
const commentsLike = Array.from(document.getElementsByName('like-comment'));
commentsLike.forEach(element => {
    element.addEventListener("click", likeComment);
});
var postLike = document.getElementById('discussion-action');
postLike.addEventListener("click", likeDiscussion);
var commentsDelete = Array.from(document.getElementsByName('delete-comment'));
commentsDelete.forEach(element => {
    element.addEventListener("click", deleteComment);
});
var postDelete = document.getElementById('delete-post');
if(postDelete) postDelete.onclick = deletePost;