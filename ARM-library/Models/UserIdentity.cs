namespace ARM_library.Models
{
    public sealed class UserIdentity
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Role { get; set; } = "Librarian";

        public bool IsAdmin => string.Equals(Role, "Admin", System.StringComparison.OrdinalIgnoreCase);
    }
}


