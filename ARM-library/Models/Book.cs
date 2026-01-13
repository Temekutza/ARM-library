namespace ARM_library.Models
{
    public sealed class Book
    {
        public int Id { get; set; } // BookID
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int? Year { get; set; } // SMALLINT
        public int Quantity { get; set; } = 1;
    }
}


