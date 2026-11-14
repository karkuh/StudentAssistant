using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Newtonsoft.Json;
using StudentAssistant.Services;
using MasterWork.Shared.Models;

namespace StudentAssistant.Views.User;

public partial class RegistrationWindow : Window
{
    private List<Group> groups;

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

    private async void Registration_Click(object sender, RoutedEventArgs e)
    {
        if (!Validation(LoginTextBox.Text, PasswordTextBox.Text, GroupComboBox.Text)) return;

        var registerDto = new RegisterDto
        {
            Login = LoginTextBox.Text.Trim(),
            Password = PasswordTextBox.Text, // Відправляємо як є, API його надійно захешує
            GroupName = GroupComboBox.Text
        };

        try
        {
            var response = await ApiClient.Client.PostAsJsonAsync("Users/Register", registerDto);
            
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Користувача успішно додано!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                var startWindow = new StartWindow();
                startWindow.Show();
                Hide();
            }
            else
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Статус коду: {response.StatusCode}\nДеталі:\n{errorDetails}", "Відповідь сервера", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show("Пароль та логін повинні містити мінімум 8 символів", "Увага!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (password.Trim().Contains(' ') || login.Trim().Contains(' ') || group.Trim().Contains(' '))
        {
            MessageBox.Show("Введені дані не повинні містити пробіл", "Увага!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
    
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
    
                string url = "https://api.campus.kpi.ua/schedule/groups";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
    
                groups = JsonConvert.DeserializeObject<List<Group>>(responseBody);
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "Name";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }
    
    private void Registration_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
    
    public class Group
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("faculty")] public string Faculty { get; set; }
    }
}