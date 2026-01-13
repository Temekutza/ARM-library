namespace ARM_library.Models
{
    public sealed class Reader
    {
        public int Id { get; set; } // ReaderID
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}


