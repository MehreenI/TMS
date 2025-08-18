// Sidebar toggle
function toggleSidebar() {
    $('#sidebar').toggleClass('sidebar-collapsed');
}

// Profile dropdown toggle
function toggleProfileDropdown() {
    $('#profile-dropdown').toggle();
}

// Close dropdown when clicking outside
$(document).click(function (e) {
    if (!$(e.target).closest('#profile-dropdown, [onclick="toggleProfileDropdown()"]').length) {
        $('#profile-dropdown').hide();
    }
});


// User Management Functions
let currentUserId = null;

// Sample user data (in real app, this would come from API)
const userData = {
    'john-doe': {
        id: 'john-doe',
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        email: 'john.doe@taskify.com',
        phone: '+1 (555) 123-4567',
        employeeId: 'EMP-001',
        role: 'Admin',
        department: 'Development',
        status: 'Active',
        avatar: 'JD',
        avatarGradient: 'from-blue-500 to-purple-600',
        tasksAssigned: 45,
        tasksCompleted: 38,
        projects: 7,
        completionRate: 84,
        joinDate: 'January 15, 2023',
        lastActive: '2 minutes ago'
    },
    'sarah-johnson': {
        id: 'sarah-johnson',
        firstName: 'Sarah',
        lastName: 'Johnson',
        fullName: 'Sarah Johnson',
        email: 'sarah.johnson@taskify.com',
        phone: '+1 (555) 234-5678',
        employeeId: 'EMP-002',
        role: 'Manager',
        department: 'Design',
        status: 'Active',
        avatar: 'SJ',
        avatarGradient: 'from-green-500 to-blue-600',
        tasksAssigned: 32,
        tasksCompleted: 28,
        projects: 5,
        completionRate: 88,
        joinDate: 'March 8, 2023',
        lastActive: '5 minutes ago'
    },
    'mike-chen': {
        id: 'mike-chen',
        firstName: 'Mike',
        lastName: 'Chen',
        fullName: 'Mike Chen',
        email: 'mike.chen@taskify.com',
        phone: '+1 (555) 345-6789',
        employeeId: 'EMP-003',
        role: 'Developer',
        department: 'Development',
        status: 'Away',
        avatar: 'MC',
        avatarGradient: 'from-purple-500 to-pink-600',
        tasksAssigned: 28,
        tasksCompleted: 22,
        projects: 4,
        completionRate: 79,
        joinDate: 'June 12, 2023',
        lastActive: '1 hour ago'
    }
};
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

    const requestOptions = {
        method: "DELETE",
        redirect: "follow"
    };

    fetch("http://localhost:40669/api/users/" + currentUserId, requestOptions)
        .then((response) => response.text())
        .then((result) => {
            alert('User deleted successfully!');
            hideDeleteModal();
            showView('all-users');
        })
        .catch((error) => console.error(error));
   
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

