using System;
using System.Data;
using ARM_library.Models;

namespace ARM_library.Data
{
    public sealed class EmployeeService
    {
        private readonly DataAccess _db;
        public EmployeeService(DataAccess db) => _db = db;

        public DataTable GetAll()
        {
            return _db.ExecuteTable("SELECT EmployeeID, FullName, Position FROM Employees ORDER BY EmployeeID DESC;");
        }

        public DataTable GetLookup()
        {
            return _db.ExecuteTable("SELECT EmployeeID, FullName AS DisplayName FROM Employees ORDER BY FullName;");
        }

        public int Add(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            const string sql = "INSERT INTO Employees (FullName, Position) VALUES (@FullName, @Position);";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@FullName", employee.FullName);
                cmd.Parameters.AddWithValue("@Position", employee.Position);
            });
        }

        public int Update(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            const string sql = "UPDATE Employees SET FullName=@FullName, Position=@Position WHERE EmployeeID=@EmployeeID;";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@FullName", employee.FullName);
                cmd.Parameters.AddWithValue("@Position", employee.Position);
                cmd.Parameters.AddWithValue("@EmployeeID", employee.Id);
            });
        }

        public int Delete(int employeeId)
        {
            return _db.ExecuteNonQuery("DELETE FROM Employees WHERE EmployeeID=@EmployeeID;", cmd => cmd.Parameters.AddWithValue("@EmployeeID", employeeId));
        }
    }
}


