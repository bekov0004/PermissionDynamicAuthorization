namespace Infrastructure.Permission;

public static class Permissions
{
    public static class Students
    {
        public const string GetStudent = "Permissions.Students.GetStudent";
        public const string UpdateStudent = "Permissions.Students.UpdateStudent";
        public const string CreateStudent = "Permissions.Students.CreateStudent"; 
        public const string DeleteStudent = "Permissions.Students.DeleteStudent"; 
    }

    public static class Accounts
    {
        public const string UpdateRoleClaims = "Permissions.Accounts.UpdateRoleClaims";
    }
}