function exportUser() {
    // Show loading state
    const button = event.target;
    const originalText = button.innerHTML;
    button.innerHTML = '<i data-lucide="loader" class="w-4 h-4 inline mr-2 animate-spin"></i>Exporting...';
    button.disabled = true;

    fetch("http://localhost:40669/api/Users/export", {
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

// Task Management Functions
let currentTaskId = null;

// Sample task data (in real app, this would come from API)
const taskData = {
    'task-001': {
        id: 'task-001',
        title: 'Database Optimization',
        description: 'Optimize database queries for better performance. This includes reviewing slow queries, adding appropriate indexes, and restructuring complex joins. The goal is to reduce page load times by at least 30% and improve overall system responsiveness.',
        assignee: 'John Doe',
        assigneeId: 'john-doe',
        priority: 'High',
        status: 'In Progress',
        progress: 65,
        dueDate: '2025-08-25',
        estimatedHours: 8,
        storyPoints: 5,
        project: 'E-commerce Platform',
        category: 'Development',
        tags: ['database', 'performance', 'optimization', 'backend'],
        createdDate: '2025-08-15',
        statusIcon: 'play-circle',
        statusGradient: 'from-orange-500 to-red-600'
    },
    'task-002': {
        id: 'task-002',
        title: 'UI Design Update',
        description: 'Update user interface components for better user experience and modern design standards.',
        assignee: 'Sarah Johnson',
        assigneeId: 'sarah-johnson',
        priority: 'Medium',
        status: 'Completed',
        progress: 100,
        dueDate: '2025-08-20',
        estimatedHours: 6,
        storyPoints: 3,
        project: 'Brand Redesign',
        category: 'Design',
        tags: ['ui', 'design', 'frontend'],
        createdDate: '2025-08-10',
        statusIcon: 'check-circle',
        statusGradient: 'from-green-500 to-green-600'
    },
    'task-003': {
        id: 'task-003',
        title: 'API Integration',
        description: 'Integrate third-party payment API for secure transaction processing.',
        assignee: 'Mike Chen',
        assigneeId: 'mike-chen',
        priority: 'Critical',
        status: 'Pending',
        progress: 25,
        dueDate: '2025-08-30',
        estimatedHours: 12,
        storyPoints: 8,
        project: 'E-commerce Platform',
        category: 'Development',
        tags: ['api', 'payment', 'integration'],
        createdDate: '2025-08-18',
        statusIcon: 'clock',
        statusGradient: 'from-gray-500 to-gray-600'
    }
};

function showDeleteTaskModal(taskId) {
    currentTaskId = taskId;
    const task = taskData[taskId] || taskData['task-001'];

    // Update modal with task data
    $('#modalTaskTitle').text(task.title);
    $('#modalTaskAssignee').text(`Assigned to ${task.assignee}`);
    $('#modalTaskProject').text(`${task.project} • ${task.priority} Priority`);

    // Reset confirmation input
    $('#deleteTaskConfirmation').val('');
    $('#confirmDeleteTaskBtn').prop('disabled', true);

    // Show modal
    $('#deleteTaskModal').removeClass('hidden');

    // Prevent body scroll
    $('body').addClass('overflow-hidden');
}

function hideDeleteTaskModal() {
    $('#deleteTaskModal').addClass('hidden');
    $('body').removeClass('overflow-hidden');
}

function confirmDeleteTask() {
    // In a real app, this would send delete request to server
    alert('Task deleted successfully!');
    hideDeleteTaskModal();
    showView('all-tasks');
}

function clearTaskForm() {
    $('#task-title').val('');
    $('#task-description').val('');
    $('#task-priority').val('');
    $('#task-category').val('');
    $('#task-project').val('');
    $('#task-due-date').val('');
    $('#task-hours').val('');
    $('#task-story-points').val('');
    $('#task-tags').val('');
    $('input[name="assignee"]').prop('checked', false);
}

function saveTaskDraft() {
    alert('Task saved as draft!');
}

// Modal confirmation input validation for tasks
$(document).on('input', '#deleteTaskConfirmation', function () {
    const value = $(this).val();
    const isValid = value === 'DELETE';
    $('#confirmDeleteTaskBtn').prop('disabled', !isValid);
});

// Close task modal when clicking outside
$(document).on('click', '#deleteTaskModal', function (e) {
    if (e.target === this) {
        hideDeleteTaskModal();
    }
});


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

let currentEmailTab = 'registration';

function switchEmailTab(tabName) {
    // Update tab appearance
    $('.email-tab').removeClass('active');
    $('#tab-' + tabName).addClass('active');

    // Hide all tab content
    $('.email-tab-content').addClass('hidden');

    // Show selected tab content
    $('#email-content-' + tabName).removeClass('hidden');

    // Update current tab
    currentEmailTab = tabName;

    // Reinitialize Lucide icons
    lucide.createIcons();
}

function insertShortcode(textareaId, shortcode) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return;

    const startPos = textarea.selectionStart;
    const endPos = textarea.selectionEnd;
    const textBefore = textarea.value.substring(0, startPos);
    const textAfter = textarea.value.substring(endPos, textarea.value.length);

    textarea.value = textBefore + shortcode + textAfter;
    textarea.focus();
    textarea.setSelectionRange(startPos + shortcode.length, startPos + shortcode.length);

    // Add a subtle visual feedback
    $(textarea).addClass('ring-2 ring-red-200').removeClass('ring-2 ring-red-200', 300);
}

function saveEmailTemplate(templateType) {
    const subjectField = document.getElementById(templateType + '-subject');
    const contentField = document.getElementById(templateType + '-content');

    if (!subjectField || !contentField) {
        alert('Error: Template fields not found!');
        return;
    }

    const subject = subjectField.value;
    const content = contentField.value;

    if (!subject.trim() || !content.trim()) {
        alert('Please fill in both subject and content before saving.');
        return;
    }

    // In a real application, this would send the data to the server
    // For now, we'll just show a success message

    // Visual feedback
    const saveBtn = $(`button[onclick="saveEmailTemplate('${templateType}')"]`);
    const originalText = saveBtn.html();

    saveBtn.html('<i data-lucide="check" class="w-4 h-4 inline mr-2"></i>Saved!').addClass('bg-green-600 hover:bg-green-700').removeClass('bg-red-600 hover:bg-red-700');

    setTimeout(() => {
        saveBtn.html(originalText).removeClass('bg-green-600 hover:bg-green-700').addClass('bg-red-600 hover:bg-red-700');
        lucide.createIcons();
    }, 2000);

    alert(`${templateType.replace('-', ' ').replace(/\b\w/g, l => l.toUpperCase())} template saved successfully!`);
}

function previewEmail(templateType) {
    const subjectField = document.getElementById(templateType + '-subject');
    const contentField = document.getElementById(templateType + '-content');

    if (!subjectField || !contentField) {
        alert('Error: Template fields not found!');
        return;
    }

    const subject = subjectField.value;
    const content = contentField.value;

    // Create a preview window
    const previewWindow = window.open('', '_blank', 'width=600,height=500,scrollbars=yes,resizable=yes');
    previewWindow.document.write(`
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Email Preview - ${templateType}</title>
                    <style>
                        body { font-family: Arial, sans-serif; padding: 20px; line-height: 1.6; }
                        .preview-header { background: #ef4444; color: white; padding: 15px; margin: -20px -20px 20px -20px; }
                        .subject { font-size: 18px; font-weight: bold; margin-bottom: 20px; color: #333; }
                        .content { white-space: pre-line; color: #555; }
                        .shortcode { background: #fef2f2; color: #ef4444; padding: 2px 6px; border-radius: 3px; font-weight: bold; }
                    </style>
                </head>
                <body>
                    <div class="preview-header">
                        <h2>📧 Email Preview</h2>
                        <p style="margin: 0; opacity: 0.9;">Template: ${templateType.replace('-', ' ').replace(/\b\w/g, l => l.toUpperCase())}</p>
                    </div>
                    <div class="subject">Subject: ${subject}</div>
                    <div class="content">${content.replace(/\[([^\]]+)\]/g, '<span class="shortcode">[$1]</span>')}</div>
                </body>
                </html>
            `);
    previewWindow.document.close();
}

function testEmail(templateType) {
    const emailAddress = prompt('Enter email address to send test email to:');
    if (!emailAddress) return;

    if (!emailAddress.includes('@') || !emailAddress.includes('.')) {
        alert('Please enter a valid email address.');
        return;
    }

    // In a real application, this would send a test email
    alert(`Test email sent to ${emailAddress}!\n\nNote: In a real application, this would send the ${templateType} template to the specified address.`);
}

function resetEmailTemplate(templateType) {
    if (!confirm('Are you sure you want to reset this template to default? This will overwrite any changes you have made.')) {
        return;
    }

    // Default templates
    const defaultTemplates = {
        'registration': {
            subject: 'Welcome to Taskify - Your Account is Ready!',
            content: `Dear [username],

Welcome to Taskify! We're excited to have you join our team.

Your account has been successfully created with the following details:
• Email: [email]
• Login URL: [login_url]
• Temporary Password: [temp_password]

To get started:
1. Click the login link above
2. Use your email and temporary password to sign in
3. You'll be prompted to create a new secure password
4. Complete your profile setup

If you have any questions or need assistance, please don't hesitate to contact our support team at [support_email].

Best regards,
The Taskify Team
[company_name]`
        },
        'login': {
            subject: 'Login Notification - Taskify Account Access',
            content: `Hello [username],

We noticed a login to your Taskify account.

Login Details:
• Time: [login_time]
• Date: [login_date]
• IP Address: [ip_address]
• Device: [device_info]
• Location: [location]

If this was you, no action is needed. Your account remains secure.

If you don't recognize this login, please:
1. Change your password immediately: [change_password_url]
2. Contact our security team: [security_email]
3. Review your account activity

Best regards,
The Taskify Security Team`
        },
        'project-assign': {
            subject: 'New Project Assignment: [project_name]',
            content: `Hello [username],

You have been assigned to a new project in Taskify!

Project Details:
• Project Name: [project_name]
• Description: [project_description]
• Priority: [project_priority]
• Due Date: [project_due_date]
• Project Manager: [project_manager]

Your Role: [assigned_role]

Tasks Assigned to You:
[assigned_tasks]

To get started:
1. Access the project: [project_url]
2. Review the project requirements
3. Check your assigned tasks
4. Contact [project_manager] if you have questions

We're excited to have you on this project!

Best regards,
[assigner_name]
Taskify Project Management`
        },
        'forgot-password': {
            subject: 'Reset Your Taskify Password',
            content: `Hello [username],

We received a request to reset your Taskify account password.

If you made this request, click the button below to reset your password:

[reset_password_button]

Or copy and paste this link into your browser:
[reset_password_url]

This password reset link will expire in [expiry_time] hours for security reasons.

Password Reset Details:
• Request Time: [request_time]
• IP Address: [ip_address]
• Device: [device_info]

If you didn't request a password reset, please ignore this email. Your password will remain unchanged.

For additional security:
• Never share your password with anyone
• Use a strong, unique password
• Contact support if you notice suspicious activity: [support_email]

Best regards,
The Taskify Security Team`
        }
    };

    const template = defaultTemplates[templateType];
    if (template) {
        document.getElementById(templateType + '-subject').value = template.subject;
        document.getElementById(templateType + '-content').value = template.content;

        alert('Template reset to default successfully!');
    }
}

// Initialize email templates when the view is shown
function initializeEmailTemplates() {
    // Set default active tab
    switchEmailTab('registration');

    // Reinitialize Lucide icons
    lucide.createIcons();
}

// Call initialization when email templates view is accessed
$(document).ready(function () {
    // Add event listener for when email templates view is shown
    const originalShowView = window.showView;
    window.showView = function (viewName) {
        originalShowView(viewName);
        if (viewName === 'email-templates') {
            setTimeout(initializeEmailTemplates, 100);
        }
    };
});



// Initialize the dashboard
$(document).ready(function () {

    // Initialize charts
    initializeCharts();
});

// Chart initialization function
function initializeCharts() {
    // Add loading states
    $('.chart-container').html('<div class="chart-loading"><i data-lucide="loader-2" class="w-6 h-6 animate-spin mr-2"></i>Loading chart...</div>');

    // Simulate loading delay for better UX
    setTimeout(function () {
        // Add fade-in animation to charts
        $('.content-card').addClass('chart-fade-in');
        // Performance Overview Chart (Line Chart)
        const performanceCtx = document.getElementById('performanceChart').getContext('2d');
        new Chart(performanceCtx, {
            type: 'line',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Tasks Completed',
                    data: [12, 19, 8, 15, 22, 18, 25],
                    borderColor: '#ef4444',
                    backgroundColor: 'rgba(239, 68, 68, 0.1)',
                    tension: 0.4,
                    fill: true,
                    pointBackgroundColor: '#ef4444',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    pointRadius: 6,
                    pointHoverRadius: 8
                }, {
                    label: 'New Projects',
                    data: [2, 3, 1, 4, 2, 3, 5],
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    tension: 0.4,
                    fill: true,
                    pointBackgroundColor: '#3b82f6',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    pointRadius: 6,
                    pointHoverRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            padding: 20,
                            font: {
                                size: 12
                            }
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        borderColor: '#ef4444',
                        borderWidth: 1
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: '#f1f5f9'
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280'
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280'
                        }
                    }
                },
                animation: {
                    duration: 2000,
                    easing: 'easeInOutQuart'
                }
            }
        });

        // Task Status Distribution (Doughnut Chart)
        const taskStatusCtx = document.getElementById('taskStatusChart').getContext('2d');
        new Chart(taskStatusCtx, {
            type: 'doughnut',
            data: {
                labels: ['Completed', 'In Progress', 'Pending'],
                datasets: [{
                    data: [65, 25, 10],
                    backgroundColor: [
                        '#10b981',
                        '#f59e0b',
                        '#ef4444'
                    ],
                    borderWidth: 3,
                    borderColor: '#ffffff',
                    hoverBorderWidth: 5,
                    hoverBorderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        callbacks: {
                            label: function (context) {
                                return context.label + ': ' + context.parsed + '%';
                            }
                        }
                    }
                },
                cutout: '70%',
                animation: {
                    animateRotate: true,
                    duration: 2000
                }
            }
        });

        // Monthly Progress Chart (Bar Chart)
        const monthlyProgressCtx = document.getElementById('monthlyProgressChart').getContext('2d');
        new Chart(monthlyProgressCtx, {
            type: 'bar',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Projects Completed',
                    data: [8, 12, 6, 15, 10, 18],
                    backgroundColor: '#ef4444',
                    borderRadius: 8,
                    borderSkipped: false,
                    hoverBackgroundColor: '#dc2626'
                }, {
                    label: 'Tasks Completed',
                    data: [45, 67, 32, 89, 55, 92],
                    backgroundColor: '#f59e0b',
                    borderRadius: 8,
                    borderSkipped: false,
                    hoverBackgroundColor: '#d97706'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            padding: 20,
                            font: {
                                size: 12
                            }
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: '#f1f5f9'
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280'
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280'
                        }
                    }
                },
                animation: {
                    duration: 1500,
                    easing: 'easeOutQuart'
                }
            }
        });

        // Team Productivity Chart (Horizontal Bar Chart)
        const teamProductivityCtx = document.getElementById('teamProductivityChart').getContext('2d');
        new Chart(teamProductivityCtx, {
            type: 'bar',
            data: {
                labels: ['Development', 'Design', 'Marketing', 'QA', 'DevOps'],
                datasets: [{
                    label: 'Productivity Score',
                    data: [92, 85, 78, 88, 94],
                    backgroundColor: [
                        '#ef4444',
                        '#3b82f6',
                        '#10b981',
                        '#f59e0b',
                        '#8b5cf6'
                    ],
                    hoverBackgroundColor: [
                        '#dc2626',
                        '#2563eb',
                        '#059669',
                        '#d97706',
                        '#7c3aed'
                    ],
                    borderRadius: 8,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        callbacks: {
                            label: function (context) {
                                return 'Score: ' + context.parsed.x + '%';
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        max: 100,
                        grid: {
                            color: '#f1f5f9'
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280',
                            callback: function (value) {
                                return value + '%';
                            }
                        }
                    },
                    y: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            },
                            color: '#6b7280'
                        }
                    }
                },
                animation: {
                    duration: 2000,
                    easing: 'easeOutBounce'
                }
            }
        });

        // User Activity Chart (for Analytics page)
        const userActivityCtx = document.getElementById('userActivityChart');
        if (userActivityCtx) {
            new Chart(userActivityCtx.getContext('2d'), {
                type: 'line',
                data: {
                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                    datasets: [{
                        label: 'Active Users',
                        data: [650, 740, 680, 920, 850, 1200, 1100, 1350, 1180, 1450, 1320, 1600],
                        borderColor: '#ef4444',
                        backgroundColor: 'rgba(239, 68, 68, 0.1)',
                        tension: 0.4,
                        fill: true,
                        pointBackgroundColor: '#ef4444',
                        pointBorderColor: '#ffffff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    }, {
                        label: 'New Registrations',
                        data: [120, 150, 100, 180, 140, 220, 190, 250, 210, 280, 240, 320],
                        borderColor: '#3b82f6',
                        backgroundColor: 'rgba(59, 130, 246, 0.1)',
                        tension: 0.4,
                        fill: true,
                        pointBackgroundColor: '#3b82f6',
                        pointBorderColor: '#ffffff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: {
                        intersect: false,
                        mode: 'index'
                    },
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                padding: 20,
                                font: {
                                    size: 12
                                }
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            titleColor: '#ffffff',
                            bodyColor: '#ffffff',
                            borderColor: '#ef4444',
                            borderWidth: 1
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: {
                                color: '#f1f5f9'
                            },
                            ticks: {
                                font: {
                                    size: 11
                                },
                                color: '#6b7280'
                            }
                        },
                        x: {
                            grid: {
                                display: false
                            },
                            ticks: {
                                font: {
                                    size: 11
                                },
                                color: '#6b7280'
                            }
                        }
                    },
                    animation: {
                        duration: 2000,
                        easing: 'easeInOutQuart'
                    }
                }
            });
        }

        // Device Analytics Chart (Doughnut Chart for Analytics page)
        const deviceCtx = document.getElementById('deviceChart');
        if (deviceCtx) {
            new Chart(deviceCtx.getContext('2d'), {
                type: 'doughnut',
                data: {
                    labels: ['Desktop', 'Mobile', 'Tablet'],
                    datasets: [{
                        data: [58, 32, 10],
                        backgroundColor: [
                            '#3b82f6',
                            '#10b981',
                            '#8b5cf6'
                        ],
                        borderWidth: 3,
                        borderColor: '#ffffff',
                        hoverBorderWidth: 5,
                        hoverBorderColor: '#ffffff'
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: false
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            titleColor: '#ffffff',
                            bodyColor: '#ffffff',
                            callbacks: {
                                label: function (context) {
                                    return context.label + ': ' + context.parsed + '%';
                                }
                            }
                        }
                    },
                    cutout: '60%',
                    animation: {
                        animateRotate: true,
                        duration: 2000
                    }
                }
            });
        }

        // Reinitialize Lucide icons after chart loading
        lucide.createIcons();
    }, 500);
}