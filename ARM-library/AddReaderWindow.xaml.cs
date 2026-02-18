using System;
using System.Windows;
using ARM_library.Models;

namespace ARM_library
{
    public partial class AddReaderWindow : Window
    {
        public Reader Reader { get; private set; }
        private readonly bool _isEditMode;
        private readonly int _editId;

        public AddReaderWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            FullNameBox.Focus();
        }

        public AddReaderWindow(Reader reader) : this()
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            _isEditMode = true;
            _editId = reader.Id;

            Title = "Редактирование читателя";
            HeaderText.Text = "Редактирование читателя";
            OkButton.Content = "Сохранить";

            FullNameBox.Text = reader.FullName;
            PhoneBox.Text = reader.Phone;
            EmailBox.Text = reader.Email;
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

            Reader = new Reader
            {
                Id = _isEditMode ? _editId : 0,
                FullName = fullName,
                Phone = PhoneBox.Text?.Trim() ?? string.Empty,
                Email = EmailBox.Text?.Trim() ?? string.Empty
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
