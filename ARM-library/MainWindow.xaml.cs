using System;
using System.Data;
using System.Windows;
using ARM_library.Data;
using ARM_library.Models;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace ARM_library
{
    public partial class MainWindow : Window
    {
        private readonly DataAccess _db;
        private readonly BookService _books;
        private readonly ReaderService _readers;
        private readonly EmployeeService _employees;
        private readonly IssueService _issues;

        private DataTable _booksTable;
        private DataTable _readersTable;
        private DataTable _issuesTable;
        private DataTable _employeesTable;

        public MainWindow()
        {
            InitializeComponent();

            _db = new DataAccess();
            _books = new BookService(_db);
            _readers = new ReaderService(_db);
            _employees = new EmployeeService(_db);
            _issues = new IssueService(_db);

            Loaded += (s, e) => RefreshAll();
        }

        private void RefreshAll()
        {
            _booksTable = _books.GetAll();
            _readersTable = _readers.GetAll();
            _issuesTable = _issues.GetAll();
            _employeesTable = _employees.GetAll();

            BooksGrid.ItemsSource = _booksTable.DefaultView;
            ReadersGrid.ItemsSource = _readersTable.DefaultView;
            IssuesGrid.ItemsSource = _issuesTable.DefaultView;
            EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
        }

        #region Книги

        private void Books_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new AddBookWindow();
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _books.Add(w.Book);
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Книга успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка добавления книги", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Books_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = BooksGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите книгу для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var book = new Book
                {
                    Id = Convert.ToInt32(row["BookID"]),
                    Title = Convert.ToString(row["Title"]) ?? string.Empty,
                    Author = Convert.ToString(row["Author"]) ?? string.Empty,
                    Publisher = Convert.ToString(row["Publisher"]) ?? string.Empty,
                    Year = row["Year"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["Year"]),
                    Quantity = Convert.ToInt32(row["Quantity"])
                };

                var w = new AddBookWindow(book);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _books.Update(w.Book);
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Книга успешно изменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка редактирования книги", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Books_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = BooksGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите книгу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var bookId = Convert.ToInt32(row["BookID"]);
                var title = Convert.ToString(row["Title"]);

                var result = MessageBox.Show(this, $"Вы уверены, что хотите удалить книгу \"{title}\"?", 
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _books.Delete(bookId);
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Книга успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка удаления книги", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Books_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _booksTable = _books.GetAll();
                BooksGrid.ItemsSource = _booksTable.DefaultView;
                MessageBox.Show(this, "Данные сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Books_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _booksTable = _books.GetAll();
            BooksGrid.ItemsSource = _booksTable.DefaultView;
            BooksSearchBox.Text = string.Empty;
        }

        private void BooksSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter(_booksTable, BooksSearchBox.Text, "Title", "Author", "Publisher");
        }

        private void Books_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_booksTable, "Книги");
        }

        #endregion

        #region Читатели

        private void Readers_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new AddReaderWindow();
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _readers.Add(w.Reader);
                    _readersTable = _readers.GetAll();
                    ReadersGrid.ItemsSource = _readersTable.DefaultView;
                    MessageBox.Show(this, "Читатель успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка добавления читателя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Readers_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = ReadersGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите читателя для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reader = new Reader
                {
                    Id = Convert.ToInt32(row["ReaderID"]),
                    FullName = Convert.ToString(row["FullName"]) ?? string.Empty,
                    Phone = Convert.ToString(row["Phone"]) ?? string.Empty,
                    Email = Convert.ToString(row["Email"]) ?? string.Empty
                };

                var w = new AddReaderWindow(reader);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _readers.Update(w.Reader);
                    _readersTable = _readers.GetAll();
                    ReadersGrid.ItemsSource = _readersTable.DefaultView;
                    MessageBox.Show(this, "Читатель успешно изменён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка редактирования читателя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Readers_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = ReadersGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите читателя для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var readerId = Convert.ToInt32(row["ReaderID"]);
                var name = Convert.ToString(row["FullName"]);

                var result = MessageBox.Show(this, $"Вы уверены, что хотите удалить читателя \"{name}\"?", 
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _readers.Delete(readerId);
                    _readersTable = _readers.GetAll();
                    ReadersGrid.ItemsSource = _readersTable.DefaultView;
                    MessageBox.Show(this, "Читатель успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка удаления читателя", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Readers_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _readersTable = _readers.GetAll();
                ReadersGrid.ItemsSource = _readersTable.DefaultView;
                MessageBox.Show(this, "Данные сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Readers_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _readersTable = _readers.GetAll();
            ReadersGrid.ItemsSource = _readersTable.DefaultView;
            ReadersSearchBox.Text = string.Empty;
        }

        private void ReadersSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter(_readersTable, ReadersSearchBox.Text, "FullName", "Phone", "Email");
        }

        private void Readers_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_readersTable, "Читатели");
        }

        #endregion

        #region Выдачи

        private void Issues_Issue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new IssueWindow(_db);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _issues.IssueBook(w.Issue);
                    _issuesTable = _issues.GetAll();
                    IssuesGrid.ItemsSource = _issuesTable.DefaultView;
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Книга успешно выдана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка выдачи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = IssuesGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите выдачу для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var issue = new Issue
                {
                    Id = Convert.ToInt32(row["IssueID"]),
                    BookId = Convert.ToInt32(row["BookID"]),
                    ReaderId = Convert.ToInt32(row["ReaderID"]),
                    EmployeeId = Convert.ToInt32(row["EmployeeID"]),
                    IssueDate = Convert.ToDateTime(row["IssueDate"]),
                    ReturnDate = row["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ReturnDate"]),
                    Status = Convert.ToString(row["Status"]) ?? "Выдано"
                };

                var w = new IssueWindow(_db, issue);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    // Обновление выдачи - используем IssueBook для простоты (можно добавить отдельный метод Update)
                    _issues.Delete(issue.Id);
                    _issues.IssueBook(w.Issue);
                    _issuesTable = _issues.GetAll();
                    IssuesGrid.ItemsSource = _issuesTable.DefaultView;
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Выдача успешно изменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка редактирования выдачи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = IssuesGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите выдачу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var issueId = Convert.ToInt32(row["IssueID"]);
                var bookTitle = Convert.ToString(row["BookTitle"]);
                var readerName = Convert.ToString(row["ReaderName"]);

                var result = MessageBox.Show(this, $"Вы уверены, что хотите удалить выдачу книги \"{bookTitle}\" читателю \"{readerName}\"?", 
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _issues.Delete(issueId);
                    _issuesTable = _issues.GetAll();
                    IssuesGrid.ItemsSource = _issuesTable.DefaultView;
                    _booksTable = _books.GetAll();
                    BooksGrid.ItemsSource = _booksTable.DefaultView;
                    MessageBox.Show(this, "Выдача успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка удаления выдачи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _issuesTable = _issues.GetAll();
                IssuesGrid.ItemsSource = _issuesTable.DefaultView;
                MessageBox.Show(this, "Данные сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Return_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = IssuesGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите выдачу для возврата.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var status = Convert.ToString(row["Status"]) ?? string.Empty;
                if (string.Equals(status, "Возвращено", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this, "Эта выдача уже возвращена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var issueId = Convert.ToInt32(row["IssueID"]);
                _issues.ReturnIssue(issueId, DateTime.Today);
                _issuesTable = _issues.GetAll();
                IssuesGrid.ItemsSource = _issuesTable.DefaultView;
                _booksTable = _books.GetAll();
                BooksGrid.ItemsSource = _booksTable.DefaultView;
                MessageBox.Show(this, "Книга успешно возвращена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка возврата", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Issues_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _issuesTable = _issues.GetAll();
            IssuesGrid.ItemsSource = _issuesTable.DefaultView;
            IssuesSearchBox.Text = string.Empty;
        }

        private void IssuesSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter(_issuesTable, IssuesSearchBox.Text, "BookTitle", "ReaderName", "EmployeeName", "Status");
        }

        private void Issues_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_issuesTable, "Выдачи");
        }

        #endregion

        #region Сотрудники

        private void Employees_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new AddEmployeeWindow();
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _employees.Add(w.Employee);
                    _employeesTable = _employees.GetAll();
                    EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
                    MessageBox.Show(this, "Сотрудник успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка добавления сотрудника", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Employees_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = EmployeesGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите сотрудника для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var employee = new Employee
                {
                    Id = Convert.ToInt32(row["EmployeeID"]),
                    FullName = Convert.ToString(row["FullName"]) ?? string.Empty,
                    Position = Convert.ToString(row["Position"]) ?? string.Empty
                };

                var w = new AddEmployeeWindow(employee);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    _employees.Update(w.Employee);
                    _employeesTable = _employees.GetAll();
                    EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
                    MessageBox.Show(this, "Сотрудник успешно изменён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка редактирования сотрудника", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Employees_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = EmployeesGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show(this, "Выберите сотрудника для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var employeeId = Convert.ToInt32(row["EmployeeID"]);
                var name = Convert.ToString(row["FullName"]);

                var result = MessageBox.Show(this, $"Вы уверены, что хотите удалить сотрудника \"{name}\"?", 
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _employees.Delete(employeeId);
                    _employeesTable = _employees.GetAll();
                    EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
                    MessageBox.Show(this, "Сотрудник успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка удаления сотрудника", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Employees_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _employeesTable = _employees.GetAll();
                EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
                MessageBox.Show(this, "Данные сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Employees_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _employeesTable = _employees.GetAll();
            EmployeesGrid.ItemsSource = _employeesTable.DefaultView;
            EmployeesSearchBox.Text = string.Empty;
        }

        private void EmployeesSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter(_employeesTable, EmployeesSearchBox.Text, "FullName", "Position");
        }

        private void Employees_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_employeesTable, "Сотрудники");
        }

        #endregion

        #region Общие методы

        private void ApplyFilter(DataTable table, string searchText, params string[] columns)
        {
            if (table == null) return;

            searchText = (searchText ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                table.DefaultView.RowFilter = string.Empty;
                return;
            }

            var escaped = searchText.Replace("'", "''").Replace("[", "[[]").Replace("*", "[*]").Replace("%", "[%]");
            var conditions = new System.Collections.Generic.List<string>();
            foreach (var col in columns)
            {
                if (table.Columns.Contains(col))
                    conditions.Add($"CONVERT([{col}], 'System.String') LIKE '%{escaped}%'");
            }

            table.DefaultView.RowFilter = conditions.Count > 0 ? string.Join(" OR ", conditions) : string.Empty;
        }

        private void ExportToExcel(DataTable table, string sheetName)
        {
            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show(this, "Нет данных для экспорта.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = $"{sheetName}_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx",
                Title = "Сохранить отчёт"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // Заголовки
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        var cell = worksheet.Cell(1, col + 1);
                        cell.Value = table.Columns[col].ColumnName;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    // Данные (с учётом фильтра)
                    var view = table.DefaultView;
                    int rowIndex = 2;
                    foreach (DataRowView rowView in view)
                    {
                        for (int col = 0; col < table.Columns.Count; col++)
                        {
                            var value = rowView[col];
                            var cell = worksheet.Cell(rowIndex, col + 1);

                            if (value == null || value == DBNull.Value)
                                cell.Value = string.Empty;
                            else if (value is DateTime dt)
                                cell.Value = dt;
                            else if (value is int || value is long || value is short)
                                cell.Value = Convert.ToInt64(value);
                            else if (value is decimal || value is double || value is float)
                                cell.Value = Convert.ToDouble(value);
                            else
                                cell.Value = value.ToString();
                        }
                        rowIndex++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(dialog.FileName);
                }

                MessageBox.Show(this, $"Отчёт сохранён:\n{dialog.FileName}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
