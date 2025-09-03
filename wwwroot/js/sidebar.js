// Wait for the page to fully load
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM loaded, setting up sidebar toggle');
    setupSidebarToggle();
});

// Also try after Blazor loads
window.addEventListener('load', function() {
    console.log('Window loaded, setting up sidebar toggle');
    setupSidebarToggle();
});

// Try again after a delay for Blazor
setTimeout(setupSidebarToggle, 1000);
setTimeout(setupSidebarToggle, 2000);

function setupSidebarToggle() {
    // Find the button by ID
    var button = document.getElementById('sidebarToggle');
    
    if (!button) {
        console.log('Button not found, will try again...');
        return;
    }
    
    // Check if we already attached the handler
    if (button.hasAttribute('data-handler-attached')) {
        return;
    }
    
    console.log('Attaching click handler to sidebar toggle button');
    button.setAttribute('data-handler-attached', 'true');
    
    // Attach click handler
    button.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        console.log('Sidebar toggle clicked!');
        
        // Find the page element
        var page = document.getElementById('app-layout');
        if (!page) {
            page = document.querySelector('.page');
        }
        
        if (!page) {
            console.error('Could not find page element');
            return;
        }
        
        // Toggle the collapsed class
        var isCollapsed = page.classList.contains('sidebar-collapsed');
        
        if (isCollapsed) {
            page.classList.remove('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', 'false');
            console.log('Sidebar expanded');
        } else {
            page.classList.add('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', 'true');
            console.log('Sidebar collapsed');
        }
        
        // Update button icon
        var icon = button.querySelector('i');
        if (icon) {
            if (page.classList.contains('sidebar-collapsed')) {
                icon.className = 'fas fa-chevron-right';
            } else {
                icon.className = 'fas fa-bars';
            }
        }
    });
    
    // Restore saved state
    var savedState = localStorage.getItem('sidebar-collapsed');
    if (savedState === 'true') {
        var page = document.getElementById('app-layout');
        if (!page) {
            page = document.querySelector('.page');
        }
        
        if (page) {
            page.classList.add('sidebar-collapsed');
            var icon = button.querySelector('i');
            if (icon) {
                icon.className = 'fas fa-chevron-right';
            }
            console.log('Restored collapsed state from localStorage');
        }
    }
}

console.log('sidebar.js loaded');