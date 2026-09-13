// Inicialização do editor WYSIWYG (Quill) usado nas telas de
// criação e edição de notícias/artigos do Admin.
(function () {
    const editorElement = document.getElementById('quill-editor');
    if (!editorElement) {
        return;
    }

    const quill = new Quill('#quill-editor', {
        theme: 'snow',
        placeholder: 'Escreva o conteúdo da notícia...'
    });

    const conteudoTextarea = document.getElementById('editor-conteudo');
    if (conteudoTextarea) {
        quill.root.innerHTML = conteudoTextarea.value;
    }

    const form = document.getElementById('form-post');
    if (form && conteudoTextarea) {
        form.addEventListener('submit', function () {
            conteudoTextarea.value = quill.root.innerHTML;
        });
    }
})();
