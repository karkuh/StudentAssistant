using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StudentAssistant.DB;

namespace StudentAssistant.Windows.User;

public partial class RegistrationWindow
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
    private void Registration_Click(object sender, RoutedEventArgs e)
    {
        if (!Validation(LoginTextBox.Text, PasswordTextBox.Text, GroupComboBox.Text)) return;
        if (Check_Login(LoginTextBox.Text))
        {
            // Додавання нового облікового запису студента
            var insertQuery =
                $"INSERT INTO students (login, password, group_name) VALUES " +
                $"('{LoginTextBox.Text.Trim()}', '{PasswordTextBox.Text}', '{GroupComboBox.Text}');";

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
    
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.campus.kpi.ua/schedule/groups";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var groupList = JsonConvert.DeserializeObject<List<Group>>(responseBody);
                groups = groupList;
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "Name";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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