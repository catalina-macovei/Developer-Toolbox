// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Funcție pentru a da like la întrebare
function likeQuestion(questionId) {
    fetch('/Questions/LikeQuestion/' + questionId, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                // Dezactivează butonul de like
                document.getElementById('likeButton').disabled = true;
                // Dacă utilizatorul a dat deja dislike, dezactivează butonul de dislike
                if (!document.getElementById('dislikeButton').disabled) {
                    document.getElementById('dislikeButton').disabled = true;
                }
            }
        });
}

// Funcție pentru a da dislike la întrebare
function dislikeQuestion(questionId) {
    fetch('/Questions/DislikeQuestion/' + questionId, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                // Dezactivează butonul de dislike
                document.getElementById('dislikeButton').disabled = true;
                // Dacă utilizatorul a dat deja like, dezactivează butonul de like
                if (!document.getElementById('likeButton').disabled) {
                    document.getElementById('likeButton').disabled = true;
                }
            }
        });
}
