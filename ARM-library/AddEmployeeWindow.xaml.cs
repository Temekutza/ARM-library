using System;
using System.Windows;
using ARM_library.Models;

namespace ARM_library
{
    public partial class AddEmployeeWindow : Window
    {
        public Employee Employee { get; private set; }
        private readonly bool _isEditMode;
        private readonly int _editId;

        public AddEmployeeWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            FullNameBox.Focus();
        }

        public AddEmployeeWindow(Employee employee) : this()
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            _isEditMode = true;
            _editId = employee.Id;

            Title = "Редактирование сотрудника";
            HeaderText.Text = "Редактирование сотрудника";
            OkButton.Content = "Сохранить";

            FullNameBox.Text = employee.FullName;
            PositionBox.Text = employee.Position;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;

            var fullName = FullNameBox.Text?.Trim();
            if (string.IsNullOrEmpty(fullName))
            {
                ErrorText.Text = "ФИО обязательно.";
                FullNameBox.Focus();
                return;
            }

            var position = PositionBox.Text?.Trim();
            if (string.IsNullOrEmpty(position))
            {
                ErrorText.Text = "Должность обязательна.";
                PositionBox.Focus();
                return;
            }

            Employee = new Employee
            {
                Id = _isEditMode ? _editId : 0,
                FullName = fullName,
                Position = position
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
