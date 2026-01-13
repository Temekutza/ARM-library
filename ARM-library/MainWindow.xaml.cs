using System;
using System.Windows;
using ARM_library.Data;
using ARM_library.Security;

namespace ARM_library
{
    public partial class MainWindow : Window
    {
        private readonly DataAccess _db;
        private readonly BookService _books;
        private readonly ReaderService _readers;
        private readonly EmployeeService _employees;
        private readonly IssueService _issues;

        public MainWindow()
        {
            InitializeComponent();

            _db = new DataAccess();
            _books = new BookService(_db);
            _readers = new ReaderService(_db);
            _employees = new EmployeeService(_db);
            _issues = new IssueService(_db);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var user = UserContext.Current;
            if (user == null)
            {
                // Если зашли напрямую без логина
                new LoginWindow().Show();
                Close();
                return;
            }

            UserInfoText.Text = $"{user.EmployeeName} — {user.Role} ({user.Username})";

            // Ролевой доступ: Librarian не видит вкладку "Сотрудники"
            if (!user.IsAdmin)
                EmployeesTab.Visibility = Visibility.Collapsed;

            RefreshAll();
        }

        private void RefreshAll()
        {
            BooksGrid.ItemsSource = _books.GetAll().DefaultView;
            ReadersGrid.ItemsSource = _readers.GetAll().DefaultView;
            IssuesGrid.ItemsSource = _issues.GetAll().DefaultView;

            var user = UserContext.Current;
            if (user != null && user.IsAdmin)
                EmployeesGrid.ItemsSource = _employees.GetAll().DefaultView;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            UserContext.Current = null;
            new LoginWindow().Show();
            Close();
        }

        private void Books_Refresh_Click(object sender, RoutedEventArgs e)
        {
            BooksGrid.ItemsSource = _books.GetAll().DefaultView;
        }

        private void Readers_Refresh_Click(object sender, RoutedEventArgs e)
        {
            ReadersGrid.ItemsSource = _readers.GetAll().DefaultView;
        }

        private void Issues_Refresh_Click(object sender, RoutedEventArgs e)
        {
            IssuesGrid.ItemsSource = _issues.GetAll().DefaultView;
        }

        private void Employees_Refresh_Click(object sender, RoutedEventArgs e)
        {
            EmployeesGrid.ItemsSource = _employees.GetAll().DefaultView;
        }

        private void Issues_Issue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new IssueWindow(_db);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _issues.IssueBook(w.Issue);
                    IssuesGrid.ItemsSource = _issues.GetAll().DefaultView;
                    BooksGrid.ItemsSource = _books.GetAll().DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка выдачи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Return_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = IssuesGrid.SelectedItem as System.Data.DataRowView;
                if (row == null) return;

                var status = Convert.ToString(row["Status"]) ?? string.Empty;
                if (string.Equals(status, "Возвращено", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this, "Эта выдача уже возвращена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var issueId = Convert.ToInt32(row["IssueID"]);
                _issues.ReturnIssue(issueId, DateTime.Today);
                IssuesGrid.ItemsSource = _issues.GetAll().DefaultView;
                BooksGrid.ItemsSource = _books.GetAll().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка возврата", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


