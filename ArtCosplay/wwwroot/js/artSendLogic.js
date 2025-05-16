document.querySelector('.file-upload').addEventListener('click', function () {
    document.getElementById('art-file').click();
});

const input = document.getElementById('art-file');
const dropArea = document.querySelector('.file-upload');

input.addEventListener('change', function (e) {
    if (this.files.length) handleFile(this.files[0]);
});

dropArea.addEventListener('dragover', function (e) {
    e.preventDefault();
    this.classList.add('dragover');
});

dropArea.addEventListener('dragleave', function (e) {
    e.preventDefault();
    this.classList.remove('dragover');
});

dropArea.addEventListener('drop', function (e) {
    e.preventDefault();
    this.classList.remove('dragover');

    const files = e.dataTransfer.files;
    if (files.length && files[0].type.startsWith('image/')) {
        input.files = files;
        handleFile(files[0]);
    }
});

function handleFile(file) {
    console.log('Выбран файл:', file.name);

    const existingPreview = document.getElementById('preview-art');
    if (existingPreview) existingPreview.remove();


    const reader = new FileReader();
    reader.onload = function (e) {
        const img = document.createElement('img');
        img.src = e.target.result;
        img.style.maxWidth = '90%';
        img.style.borderRadius = '12px';
        img.setAttribute('id', 'preview-art');
        dropArea.appendChild(img);
    };
    reader.readAsDataURL(file);
}