using System;
using System.Data;
using ARM_library.Models;

namespace ARM_library.Data
{
    public sealed class ReaderService
    {
        private readonly DataAccess _db;
        public ReaderService(DataAccess db) => _db = db;

        public DataTable GetAll()
        {
            return _db.ExecuteTable("SELECT ReaderID, FullName, Phone, Email FROM Readers ORDER BY ReaderID DESC;");
        }

        public DataTable GetLookup()
        {
            return _db.ExecuteTable("SELECT ReaderID, FullName AS DisplayName FROM Readers ORDER BY FullName;");
        }

        public int Add(Reader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            const string sql = "INSERT INTO Readers (FullName, Phone, Email) VALUES (@FullName, @Phone, @Email);";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@FullName", reader.FullName);
                cmd.Parameters.AddWithValue("@Phone", reader.Phone);
                cmd.Parameters.AddWithValue("@Email", reader.Email);
            });
        }

        public int Update(Reader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            const string sql = "UPDATE Readers SET FullName=@FullName, Phone=@Phone, Email=@Email WHERE ReaderID=@ReaderID;";
            return _db.ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@FullName", reader.FullName);
                cmd.Parameters.AddWithValue("@Phone", reader.Phone);
                cmd.Parameters.AddWithValue("@Email", reader.Email);
                cmd.Parameters.AddWithValue("@ReaderID", reader.Id);
            });
        }

        public int Delete(int readerId)
        {
            return _db.ExecuteNonQuery("DELETE FROM Readers WHERE ReaderID=@ReaderID;", cmd => cmd.Parameters.AddWithValue("@ReaderID", readerId));
        }
    }
}


