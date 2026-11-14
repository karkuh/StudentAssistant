using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Task;

public partial class AddTaskWindow : Window
{
    private readonly int _idStudent;
    private List<SubjectViewDto> _studentSubjects;

    public AddTaskWindow(int idStudent)
    {
        InitializeComponent();
        DateTime today = DateTime.Today;

        DeadLineDatePicker.DisplayDateStart = today;
        DeadLineDatePicker.DisplayDateEnd = today.AddYears(1);
        _idStudent = idStudent;
        _ = InitListBoxAsync();
    }

    private async System.Threading.Tasks.Task InitListBoxAsync()
    {
        try
        {
            _studentSubjects = await ApiClient.Client.GetFromJsonAsync<List<SubjectViewDto>>($"Subjects/Student/{_idStudent}/Views");
            
            if (_studentSubjects != null)
            {
                foreach (var subject in _studentSubjects)
                {
                    SubjectsListBox.Items.Add($"{subject.IdSubject}. {subject.Name} {subject.FullName}");
                }
            }

            if (SubjectsListBox.Items.Count > 0)
                SubjectsListBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження предметів: {ex.Message}");
        }
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }

    private async void Add_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(TaskTextBox.Text))
        {
            MessageBox.Show("Введіть завдання", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SubjectsListBox.SelectedItem == null)
        {
            MessageBox.Show("Оберіть предмет", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var idSubject = int.Parse(SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0]);
        
        // Виправлено створення DateOnly?
        DateOnly? parsedDeadline = null;
        if (!string.IsNullOrEmpty(DeadLineDatePicker.Text))
        {
            if (DateTime.TryParse(DeadLineDatePicker.Text, out DateTime dt))
            {
                parsedDeadline = DateOnly.FromDateTime(dt);
            }
        }

        var newTask = new MasterWork.Shared.Models.Task
        {
            IdStudent = _idStudent,
            IdSubject = idSubject,
            Task1 = TaskTextBox.Text,
            Deadline = parsedDeadline,
            MaxScore = (decimal)MaxScoreSlider.Value,
            IdStatus = 1 // 1 - Нове завдання (0%)
        };

        try
        {
            var response = await ApiClient.Client.PostAsJsonAsync("Tasks", newTask);
            response.EnsureSuccessStatusCode();

            MessageBox.Show("Завдання додано", "", MessageBoxButton.OK, MessageBoxImage.Information);
            var mainWindow = new MainWindow(_idStudent);
            mainWindow.Show();
            Hide();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void AddTaskWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}