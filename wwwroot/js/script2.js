document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const mainContent = document.querySelector('.flex-grow-1');
    const toggleIcon = document.getElementById('toggleIcon');
    let isSidebarCollapsed = false;

    // Sidebar Toggle Function
    function toggleSidebar() {
        isSidebarCollapsed = !isSidebarCollapsed;
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
        toggleIcon.classList.toggle('fa-bars');
        toggleIcon.classList.toggle('fa-times');

        // Save state to localStorage
        localStorage.setItem('sidebarState', isSidebarCollapsed ? 'collapsed' : 'expanded');
    }

    // Initialize sidebar state from localStorage
    const savedState = localStorage.getItem('sidebarState');
    if (savedState === 'collapsed') {
        toggleSidebar();
    }

    // Event Listeners
    sidebarToggle.addEventListener('click', toggleSidebar);

    // Submenu Toggle Animation
    const dropdownToggles = document.querySelectorAll('.dropdown-toggle');
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const submenu = document.querySelector(this.getAttribute('data-bs-target'));
            if (submenu) {
                submenu.style.transition = 'max-height 0.3s ease-in-out';
                if (submenu.style.maxHeight) {
                    submenu.style.maxHeight = null;
                } else {
                    submenu.style.maxHeight = submenu.scrollHeight + "px";
                }
            }
        });
    });

    // Add hover effect for sidebar items
    const sidebarItems = document.querySelectorAll('.sidebar .nav-link');
    sidebarItems.forEach(item => {
        item.addEventListener('mouseenter', function () {
            if (sidebar.classList.contains('collapsed')) {
                const tooltip = document.createElement('div');
                tooltip.className = 'sidebar-tooltip';
                tooltip.textContent = this.querySelector('.menu-text').textContent;
                tooltip.style.cssText = `
                    position: fixed;
                    background: #333;
                    color: white;
                    padding: 5px 10px;
                    border-radius: 4px;
                    font-size: 12px;
                    z-index: 1001;
                    opacity: 0;
                    transition: opacity 0.2s;
                `;

                const rect = this.getBoundingClientRect();
                tooltip.style.left = rect.right + 10 + 'px';
                tooltip.style.top = rect.top + (rect.height / 2 - 10) + 'px';

                document.body.appendChild(tooltip);
                setTimeout(() => tooltip.style.opacity = '1', 10);

                this.addEventListener('mouseleave', function () {
                    tooltip.remove();
                }, { once: true });
            }
        });
    });

    // Add smooth scroll for content
    const content = document.getElementById('content');
    content.style.scrollBehavior = 'smooth';

    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
});