using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Windows;
using Newtonsoft.Json;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Subject;

public partial class AddSubjectWindow : Window
{
    private readonly int _idStudent;
    private Dictionary<string, string> _teacherBook = new();
    private string GroupName;

    public AddSubjectWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        _ = InitializeDataAsync();
    }

    private async System.Threading.Tasks.Task InitializeDataAsync()
    {
        try
        {
            // Отримуємо групу студента через API
            GroupName = await ApiClient.Client.GetStringAsync($"Subjects/Student/{_idStudent}/GroupName");
            await LoadGroupScheduleAsync(GroupName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження даних: {ex.Message}");
        }
    }

    private async System.Threading.Tasks.Task LoadGroupScheduleAsync(string groupName)
    {
        try
        {
            string groupId = await GetGroupIdByName(groupName);
            if (string.IsNullOrEmpty(groupId))
            {
                MessageBox.Show("Не вдалося знайти групу.");
                return;
            }

            string scheduleUrl = $"https://api.campus.kpi.ua/schedule/lessons?groupId={groupId}";
            var response = await ApiClient.Client.GetAsync(scheduleUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var scheduleResponse = JsonConvert.DeserializeObject<RozkladData>(json);

            if (scheduleResponse?.ScheduleFirstWeek != null)
            {
                var allLessons = scheduleResponse.ScheduleFirstWeek
                    .Where(d => d.Pairs != null)
                    .SelectMany(d => d.Pairs)
                    .ToList();

                _teacherBook.Clear();

                var teachersWithIds = allLessons
                    .Where(p => !string.IsNullOrWhiteSpace(p.LecturerId) && !string.IsNullOrWhiteSpace(p.TeacherName))
                    .GroupBy(p => p.LecturerId)
                    .Select(g => new { Id = g.Key, Name = g.First().TeacherName.Replace('\'', '`') }) 
                    .ToList();

                foreach (var teacher in teachersWithIds)
                {
                    if (!_teacherBook.ContainsKey(teacher.Id))
                        _teacherBook.Add(teacher.Id, teacher.Name);
                }

                TeacherComboBox.ItemsSource = _teacherBook.Select(t => $"{t.Value}").ToList();

                var uniqueSubjects = allLessons
                    .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                    .Select(p => p.Name.Replace('\'', '`')) 
                    .Distinct()
                    .ToList();

                SubjectComboBox.ItemsSource = uniqueSubjects;
            }
            else
            {
                MessageBox.Show("Розклад відсутній або неправильний формат.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка при завантаженні розкладу: {ex.Message}");
        }
    }

    private async System.Threading.Tasks.Task<string> GetGroupIdByName(string groupName)
    {
        string encodedName = Uri.EscapeDataString(groupName);
        string url = $"https://api.campus.kpi.ua/group/find?name={encodedName}";

        var response = await ApiClient.Client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var groups = JsonConvert.DeserializeObject<List<GroupInfo>>(json);

        return groups?.FirstOrDefault()?.Id;
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }

    private async void Add_OnClick(object sender, RoutedEventArgs e)
    {
        if (TeacherComboBox.SelectedIndex == -1 || SubjectComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("Оберіть викладача та предмет", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string selectedFullName = TeacherComboBox.SelectedItem.ToString()!;
        string teacherGuid = _teacherBook.FirstOrDefault(x => x.Value == selectedFullName).Key;
        string subjectName = SubjectComboBox.SelectedItem.ToString()!;

        if (string.IsNullOrEmpty(teacherGuid))
        {
            MessageBox.Show("Не вдалося знайти викладача.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            // Формуємо об'єкт для відправки на API
            var enrollmentData = new SubjectEnrollmentDto
            {
                StudentId = _idStudent,
                SubjectName = subjectName,
                Teachers = new Dictionary<string, string> { { teacherGuid, selectedFullName } }
            };

            // Викликаємо метод Enroll у вашому SubjectsController
            var response = await ApiClient.Client.PostAsJsonAsync("Subjects/Enroll", enrollmentData);
            response.EnsureSuccessStatusCode();

            MessageBox.Show("Предмет додано", "", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = new MainWindow(_idStudent);
            mainWindow.Show();
            Hide();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка додавання: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddSubjectWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}

// Допоміжні класи для Campus API залишаються без змін
public class Lecturer { [JsonProperty("id")] public string Id { get; set; } [JsonProperty("name")] public string Name { get; set; } }
public class Lesson { [JsonProperty("teacherName")] public string TeacherName { get; set; } [JsonProperty("lecturerId")] public string LecturerId { get; set; } [JsonProperty("type")] public string Type { get; set; } [JsonProperty("time")] public string Time { get; set; } [JsonProperty("name")] public string Name { get; set; } [JsonProperty("place")] public string Place { get; set; } [JsonProperty("tag")] public string Tag { get; set; } }
public class GroupInfo { [JsonProperty("id")] public string Id { get; set; } [JsonProperty("name")] public string Name { get; set; } }
public class ScheduleResponse { [JsonProperty("schedule")] public List<Lesson> Schedule { get; set; } }
public class RozkladData { [JsonProperty("groupCode")] public string GroupCode { get; set; } [JsonProperty("scheduleFirstWeek")] public List<ScheduleDay> ScheduleFirstWeek { get; set; } [JsonProperty("scheduleSecondWeek")] public List<ScheduleDay> ScheduleSecondWeek { get; set; } }
public class ScheduleDay { [JsonProperty("day")] public string Day { get; set; } [JsonProperty("pairs")] public List<Lesson> Pairs { get; set; } }