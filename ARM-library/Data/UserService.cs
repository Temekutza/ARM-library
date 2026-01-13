using System;
using System.Data;

namespace ARM_library.Data
{
    public sealed class UserService
    {
        private readonly DataAccess _db;
        public UserService(DataAccess db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public bool UsernameExists(string username)
        {
            const string sql = "SELECT COUNT(*) FROM Users WHERE Username=@Username;";
            var obj = _db.ExecuteScalar(sql, cmd => cmd.Parameters.AddWithValue("@Username", username));
            return Convert.ToInt32(obj) > 0;
        }

        public bool EmployeeHasUser(int employeeId)
        {
            const string sql = "SELECT COUNT(*) FROM Users WHERE EmployeeID=@EmployeeID;";
            var obj = _db.ExecuteScalar(sql, cmd => cmd.Parameters.AddWithValue("@EmployeeID", employeeId));
            return Convert.ToInt32(obj) > 0;
        }

        public int CreateUser(int employeeId, string username, string passwordHash, string role)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("username is empty", nameof(username));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("passwordHash is empty", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(role)) role = "Librarian";

            const string sql = @"
INSERT INTO Users (EmployeeID, Username, PasswordHash, Role)
VALUES (@EmployeeID, @Username, @PasswordHash, @Role);";

            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@Role", role);
            });
        }
    }
}


