// Hiển thị hoặc ẩn form
function toggleForm(formId) {
  const form = document.getElementById(formId);
  form.style.display = form.style.display === "block" ? "none" : "block";
}

// Hiển thị modal thêm hoặc chỉnh sửa
function showModal(modalId, isEdit = false, data = {}) {
  const modal = document.getElementById(modalId);
  modal.style.display = "block";

  if (isEdit) {
      for (const key in data) {
          const input = modal.querySelector(`#${key}`);
          if (input) input.value = data[key];
      }
      modal.querySelector("h4").textContent = "Chỉnh sửa";
  } else {
      modal.querySelector("form").reset();
      modal.querySelector("h4").textContent = "Thêm mới";
  }
}

// Đóng modal
function closeModal(modalId) {
  const modal = document.getElementById(modalId);
  modal.style.display = "none";
}

// Xác nhận và xóa hàng
function confirmDelete(rowId) {
  if (confirm("Bạn có chắc chắn muốn xóa?")) {
      const row = document.getElementById(rowId);
      if (row) row.remove();
      alert("Xóa thành công!");
  }
}

// Thêm hoặc chỉnh sửa hàng
function addOrEditRow(tableId, data, rowId = null) {
  const table = document.getElementById(tableId);
  let row;

  if (rowId) {
      // Sửa hàng
      row = document.getElementById(rowId);
      for (const key in data) {
          const cell = row.querySelector(`.cell-${key}`);
          if (cell) cell.textContent = data[key];
      }
      alert("Đã chỉnh sửa thành công!");
  } else {
      // Thêm hàng mới
      row = document.createElement("tr");
      const newId = `row-${Date.now()}`;
      row.id = newId;

      for (const key in data) {
          const cell = document.createElement("td");
          cell.className = `cell-${key}`;
          cell.textContent = data[key];
          row.appendChild(cell);
      }

      const actionsCell = document.createElement("td");
      actionsCell.innerHTML = `
          <button class="btn btn-warning" onclick="showModal('accountForm', true, ${JSON.stringify(data)})">Sửa</button>
          <button class="btn btn-danger" onclick="confirmDelete('${newId}')">Xóa</button>
      `;
      row.appendChild(actionsCell);

      table.querySelector("tbody").appendChild(row);
      alert("Đã thêm thành công!");
  }
}
