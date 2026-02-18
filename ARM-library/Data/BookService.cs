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

        public int Add(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            const string sql = "INSERT INTO Books (Title, Author, Publisher, Year, Quantity) VALUES (@Title, @Author, @Publisher, @Year, @Quantity);";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Publisher", book.Publisher ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", book.Year.HasValue ? (object)book.Year.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", book.Quantity);
            });
        }

        public int Update(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            const string sql = "UPDATE Books SET Title=@Title, Author=@Author, Publisher=@Publisher, Year=@Year, Quantity=@Quantity WHERE BookID=@BookID;";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Publisher", book.Publisher ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", book.Year.HasValue ? (object)book.Year.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", book.Quantity);
                cmd.Parameters.AddWithValue("@BookID", book.Id);
            });
        }

        public int Delete(int bookId)
        {
            return _db.ExecuteNonQuery("DELETE FROM Books WHERE BookID=@BookID;", cmd => cmd.Parameters.AddWithValue("@BookID", bookId));
        }
    }
}


