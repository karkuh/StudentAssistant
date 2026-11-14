using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Subject;

public partial class DeleteSubjectWindow : Window
{
    private readonly int _idStudent;
    private List<SubjectViewDto> _studentSubjects;

    public DeleteSubjectWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        _ = InitListBoxAsync();
    }

    private async System.Threading.Tasks.Task InitListBoxAsync()
    {
        try
        {
            // Отримуємо предмети студента з API
            _studentSubjects = await ApiClient.Client.GetFromJsonAsync<List<SubjectViewDto>>($"Subjects/Student/{_idStudent}/Views");
            
            if (_studentSubjects != null)
            {
                foreach (var subject in _studentSubjects)
                {
                    SubjectsListBox.Items.Add($"{subject.IdSubject}. {subject.Name} -> {subject.FullName}");
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

    private async void OK_OnClick(object sender, RoutedEventArgs e)
    {
        if (SubjectsListBox.SelectedItem == null) return;

        if (MessageBox.Show(
                $"Видалити {SubjectsListBox.SelectedItem}? Разом з предметом видаляться усі завдання.",
                "",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        
        var idString = SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0];
        
        try
        {
            // Видаляємо предмет через API
            var response = await ApiClient.Client.DeleteAsync($"Subjects/Student/{_idStudent}/Subject/{idString}");
            response.EnsureSuccessStatusCode();

            var start = new MainWindow(_idStudent);
            Hide();
            start.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка видалення: {ex.Message}");
        }
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_idStudent);
        Hide();
        main.Show();
    }

    private void DeleteSubject_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}