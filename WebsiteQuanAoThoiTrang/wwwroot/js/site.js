// AJAX cho thêm/xóa giỏ hàng
document.addEventListener('DOMContentLoaded', function () {
    // Thêm vào giỏ
    const addToCartForms = document.querySelectorAll('form[action*="AddToCart"]');
    addToCartForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            handleCartForm(form);
        });
    });

    // Xóa khỏi giỏ
    const removeFromCartForms = document.querySelectorAll('form[action*="RemoveFromCart"]');
    removeFromCartForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            if (confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
                handleCartForm(form);
            }
        });
    });

    function handleCartForm(form) {
        const formData = new FormData(form);
        fetch(form.action, { method: 'POST', body: formData })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert(data.message);
                    if (data.totalItems !== undefined) {
                        const cartBadge = document.querySelector('#cartBadge');
                        if (cartBadge) cartBadge.textContent = data.totalItems;
                    }
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