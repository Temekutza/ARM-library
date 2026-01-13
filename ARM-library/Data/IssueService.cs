using System;
using System.Data;
using ARM_library.Models;

namespace ARM_library.Data
{
    public sealed class IssueService
    {
        private readonly DataAccess _db;
        public IssueService(DataAccess db) => _db = db;

        public DataTable GetAll()
        {
            const string sql = @"
SELECT
  i.IssueID,
  i.BookID,
  b.Title AS BookTitle,
  i.ReaderID,
  r.FullName AS ReaderName,
  i.EmployeeID,
  e.FullName AS EmployeeName,
  i.IssueDate,
  i.ReturnDate,
  i.Status
FROM Issues i
JOIN Books b ON b.BookID = i.BookID
JOIN Readers r ON r.ReaderID = i.ReaderID
JOIN Employees e ON e.EmployeeID = i.EmployeeID
ORDER BY i.IssueID DESC;";
            return _db.ExecuteTable(sql);
        }

        public int IssueBook(Issue issue)
        {
            if (issue == null) throw new ArgumentNullException(nameof(issue));

            const string checkSql = @"
SELECT
  (b.Quantity - IFNULL(x.ActiveIssues, 0)) AS Available
FROM Books b
LEFT JOIN (
  SELECT BookID, COUNT(*) AS ActiveIssues
  FROM Issues
  WHERE Status = 'Выдано'
  GROUP BY BookID
) x ON x.BookID = b.BookID
WHERE b.BookID = @BookID;";

            var availableObj = _db.ExecuteScalar(checkSql, cmd => cmd.Parameters.AddWithValue("@BookID", issue.BookId));
            var available = availableObj == null || availableObj == DBNull.Value ? 0 : Convert.ToInt32(availableObj);
            if (available <= 0)
                throw new InvalidOperationException("Нет доступных экземпляров этой книги для выдачи.");

            const string insertSql = @"
INSERT INTO Issues (BookID, ReaderID, EmployeeID, IssueDate, ReturnDate, Status)
VALUES (@BookID, @ReaderID, @EmployeeID, @IssueDate, @ReturnDate, @Status);";

            return _db.ExecuteNonQuery(insertSql, cmd =>
            {
                cmd.Parameters.AddWithValue("@BookID", issue.BookId);
                cmd.Parameters.AddWithValue("@ReaderID", issue.ReaderId);
                cmd.Parameters.AddWithValue("@EmployeeID", issue.EmployeeId);
                cmd.Parameters.AddWithValue("@IssueDate", issue.IssueDate);
                cmd.Parameters.AddWithValue("@ReturnDate", (object)issue.ReturnDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(issue.Status) ? "Выдано" : issue.Status);
            });
        }

        public int ReturnIssue(int issueId, DateTime returnDate)
        {
            const string sql = "UPDATE Issues SET ReturnDate=@ReturnDate, Status='Возвращено' WHERE IssueID=@IssueID;";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                cmd.Parameters.AddWithValue("@IssueID", issueId);
            });
        }
    }
}


