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