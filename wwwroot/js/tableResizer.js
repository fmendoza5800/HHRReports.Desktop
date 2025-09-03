window.initializeTableResizing = function() {
    // Remove any existing event listeners first
    if (window.tableResizerInitialized) {
        return;
    }
    
    setTimeout(() => {
        const table = document.querySelector('.table');
        if (!table) {
            console.log('Table not found, retrying...');
            setTimeout(() => window.initializeTableResizing(), 500);
            return;
        }

        let isResizing = false;
        let currentTh = null;
        let startX = 0;
        let startWidth = 0;

        // Add resizable class to all th elements
        const headers = table.querySelectorAll('thead th');
        headers.forEach(th => {
            th.classList.add('resizable');
            th.style.position = 'relative';
        });

        const handleMouseDown = (e) => {
            const th = e.target.closest('th');
            if (!th || !th.classList.contains('resizable')) return;

            // Check if click is on the resize handle (right 8px of th for better touch)
            const rect = th.getBoundingClientRect();
            const isHandle = e.clientX >= rect.right - 8 && e.clientX <= rect.right;

            if (isHandle) {
                e.preventDefault();
                isResizing = true;
                currentTh = th;
                startX = e.clientX;
                startWidth = th.offsetWidth;
                th.classList.add('resizing');
                document.body.style.cursor = 'col-resize';
                document.body.style.userSelect = 'none';
            }
        };

        const handleMouseMove = (e) => {
            // Show resize cursor when hovering over resize handle
            const th = e.target.closest('th');
            if (th && th.classList.contains('resizable') && !isResizing) {
                const rect = th.getBoundingClientRect();
                const isHandle = e.clientX >= rect.right - 8 && e.clientX <= rect.right;
                th.style.cursor = isHandle ? 'col-resize' : 'default';
            }

            if (!isResizing) return;

            const width = startWidth + (e.clientX - startX);
            if (width > 50) { // Minimum width
                currentTh.style.width = `${width}px`;
                currentTh.style.minWidth = `${width}px`;
                currentTh.style.maxWidth = `${width}px`;
            }
        };

        const handleMouseUp = () => {
            if (!isResizing) return;

            isResizing = false;
            if (currentTh) {
                currentTh.classList.remove('resizing');
                currentTh = null;
            }
            document.body.style.cursor = 'default';
            document.body.style.userSelect = '';
        };

        // Remove existing listeners if any
        document.removeEventListener('mousedown', handleMouseDown);
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);

        // Add new listeners
        document.addEventListener('mousedown', handleMouseDown);
        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
        
        window.tableResizerInitialized = true;
        console.log('Table resizing initialized successfully');
    }, 100);
}
