using System;

namespace ARM_library.Models
{
    public sealed class Issue
    {
        public int Id { get; set; } // IssueID
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Today;
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Выдано";
    }
}


