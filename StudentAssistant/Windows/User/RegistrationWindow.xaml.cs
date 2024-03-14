using System;
using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;

namespace StudentAssistant.Windows.User;

public partial class RegistrationWindow
{
    public RegistrationWindow()
    {
        InitializeComponent();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        var startWindow = new StartWindow();
        startWindow.Show();
        Hide();
    }
    private void Registration_Click(object sender, RoutedEventArgs e)
    {
        if (!Validation(LoginTextBox.Text, PasswordTextBox.Text, GroupTextBox.Text)) return;
        if (Check_Login(LoginTextBox.Text))
        {
            // Додавання нового облікового запису студента
            var insertQuery =
                $"INSERT INTO students (login, password, group_name) VALUES " +
                $"('{LoginTextBox.Text.Trim()}', '{PasswordTextBox.Text}', '{GroupTextBox.Text}');";

            var commandInsert = new MySqlCommand(insertQuery, Db.Connection.Connect);
            commandInsert.ExecuteScalar();
            MessageBox.Show("Користувача додано", "", MessageBoxButton.OK, MessageBoxImage.Information);
            var startWindow = new StartWindow();
            startWindow.Show();
            Hide();
        }
        else
        {
            MessageBox.Show("Такий логін вже існує", "", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static bool Validation(string login, string password, string group)
    {
        if (string.IsNullOrEmpty(login.Trim()) || string.IsNullOrEmpty(password.Trim()) ||
            string.IsNullOrEmpty(group.Trim()))
        {
            MessageBox.Show("Заповніть усі поля", "Увага!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (password.Trim().Length < 8 || login.Trim().Length < 8)
        {
            MessageBox.Show("Пароль та логін повинні містити мінімум 8 символів", "Увага!", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }

        if (password.Trim().Contains(' ') || login.Trim().Contains(' ') || group.Trim().Contains(' '))
        {
            MessageBox.Show("Введені дані не повинні містити пробіл", "Увага!", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private static bool Check_Login(string login)
    {
        // Перевірка унікальності логіну
        var countQuery = $"select count(*) from students where login = '{login.Trim()}';";

        var commandCount = new MySqlCommand(countQuery, Db.Connection.Connect);
        var count = Convert.ToInt32(commandCount.ExecuteScalar());
        return count == 0;
    }
    
    private void Registration_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}