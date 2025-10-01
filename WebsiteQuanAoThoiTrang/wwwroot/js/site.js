// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Thêm vào cuối file
$(document).ready(function () {
    $('.add-to-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        $.post('/Cart/AddToCart', { productId: productId }, function (data) {
            if (data.success) {
                alert('Đã thêm vào giỏ!');
                // Update badge số lượng
            }
        });
    });
});

// AJAX cho nút "Thêm vào giỏ" và "Xóa khỏi giỏ" - Ngăn reload và show alert từ JSON
document.addEventListener('DOMContentLoaded', function () {
    // Xử lý form AddToCart
    const addToCartForms = document.querySelectorAll('form[action*="AddToCart"]');
    addToCartForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            handleCartForm(form);
        });
    });

    // Xử lý form RemoveFromCart (THÊM MỚI)
    const removeFromCartForms = document.querySelectorAll('form[action*="RemoveFromCart"]');
    removeFromCartForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            if (confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
                handleCartForm(form);
            }
        });
    });

    // Hàm chung xử lý form Cart (Add hoặc Remove)
    function handleCartForm(form) {
        const formData = new FormData(form);

        fetch(form.action, {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert(data.message);  // Show "Đã thêm/xóa khỏi giỏ!"
                    // Cập nhật badge tổng số item
                    if (data.totalItems !== undefined) {
                        const cartBadge = document.querySelector('#cartBadge');
                        if (cartBadge) cartBadge.textContent = data.totalItems;
                    }
                    // Nếu đang ở trang Cart, reload nhẹ để cập nhật table
                    if (window.location.pathname.includes('/Cart')) {
                        location.reload();
                    }
                } else {
                    alert(data.message || 'Lỗi khi xử lý giỏ hàng.');
                }
            })
            .catch(error => {
                console.error('Lỗi AJAX:', error);
                alert('Có lỗi xảy ra. Vui lòng thử lại.');
            });
    }
});