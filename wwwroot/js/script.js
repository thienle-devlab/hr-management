// Sidebar toggle
document.getElementById("sidebarToggle").addEventListener("click", function () {
    const sidebar = document.getElementById("sidebar");
    const toggleIcon = document.getElementById("toggleIcon");

    // Toggle sidebar collapsed class
    sidebar.classList.toggle("collapsed");

    // Update icon
    if (sidebar.classList.contains("collapsed")) {
        toggleIcon.classList.remove("fa-bars");
        toggleIcon.classList.add("fa-ellipsis-v");
    } else {
        toggleIcon.classList.remove("fa-ellipsis-v");
        toggleIcon.classList.add("fa-bars");
    }
});

// Dropdown menu toggle
document.querySelectorAll(".dropdown-parent > a").forEach(function (parentLink) {
    parentLink.addEventListener("click", function (event) {
        event.preventDefault();
        const parentItem = this.parentElement;
        parentItem.classList.toggle("show");
    });
});

// Thêm hàng mới (Nhân sự hoặc Tài khoản)
document.getElementById("btnThem").addEventListener("click", function() {
    resetForm();
    formTitle.textContent = formId === "nhanSuForm" ? "Thêm Nhân Sự" : "Thêm Tài Khoản";
    editingRow = null;
    formModal.show();
});

// Lưu các thay đổi khi thêm hoặc chỉnh sửa
saveChanges.addEventListener("click", function() {
    const formData = getFormData();
    if (editingRow) {
        // Cập nhật hàng hiện tại
        updateRow(editingRow, formData);
        alert("Chỉnh sửa thành công!");
    } else {
        // Thêm hàng mới
        addRow(formData);
        alert(formId === "nhanSuForm" ? "Thêm nhân sự thành công!" : "Thêm tài khoản thành công!");
    }
    formModal.hide();
});

// Chỉnh sửa hàng
document.addEventListener("click", function(event) {
    if (event.target.classList.contains("btnEdit")) {
        editingRow = event.target.closest("tr");
        populateForm(editingRow);
        formTitle.textContent = formId === "nhanSuForm" ? "Chỉnh sửa Nhân Sự" : "Chỉnh sửa Tài Khoản";
        formModal.show();
    }
});

// Xóa hàng
document.addEventListener("click", function(event) {
    if (event.target.classList.contains("btnDelete")) {
        if (confirm("Bạn có chắc chắn muốn xóa không?")) {
            const row = event.target.closest("tr");
            row.remove();
            alert("Xóa thành công!");
        }
    }
});

// Lấy dữ liệu từ form
function getFormData() {
    if (formId === "nhanSuForm") {
        return {
            id: document.getElementById("id").value || new Date().getTime(),
            name: document.getElementById("name").value,
            birthdate: document.getElementById("birthdate").value,
            gender: document.getElementById("gender").value,
            address: document.getElementById("address").value,
            phone: document.getElementById("phone").value,
            email: document.getElementById("email").value,
            cccd: document.getElementById("cccd").value,
            position: document.getElementById("position").value,
        };
    } else if (formId === "taiKhoanForm") {
        return {
            accountName: document.getElementById("accountName").value,
            employeeId: document.getElementById("employeeId").value,
            status: document.getElementById("status").value
        };
    }
}

// Reset form
function resetForm() {
    document.getElementById(formId).reset();
    if (formId === "nhanSuForm") {
        document.getElementById("id").value = "";
    }
}

// Điền dữ liệu vào form khi chỉnh sửa
function populateForm(row) {
    if (formId === "nhanSuForm") {
        document.getElementById("id").value = row.cells[0].textContent;
        document.getElementById("name").value = row.cells[1].textContent;
        document.getElementById("birthdate").value = row.cells[2].textContent;
        document.getElementById("gender").value = row.cells[3].textContent;
        document.getElementById("address").value = row.cells[5].textContent;
        document.getElementById("phone").value = row.cells[6].textContent;
        document.getElementById("email").value = row.cells[7].textContent;
        document.getElementById("cccd").value = row.cells[8].textContent;
        document.getElementById("position").value = row.cells[9].textContent;
    } else if (formId === "taiKhoanForm") {
        document.getElementById("accountName").value = row.cells[0].textContent;
        document.getElementById("employeeId").value = row.cells[1].textContent;
        document.getElementById("status").value = row.cells[2].textContent;
    }
}

// Thêm hàng mới vào bảng
function addRow(data) {
    const newRow = tableBody.insertRow();
    if (formId === "nhanSuForm") {
        newRow.innerHTML = `
            <td>${data.id}</td>
            <td>${data.name}</td>
            <td>${data.birthdate}</td>
            <td>${data.gender}</td>
            <td><img src="../assets/avatar1.jpg" alt="Avatar" class="img-thumbnail" width="50"></td>
            <td>${data.address}</td>
            <td>${data.phone}</td>
            <td>${data.email}</td>
            <td>${data.cccd}</td>
            <td>${data.position}</td>
            <td>
                <button class="btn btn-warning btn-sm me-2 btnEdit">Chỉnh sửa</button>
                <button class="btn btn-danger btn-sm btnDelete">Xóa</button>
            </td>
        `;
    } else if (formId === "taiKhoanForm") {
        newRow.innerHTML = `
            <td>${data.accountName}</td>
            <td>${data.employeeId}</td>
            <td>${data.status}</td>
            <td>
                <button class="btn btn-warning btn-sm me-2 btnEdit">Chỉnh sửa</button>
                <button class="btn btn-danger btn-sm btnDelete">Xóa</button>
            </td>
        `;
    }
}

// Cập nhật hàng hiện tại
function updateRow(row, data) {
    if (formId === "nhanSuForm") {
        row.cells[1].textContent = data.name;
        row.cells[2].textContent = data.birthdate;
        row.cells[3].textContent = data.gender;
        row.cells[5].textContent = data.address;
        row.cells[6].textContent = data.phone;
        row.cells[7].textContent = data.email;
        row.cells[8].textContent = data.cccd;
        row.cells[9].textContent = data.position;
    } else if (formId === "taiKhoanForm") {
        row.cells[0].textContent = data.accountName;
        row.cells[1].textContent = data.employeeId;
        row.cells[2].textContent = data.status;
    }
}

// Export dữ liệu dưới dạng CSV (Ví dụ cho bảng nhân sự)
document.getElementById("exportCSV").addEventListener("click", function() {
    const rows = Array.from(document.querySelectorAll("table tbody tr"));
    const csvContent = "data:text/csv;charset=utf-8,"
        + rows.map(row => Array.from(row.cells).map(cell => cell.textContent).join(",")).join("\n");

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "employee_data.csv");
    document.body.appendChild(link);
    link.click();
});
