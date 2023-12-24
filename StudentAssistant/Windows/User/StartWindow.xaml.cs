using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;

namespace StudentAssistant.Windows.User;

public partial class StartWindow
{
    public StartWindow()
    {
        InitializeComponent();
    }

    private void LogIn_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Text))
        {
            MessageBox.Show("Заповніть усі поля", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var passwordQuery = $"select password from students where login = '{LoginTextBox.Text}'";
        var commandPassword = new MySqlCommand(passwordQuery, Db.Connection.Connect);
        var readerPassword = commandPassword.ExecuteReader();
        string password;
        if (readerPassword.HasRows)
        {
            readerPassword.Read();
            password = readerPassword.GetString(0);
            readerPassword.Close();

        }
        else
        {
            MessageBox.Show("Невірний логін або пароль. Спробуйте ще раз.", "", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            LoginTextBox.Text = "";
            PasswordTextBox.Text = "";
            readerPassword.Close();
            return;  
        }


        if (password == PasswordTextBox.Text)
        {
            var idStudentQuery = $"select id_student from students where login = '{LoginTextBox.Text}'";
            var commandIdStudent = new MySqlCommand(idStudentQuery, Db.Connection.Connect);
            var idStudent = Convert.ToInt32(commandIdStudent.ExecuteScalar());
            var mainWindow = new MainWindow(idStudent);
            mainWindow.Show();
            Hide();
        }
        else
        {
            MessageBox.Show("Невірний логін або пароль. Спробуйте ще раз.", "", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            LoginTextBox.Text = "";
            PasswordTextBox.Text = "";
        }


    }

    private void SignUp_Click(object sender, RoutedEventArgs e)
    {
        var registrationWindow = new RegistrationWindow();
        registrationWindow.Show();
        Hide();
    }

    public static string TransformDateFormat(string inputDate)
    {
        try
        {
            var dateComponents = inputDate.Split('.');
            var day = int.Parse(dateComponents[0]);
            var month = int.Parse(dateComponents[1]);
            var year = int.Parse(dateComponents[2]);

            var formattedDate = $"{year:D4}-{month:D2}-{day:D2}";
            return formattedDate;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
            return string.Empty;
        }
    }
    
    private void StartForm_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}