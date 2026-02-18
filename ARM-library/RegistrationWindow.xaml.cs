using System;
using System.Windows;
using ARM_library.Data;
using ARM_library.Security;

namespace ARM_library
{
    public partial class RegistrationWindow : Window
    {
        private readonly DataAccess _db;
        private readonly EmployeeService _employees;
        private readonly UserService _users;

        public string RegisteredUsername { get; private set; }

        public RegistrationWindow()
        {
            InitializeComponent();

            _db = new DataAccess();
            _employees = new EmployeeService(_db);
            _users = new UserService(_db);

            EmployeesBox.ItemsSource = _employees.GetLookup().DefaultView;
            EmployeesBox.DisplayMemberPath = "DisplayName";
            EmployeesBox.SelectedValuePath = "EmployeeID";
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmployeesBox.SelectedValue == null)
                    throw new InvalidOperationException("Выберите сотрудника.");

                var employeeId = Convert.ToInt32(EmployeesBox.SelectedValue);
                var username = (UsernameBox.Text ?? string.Empty).Trim();
                var pass1 = PasswordBox.Password ?? string.Empty;
                var pass2 = Password2Box.Password ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                    throw new InvalidOperationException("Логин не должен быть пустым.");
                if (pass1.Length < 4)
                    throw new InvalidOperationException("Пароль слишком короткий (минимум 4 символа).");
                if (!string.Equals(pass1, pass2, StringComparison.Ordinal))
                    throw new InvalidOperationException("Пароли не совпадают.");

                if (_users.UsernameExists(username))
                    throw new InvalidOperationException("Такой логин уже существует.");

                if (_users.EmployeeHasUser(employeeId))
                    throw new InvalidOperationException("Для этого сотрудника уже создан пользователь.");

                var hash = PasswordHasher.Sha256Hex(pass1);
                _users.CreateUser(employeeId, username, hash, role: "Librarian");

                RegisteredUsername = username;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Регистрация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


