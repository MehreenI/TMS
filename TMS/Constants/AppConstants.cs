namespace TMS.Constants
{
    public static class AppConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public static class SessionKeys
        {
            public const string JwtToken = "JWTToken";
            public const string UserRole = "UserRole";
            public const string UserId = "UserId";
            public const string UserEmail = "UserEmail";
        }

        public static class TaskStatus
        {
            public const string ToDo = "ToDo";
            public const string InProgress = "InProgress";
            public const string Done = "Done";
        }

        public static class TaskPriority
        {
            public const string Low = "Low";
            public const string Medium = "Medium";
            public const string High = "High";
        }

        public static class Routes
        {
            public const string Login = "/Home/Login";
            public const string Index = "/Home/Index";
            public const string Error = "/Error";
        }

        public static class ApiEndpoints
        {
            public const string Users = "/api/Users";
            public const string Tasks = "/api/TaskItems";
            public const string Auth = "/api/Auth";
        }
    }
}
