using System;
using System.Data;
using ARM_library.Models;
using ARM_library.Security;

namespace ARM_library.Data
{
    public sealed class AuthService
    {
        private readonly DataAccess _db;
        public AuthService(DataAccess db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public UserIdentity Authenticate(string username, string password)
        {
            username = (username ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Введите логин и пароль.");

            const string sql = @"
SELECT
  u.UserID,
  u.EmployeeID,
  u.Username,
  u.PasswordHash,
  u.Role,
  e.FullName AS EmployeeName
FROM Users u
JOIN Employees e ON e.EmployeeID = u.EmployeeID
WHERE u.Username = @Username
LIMIT 1;";

            var dt = _db.ExecuteTable(sql, cmd => cmd.Parameters.AddWithValue("@Username", username));
            if (dt.Rows.Count == 0)
                throw new InvalidOperationException("Неверный логин или пароль.");

            var row = dt.Rows[0];
            var storedHash = Convert.ToString(row["PasswordHash"]) ?? string.Empty;
            var computed = PasswordHasher.Sha256Hex(password);

            if (!string.Equals(storedHash, computed, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Неверный логин или пароль.");

            return new UserIdentity
            {
                UserId = Convert.ToInt32(row["UserID"]),
                EmployeeId = Convert.ToInt32(row["EmployeeID"]),
                Username = Convert.ToString(row["Username"]) ?? username,
                Role = Convert.ToString(row["Role"]) ?? "Librarian",
                EmployeeName = Convert.ToString(row["EmployeeName"]) ?? string.Empty
            };
        }
    }
}


