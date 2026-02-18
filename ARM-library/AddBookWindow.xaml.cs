using System;
using System.Windows;
using ARM_library.Models;

namespace ARM_library
{
    public partial class AddBookWindow : Window
    {
        public Book Book { get; private set; }
        private readonly bool _isEditMode;
        private readonly int _editId;

        public AddBookWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            TitleBox.Focus();
        }

        public AddBookWindow(Book book) : this()
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            _isEditMode = true;
            _editId = book.Id;

            Title = "Редактирование книги";
            HeaderText.Text = "Редактирование книги";
            OkButton.Content = "Сохранить";

            TitleBox.Text = book.Title;
            AuthorBox.Text = book.Author;
            PublisherBox.Text = book.Publisher;
            YearBox.Text = book.Year?.ToString() ?? string.Empty;
            QuantityBox.Text = book.Quantity.ToString();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;

            var title = TitleBox.Text?.Trim();
            if (string.IsNullOrEmpty(title))
            {
                ErrorText.Text = "Название книги обязательно.";
                TitleBox.Focus();
                return;
            }

            int? year = null;
            var yearText = YearBox.Text?.Trim();
            if (!string.IsNullOrEmpty(yearText))
            {
                if (!int.TryParse(yearText, out int y) || y < 1 || y > DateTime.Now.Year + 1)
                {
                    ErrorText.Text = "Некорректный год издания.";
                    YearBox.Focus();
                    return;
                }
                year = y;
            }

            var quantityText = QuantityBox.Text?.Trim();
            if (!int.TryParse(quantityText, out int quantity) || quantity < 1)
            {
                ErrorText.Text = "Количество должно быть положительным числом.";
                QuantityBox.Focus();
                return;
            }

            Book = new Book
            {
                Id = _isEditMode ? _editId : 0,
                Title = title,
                Author = AuthorBox.Text?.Trim() ?? string.Empty,
                Publisher = PublisherBox.Text?.Trim() ?? string.Empty,
                Year = year,
                Quantity = quantity
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
