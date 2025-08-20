

function exportUserCSV() {
    // Show loading state
    const button = event.target;
    const originalText = button.innerHTML;
    button.innerHTML = '<i data-lucide="loader" class="w-4 h-4 inline mr-2 animate-spin"></i>Exporting...';
    button.disabled = true;

    fetch("http://localhost:5019/api/CSV/export", {
        method: "GET",
        redirect: "follow"
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'users.csv';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);

            // Show success message
            alert('Users exported successfully!');
        })
        .catch(error => {
            console.error('Export failed:', error);
            alert('Export failed. Please try again.');
        })
        .finally(() => {
            // Restore button state
            button.innerHTML = originalText;
            button.disabled = false;
        });
}

function exportTaskCSV() {
    // Show loading state
    const button = event.target;
    const originalText = button.innerHTML;
    button.innerHTML = '<i data-lucide="loader" class="w-4 h-4 inline mr-2 animate-spin"></i>Exporting...';
    button.disabled = true;

    fetch("http://localhost:5019/api/CSV/taskcsv", {
        method: "GET",
        redirect: "follow"
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'users.csv';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);

            // Show success message
            alert('Users exported successfully!');
        })
        .catch(error => {
            console.error('Export failed:', error);
            alert('Export failed. Please try again.');
        })
        .finally(() => {
            // Restore button state
            button.innerHTML = originalText;
            button.disabled = false;
        });
}



// Close modals with Escape key
$(document).on('keydown', function (e) {
    if (e.key === 'Escape') {
        if (!$('#deleteUserModal').hasClass('hidden')) {
            hideDeleteModal();
        }
        if (!$('#deleteTaskModal').hasClass('hidden')) {
            hideDeleteTaskModal();
        }
        if (!$('#task-templates-dropdown').hasClass('hidden')) {
            $('#task-templates-dropdown').hide();
        }
    }
});
