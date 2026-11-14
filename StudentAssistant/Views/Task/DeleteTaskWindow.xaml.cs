using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Task;

public partial class DeleteTaskWindow : Window
{
    private readonly int _idStudent;
    private List<SubjectViewDto> _studentSubjects;
    private List<MasterWork.Shared.Models.Task> _currentTasks;

    public DeleteTaskWindow(int idStudent)
    {
        InitializeComponent();
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

    private async void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TasksListBox.Items.Clear();
        if (SubjectsListBox.SelectedItem == null) return;
        
        var idSubject = int.Parse(SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0]);
        
        try
        {
            var allTasks = await ApiClient.Client.GetFromJsonAsync<List<MasterWork.Shared.Models.Task>>($"Tasks/Student/{_idStudent}");
            
            if (allTasks != null)
            {
                _currentTasks = allTasks.Where(t => t.IdSubject == idSubject).ToList();
                
                foreach (var task in _currentTasks)
                {
                    // Виправлено читання DateOnly?
                    var datePart = "";
                    if (task.Deadline.HasValue)
                    {
                        datePart = " -> " + task.Deadline.Value.ToString("dd.MM.yyyy");
                    }
                    
                    TasksListBox.Items.Add($"{task.IdTask}. {task.Task1}{datePart}");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження завдань: {ex.Message}");
        }
    }
    
    private async void DeleteTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem == null)
        {
            MessageBox.Show("Оберіть завдання для видалення", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show(
                $"Видалити {TasksListBox.SelectedItem}?",
                "",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
                
        var taskId = int.Parse(TasksListBox.SelectedItem.ToString()!.Split(". ")[0]);
        
        try
        {
            var response = await ApiClient.Client.DeleteAsync($"Tasks/{taskId}");
            response.EnsureSuccessStatusCode();
            
            MessageBox.Show("Завдання видалено", "", MessageBoxButton.OK, MessageBoxImage.Information);
            var mainWindow = new MainWindow(_idStudent);
            mainWindow.Show();
            Hide();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void DeleteTaskWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}