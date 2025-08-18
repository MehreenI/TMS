let currentUserId;
function deleteUser(userId) {
    currentUserId = userId;

    // Reset confirmation input
    $('#deleteConfirmation').val('');
    $('#confirmDeleteBtn').prop('disabled', true);
    $('#deleteUserModal').removeClass('hidden');
    $('body').addClass('overflow-hidden');
}

function hideDeleteModal() {
    $('#deleteUserModal').addClass('hidden');
    $('body').removeClass('overflow-hidden');
}

function confirmDeleteUser() {
    // In a real app, this would send delete request to server
    alert('User deleted successfully!');
    hideDeleteModal();
    showView('all-users');
}

// Modal confirmation input validation
$(document).on('input', '#deleteConfirmation', function () {
    const value = $(this).val();
    const isValid = value === 'DELETE';
    $('#confirmDeleteBtn').prop('disabled', !isValid);
});

// Close modal when clicking outside
$(document).on('click', '#deleteUserModal', function (e) {
    if (e.target === this) {
        hideDeleteModal();
    }
});
