using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Task;

public partial class ChangeTaskWindow : Window
{
    private readonly int _idStudent;
    private List<SubjectViewDto> _studentSubjects;
    private List<MasterWork.Shared.Models.Task> _currentTasks;

    public ChangeTaskWindow(int idStudent)
    {
        InitializeComponent();
        DateTime today = DateTime.Today;

        DeadlineDatePicker.DisplayDateStart = today;
        DeadlineDatePicker.DisplayDateEnd = today.AddYears(1);
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

    private async void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
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
                    TasksListBox.Items.Add($"{task.IdTask}. {task.Task1}");
                }
            }

            TaskTextBox.Text = "";
            DeadlineDatePicker.Text = "";
            
            Visibility visibility = TasksListBox.Items.Count == 0 ? Visibility.Hidden : Visibility.Visible;
            
            if (TasksListBox.Items.Count == 0)
            {
                EarnedScoreTextBox.Visibility = Visibility.Hidden;
                EarnedScoreLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                TasksListBox.SelectedIndex = 0;
            }

            TaskTextBox.Visibility = visibility;
            TaskLabel.Visibility = visibility;
            DeadlineDatePicker.Visibility = visibility;
            DeadlineLabel.Visibility = visibility;
            StatusListBox.Visibility = visibility;
            StatusLabel.Visibility = visibility;
            MaxScoreTextBox.Visibility = visibility;
            MaxScoreLabel.Visibility = visibility;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження завдань: {ex.Message}");
        }
    }

    private void TasksListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TasksListBox.Items.IsEmpty || TasksListBox.SelectedItem == null || _currentTasks == null) return;

        TaskTextBox.Text = "";
        DeadlineDatePicker.Text = "";

        var taskId = int.Parse(TasksListBox.SelectedItem.ToString()!.Split(". ")[0]);
        var selectedTask = _currentTasks.FirstOrDefault(t => t.IdTask == taskId);

        if (selectedTask == null) return;

        TaskTextBox.Text = selectedTask.Task1;
        
        // Виправлено роботу з DateOnly?
        if (selectedTask.Deadline.HasValue)
        {
            DeadlineDatePicker.Text = selectedTask.Deadline.Value.ToString("dd.MM.yyyy");
        }
        
        // Виправлено роботу з decimal
        MaxScoreTextBox.Text = selectedTask.MaxScore.ToString().Replace(',', '.');
        
        StatusListBox.SelectedIndex = -1;
        StatusListBox.SelectedIndex = selectedTask.IdStatus - 1; 
    }

    private void StatusListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EarnedScoreTextBox.Visibility = StatusListBox.SelectedIndex == 4 ? Visibility.Visible : Visibility.Hidden;
        EarnedScoreLabel.Visibility = StatusListBox.SelectedIndex == 4 ? Visibility.Visible : Visibility.Hidden;
        
        if (StatusListBox.SelectedIndex != 4) return;
        
        if (TasksListBox.SelectedItem == null || _currentTasks == null) return;
        
        var taskId = int.Parse(TasksListBox.SelectedItem.ToString()!.Split(". ")[0]);
        var selectedTask = _currentTasks.FirstOrDefault(t => t.IdTask == taskId);
        
        if (selectedTask != null)
        {
            if (selectedTask.EarnedScore == null && StatusListBox.SelectedIndex == 4)
            {
                EarnedScoreTextBox.Text = "0.00";
            }
            else
            {
                EarnedScoreTextBox.Text = selectedTask.EarnedScore?.ToString().Replace(',', '.');
            }
        }
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = new Regex("[^0-9.]+").IsMatch(e.Text);
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }

    private async void ChangeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (TasksListBox.SelectedItem == null || _currentTasks == null) return;
        
        var taskId = int.Parse(TasksListBox.SelectedItem.ToString()!.Split(". ")[0]);
        var selectedTask = _currentTasks.FirstOrDefault(t => t.IdTask == taskId);
        
        if (selectedTask == null) return;

        if (string.IsNullOrEmpty(TaskTextBox.Text) || string.IsNullOrEmpty(MaxScoreTextBox.Text))
        {
            MessageBox.Show("Заповніть усі необхідні поля", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (StatusListBox.SelectedIndex == 4 && string.IsNullOrEmpty(EarnedScoreTextBox.Text))
        {
            MessageBox.Show("Введіть отриману оцінку", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!CheckNumber(MaxScoreTextBox.Text.Replace('.', ',')) || 
           (StatusListBox.SelectedIndex == 4 && !CheckNumber(EarnedScoreTextBox.Text.Replace('.', ','))))
        {
            MessageBox.Show("Введіть коректні значення", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Виправлено конвертацію рядка у DateOnly?
        DateOnly? parsedDeadline = null;
        if (!string.IsNullOrEmpty(DeadlineDatePicker.Text))
        {
            if (DateTime.TryParse(DeadlineDatePicker.Text, out DateTime dt))
            {
                parsedDeadline = DateOnly.FromDateTime(dt);
            }
        }

        selectedTask.Task1 = TaskTextBox.Text;
        selectedTask.Deadline = parsedDeadline;
        selectedTask.MaxScore = decimal.Parse(MaxScoreTextBox.Text.Replace('.', ','));
        selectedTask.IdStatus = StatusListBox.SelectedIndex + 1;
        
        if (StatusListBox.SelectedIndex == 4)
        {
            selectedTask.EarnedScore = decimal.Parse(EarnedScoreTextBox.Text.Replace('.', ','));
        }
        else
        {
            selectedTask.EarnedScore = null;
        }

        try
        {
            var response = await ApiClient.Client.PutAsJsonAsync($"Tasks/{taskId}", selectedTask);
            response.EnsureSuccessStatusCode();

            MessageBox.Show("Завдання оновлено", "", MessageBoxButton.OK, MessageBoxImage.Information);
            var mainWindow = new MainWindow(_idStudent);
            mainWindow.Show();
            Hide();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static bool CheckNumber(string input)
    {
        if (double.TryParse(input, out var number))
        {
            return number >= 0;
        }
        return false;
    }

    private void ChangeTaskWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}