using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Json;
using System.Windows;
using StudentAssistant.Services;
using MasterWork.Shared.Models;

namespace StudentAssistant.Views.User;

public partial class StartWindow : Window
{
    private const string SessionFile = "session.txt";

    public StartWindow()
    {
        InitializeComponent();
        CheckSavedSession();
    }

    private void CheckSavedSession()
    {
        if (File.Exists(SessionFile))
        {
            if (int.TryParse(File.ReadAllText(SessionFile), out int idStudent))
            {
                var mainWindow = new MainWindow(idStudent);
                mainWindow.Show();
                Hide();
            }
        }
    }

    private async void LogIn_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Text))
        {
            MessageBox.Show("Заповніть усі поля", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var loginDto = new LoginDto
        {
            Login = LoginTextBox.Text,
            Password = PasswordTextBox.Text
        };

        try
        {
            var response = await ApiClient.Client.PostAsJsonAsync("Users/Login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Оскільки контролер повертає Ok(student.IdStudent), це буде просто рядок з числом
                if (int.TryParse(jsonResponse, out int studentId))
                {
                    if (SavedStudentCheckBox.IsChecked == true)
                    {
                        File.WriteAllText(SessionFile, studentId.ToString());
                    }

                    var mainWindow = new MainWindow(studentId);
                    mainWindow.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Помилка обробки відповіді від сервера.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль. Спробуйте ще раз.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoginTextBox.Text = "";
                PasswordTextBox.Text = "";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка з'єднання: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            return $"{year:D4}-{month:D2}-{day:D2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
            return string.Empty;
        }
    }
    
    private void StartForm_OnClosing(object? sender, CancelEventArgs e)
    {
        Hide();
        Close();
    }
}