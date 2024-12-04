document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const inputs = document.querySelectorAll('.form-control');
    const submitButton = document.querySelector('button[type="submit"]');

    // Thêm loading spinner
    const loadingDiv = document.createElement('div');
    loadingDiv.className = 'loading';
    loadingDiv.innerHTML = '<div class="loading-spinner"></div>';
    form.appendChild(loadingDiv);

    // Hiệu ứng focus cho input fields
    inputs.forEach(input => {
        // Thêm hiệu ứng khi focus vào input
        input.addEventListener('focus', function () {
            this.parentElement.querySelector('label').style.color = '#2980b9';
        });

        // Trả về màu ban đầu khi blur
        input.addEventListener('blur', function () {
            this.parentElement.querySelector('label').style.color = '#2c3e50';
        });

        // Kiểm tra giá trị input khi nhập
        input.addEventListener('input', function () {
            if (this.value.trim() !== '') {
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
            }
        });
    });

    // Xử lý form submit
    form.addEventListener('submit', function (e) {
        let isValid = true;

        // Kiểm tra các trường input
        inputs.forEach(input => {
            if (!input.value.trim()) {
                isValid = false;
                input.classList.add('is-invalid');
                showError(input, 'Trường này không được để trống');
            } else {
                input.classList.remove('is-invalid');
            }
        });

        if (!isValid) {
            e.preventDefault();
            return;
        }

        // Hiển thị loading
        submitButton.disabled = true;
        loadingDiv.style.display = 'block';

        // Thêm hiệu ứng cho nút submit
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm mr-2"></span>Đang xử lý...';
    });

    // Hàm hiển thị lỗi
    function showError(input, message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;

        // Xóa thông báo lỗi cũ nếu có
        const existingError = input.parentElement.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }

        input.parentElement.appendChild(errorDiv);
    }

    // Xử lý hiển thị/ẩn mật khẩu
    const passwordInput = document.getElementById('password');
    const togglePassword = document.createElement('button');
    togglePassword.type = 'button';
    togglePassword.className = 'btn btn-link password-toggle';
    togglePassword.innerHTML = '<i class="fa fa-eye"></i>';
    togglePassword.style.position = 'absolute';
    togglePassword.style.right = '10px';
    togglePassword.style.top = '38px';
    togglePassword.style.border = 'none';

    passwordInput.parentElement.style.position = 'relative';
    passwordInput.parentElement.appendChild(togglePassword);

    togglePassword.addEventListener('click', function () {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);
        this.innerHTML = type === 'password' ? '<i class="fa fa-eye"></i>' : '<i class="fa fa-eye-slash"></i>';
    });
});