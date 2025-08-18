# Task Management System (TMS) - Web Application

A professional Task Management System built with ASP.NET Core Razor Pages that integrates with a RESTful Web API backend. This application provides comprehensive task and user management capabilities with role-based access control.

## 🚀 Features

### Authentication & Authorization
- **JWT-based Authentication**: Secure login with JWT tokens
- **Role-based Access Control**: Admin and User roles with different permissions
- **Session Management**: Secure session handling with automatic token validation
- **Middleware Protection**: Custom authentication middleware for route protection

### User Management (Admin Only)
- **User CRUD Operations**: Create, read, update, and delete users
- **Role Assignment**: Assign Admin or User roles
- **User Profile Management**: Update personal information and change passwords
- **Automatic Password Generation**: Default passwords for new users

### Task Management
- **Task CRUD Operations**: Full task lifecycle management
- **Task Assignment**: Assign tasks to specific users
- **Status Tracking**: Track task progress (ToDo, InProgress, Done)
- **Priority Levels**: Low, Medium, High priority classification
- **Deadline Management**: Set and track task deadlines
- **Due Soon Alerts**: Highlight tasks due within 24 hours

### Dashboard & Analytics
- **Real-time Statistics**: Live dashboard with task counts and progress
- **Due Soon Tasks**: Quick view of upcoming deadlines
- **Recent Activity**: Latest task updates and announcements
- **Role-based Views**: Different dashboards for Admin and User roles

### Communication
- **Announcements**: System-wide announcements and notifications
- **Email Templates**: Pre-configured email templates for notifications

## 🏗️ Architecture

### Frontend (Web Application)
- **ASP.NET Core Razor Pages**: Server-side rendering with modern UI
- **Tailwind CSS**: Utility-first CSS framework for responsive design
- **Lucide Icons**: Beautiful, customizable icons
- **Chart.js**: Interactive charts and analytics
- **Custom Middleware**: Authentication and authorization middleware

### Backend Integration
- **RESTful API**: Integration with external Web API
- **HTTP Client**: Efficient API communication
- **Error Handling**: Comprehensive error handling and logging
- **Configuration Management**: Environment-based configuration

### Security Features
- **JWT Token Validation**: Automatic token validation and refresh
- **Session Security**: Secure session management
- **Role-based Authorization**: Fine-grained access control
- **Input Validation**: Server-side validation and sanitization

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Web API backend running (configured in appsettings.json)

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd TMS
```

### 2. Configure API Settings
Update `appsettings.json` with your API endpoint:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001",
    "Timeout": 30
  }
}
```

### 3. Install Dependencies
```bash
dotnet restore
```

### 4. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001`

## 🔧 Configuration

### API Endpoints
The application integrates with the following API endpoints:

#### Authentication
- `POST /api/Auth/login` - User login
- `POST /api/Auth/validate-token` - Token validation

#### Users
- `GET /api/Users` - Get all users (Admin only)
- `POST /api/Users` - Create user (Admin only)
- `GET /api/Users/{id}` - Get user details
- `PUT /api/Users/{id}` - Update user
- `DELETE /api/Users/{id}` - Delete user (Admin only)
- `POST /api/Users/{id}/change-password` - Change password

#### Tasks
- `GET /api/TaskItems` - Get all tasks (Admin only)
- `GET /api/TaskItems/my-tasks` - Get user's tasks
- `POST /api/TaskItems` - Create task (Admin only)
- `GET /api/TaskItems/{id}` - Get task details
- `PUT /api/TaskItems/{id}` - Update task (Admin only)
- `DELETE /api/TaskItems/{id}` - Delete task (Admin only)
- `PATCH /api/TaskItems/{id}/status` - Update task status
- `GET /api/TaskItems/due-soon` - Get due soon tasks
- `GET /api/TaskItems/dashboard-stats` - Get dashboard statistics

#### Announcements
- `GET /api/Announcements` - Get all announcements
- `POST /api/Announcements` - Create announcement (Admin only)
- `PUT /api/Announcements/{id}` - Update announcement (Admin only)
- `DELETE /api/Announcements/{id}` - Delete announcement (Admin only)

## 👥 User Roles & Permissions

### Admin Role
- **Full Access**: All features and operations
- **User Management**: Create, edit, delete users
- **Task Management**: Create, assign, edit, delete tasks
- **System Administration**: Manage announcements, view analytics
- **Reports**: Access to detailed reports and analytics

### User Role
- **Limited Access**: Personal task management only
- **View Tasks**: See assigned tasks and due dates
- **Update Status**: Change task status (ToDo → InProgress → Done)
- **Profile Management**: Update personal information
- **Dashboard**: View personal statistics and due soon tasks

## 🎨 UI/UX Features

### Modern Design
- **Responsive Layout**: Works on desktop, tablet, and mobile
- **Dark/Light Theme**: Professional color scheme
- **Interactive Elements**: Hover effects and smooth transitions
- **Loading States**: Professional loading indicators

### User Experience
- **Intuitive Navigation**: Clear menu structure and breadcrumbs
- **Real-time Updates**: Live dashboard statistics
- **Search & Filter**: Advanced filtering capabilities
- **Bulk Operations**: Efficient task and user management

## 🔒 Security Considerations

### Authentication
- JWT tokens with automatic expiration handling
- Secure session management
- Password hashing and validation
- CSRF protection

### Authorization
- Role-based access control
- Route-level protection
- API endpoint authorization
- User permission validation

### Data Protection
- Input validation and sanitization
- SQL injection prevention
- XSS protection
- Secure communication with API

## 📊 Dashboard Features

### Admin Dashboard
- **User Statistics**: Total users, active users, new registrations
- **Task Overview**: Total tasks, completion rates, overdue tasks
- **System Health**: API status and performance metrics
- **Quick Actions**: Create tasks, manage users, view reports

### User Dashboard
- **Personal Tasks**: Assigned tasks and progress
- **Due Soon Alerts**: Tasks due within 24 hours
- **Task Statistics**: Personal completion rates
- **Recent Activity**: Latest task updates

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
dotnet run --environment Production
```

### Docker (Optional)
```bash
docker build -t tms-web .
docker run -p 5001:5001 tms-web
```

## 📝 API Integration

The application is designed to work with a separate Web API backend. Ensure your API is running and accessible at the configured URL in `appsettings.json`.

### API Requirements
- RESTful API with JWT authentication
- CORS enabled for web application
- Proper error handling and status codes
- JSON response format

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## 🔄 Version History

- **v1.0.0**: Initial release with core functionality
- **v1.1.0**: Added dashboard analytics and reporting
- **v1.2.0**: Enhanced security and user management
- **v1.3.0**: Improved UI/UX and mobile responsiveness

---

**Note**: This application requires a running Web API backend to function properly. Ensure your API is configured and accessible before running the web application.
