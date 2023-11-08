// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deletePost(id) {
    $.ajax({
        url: '/Posts/Delete/',
        type: 'POST',
        data: { id: id }
        }).then(function(data)
        {
            window.location = "/Posts";
        });
}

$(document).ready(function(){
    $('textarea').on('input', function () {
        this.style.height = 'auto';
        this.style.height = (this.scrollHeight) + 'px';
    });
});




function autoResizeTextarea() {
    var textareas = document.querySelectorAll('textarea');
    textareas.forEach(function(textarea) {
        // Adjust the height when the page loads
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';

        // Adjust the height when the user inputs text
        textarea.addEventListener('input', function() {
            this.style.height = 'auto';
            this.style.height = this.scrollHeight + 'px';
        });
    });
}

// Call the function when the document is ready
document.addEventListener('DOMContentLoaded', autoResizeTextarea);