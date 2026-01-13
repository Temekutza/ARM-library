using System;
using System.Windows;
using ARM_library.Data;
using ARM_library.Models;
using ARM_library.Security;

namespace ARM_library
{
    public partial class IssueWindow : Window
    {
        private readonly DataAccess _db;
        private readonly BookService _books;
        private readonly ReaderService _readers;
        private readonly EmployeeService _employees;

        public Issue Issue { get; private set; }

        public IssueWindow(DataAccess db)
        {
            InitializeComponent();

            _db = db ?? throw new ArgumentNullException(nameof(db));
            _books = new BookService(_db);
            _readers = new ReaderService(_db);
            _employees = new EmployeeService(_db);

            IssueDatePicker.SelectedDate = DateTime.Today;

            BooksBox.ItemsSource = _books.GetLookupAvailableOnly().DefaultView;
            BooksBox.DisplayMemberPath = "DisplayName";
            BooksBox.SelectedValuePath = "BookID";

            ReadersBox.ItemsSource = _readers.GetLookup().DefaultView;
            ReadersBox.DisplayMemberPath = "DisplayName";
            ReadersBox.SelectedValuePath = "ReaderID";

            EmployeesBox.ItemsSource = _employees.GetLookup().DefaultView;
            EmployeesBox.DisplayMemberPath = "DisplayName";
            EmployeesBox.SelectedValuePath = "EmployeeID";

            // Если вошёл сотрудник — по умолчанию выберем его
            var current = UserContext.Current;
            if (current != null)
                EmployeesBox.SelectedValue = current.EmployeeId;
            if (current != null && !current.IsAdmin)
                EmployeesBox.IsEnabled = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;
            try
            {
                if (BooksBox.SelectedValue == null) throw new InvalidOperationException("Выберите книгу.");
                if (ReadersBox.SelectedValue == null) throw new InvalidOperationException("Выберите читателя.");
                if (EmployeesBox.SelectedValue == null) throw new InvalidOperationException("Выберите сотрудника.");

                var date = IssueDatePicker.SelectedDate ?? DateTime.Today;
                Issue = new Issue
                {
                    BookId = Convert.ToInt32(BooksBox.SelectedValue),
                    ReaderId = Convert.ToInt32(ReadersBox.SelectedValue),
                    EmployeeId = Convert.ToInt32(EmployeesBox.SelectedValue),
                    IssueDate = date,
                    Status = "Выдано",
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


