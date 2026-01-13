using System;
using System.Data;
using ARM_library.Models;

namespace ARM_library.Data
{
    public sealed class BookService
    {
        private readonly DataAccess _db;
        public BookService(DataAccess db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public DataTable GetAll()
        {
            const string sql = @"
SELECT
  b.BookID,
  b.Title,
  b.Author,
  b.Publisher,
  b.Year,
  b.Quantity,
  (b.Quantity - IFNULL(x.ActiveIssues, 0)) AS Available
FROM Books b
LEFT JOIN (
  SELECT BookID, COUNT(*) AS ActiveIssues
  FROM Issues
  WHERE Status = 'Выдано'
  GROUP BY BookID
) x ON x.BookID = b.BookID
ORDER BY b.BookID DESC;";

            return _db.ExecuteTable(sql);
        }

        public DataTable GetLookupAvailableOnly()
        {
            const string sql = @"
SELECT
  b.BookID,
  CONCAT(b.Title, ' — ', IFNULL(b.Author, '')) AS DisplayName,
  (b.Quantity - IFNULL(x.ActiveIssues, 0)) AS Available
FROM Books b
LEFT JOIN (
  SELECT BookID, COUNT(*) AS ActiveIssues
  FROM Issues
  WHERE Status = 'Выдано'
  GROUP BY BookID
) x ON x.BookID = b.BookID
WHERE (b.Quantity - IFNULL(x.ActiveIssues, 0)) > 0
ORDER BY b.Title;";

            return _db.ExecuteTable(sql);
        }
    }
}


