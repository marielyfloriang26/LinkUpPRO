 document.addEventListener("DOMContentLoaded", function() {
     if (window.location.hash) {
         const el = document.querySelector(window.location.hash);
         if (el) {
             if (el.classList.contains('lup-comments')) {
                 el.classList.add('is-open');
             }
             el.scrollIntoView({ behavior: 'smooth', block: 'center' });
         }
     }
 });

    function openEditComment(commentId) {
        document.getElementById('commentText-' + commentId).classList.add('d-none');
        document.getElementById('editCommentForm-' + commentId).classList.remove('d-none');
    }

    function closeEditComment(commentId) {
        document.getElementById('commentText-' + commentId).classList.remove('d-none');
        document.getElementById('editCommentForm-' + commentId).classList.add('d-none');
    }
        function openDeleteComment(commentId) {
        document.getElementById('commentText-' + commentId).classList.add('d-none');
        document.getElementById('commentActions-' + commentId).classList.add('d-none');
        document.getElementById('deleteConfirm-' + commentId).classList.remove('d-none');
    }

    function closeDeleteComment(commentId) {
        document.getElementById('commentText-' + commentId).classList.remove('d-none');
        document.getElementById('commentActions-' + commentId).classList.remove('d-none');
        document.getElementById('deleteConfirm-' + commentId).classList.add('d-none');
    }
        function toggleReplyForm(commentId) {
        const form = document.getElementById('replyForm-' + commentId);
        if (form) {
            form.classList.toggle('d-none');
        }
    }
    function toggleComments(postId) {
        const el = document.getElementById('comments-' + postId);
        if (el) el.classList.toggle('is-open');
    }


        document.querySelectorAll('.lup-comment-input').forEach(form => {
        form.addEventListener('submit', async function(e) {
            e.preventDefault(); 
            
            const formData = new FormData(this);
            
            try {
                const response = await fetch(this.action, {
                    method: 'POST',
                    body: formData
                });
                
                if (response.ok) {
                    // Obtiene el ID de la publicacion y le añade el Hash antes de recargar
                    const postId = this.querySelector('input[name="PublicacionId"]').value;
                    window.location.hash = 'comments-' + postId;
                    window.location.reload(); 
                } else {
                    alert("No se pudo guardar el comentario. Verifica las validaciones.");
                }
            } catch (error) {
                console.error("Error:", error);
            }
        });
    });

    