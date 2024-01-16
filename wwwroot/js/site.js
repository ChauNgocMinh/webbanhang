// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getShoppingCart() {
    let cartId = localStorage.getItem("Cart Id");
    if (cartId) {
        window.location.href = `/Cart?Id=${cartId}`;
    } else {
        window.location.href = "/Cart";
    }
}
