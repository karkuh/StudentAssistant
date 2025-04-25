using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Windows;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Subject;

public partial class AddSubjectWindow
{
    private readonly int _idStudent;
    private readonly HttpClient _httpClient = new();
    private Dictionary<string, string> _teacherBook = new();

    private  string GroupName;

    public AddSubjectWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        string query =
            $"SELECT group_name FROM students WHERE id_student = '{idStudent}'";
        var command = new MySqlCommand(query, Db.Connection.Connect);
        object? result = command.ExecuteScalar();
        GroupName = result.ToString();
        _ = LoadGroupScheduleAsync(GroupName);
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
            var response = await _httpClient.GetAsync(scheduleUrl);
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
                    .Select(g => new { Id = g.Key, Name = g.First().TeacherName.Replace('\'', '`') }) // заміна '
                    .ToList();

                foreach (var teacher in teachersWithIds)
                {
                    if (!_teacherBook.ContainsKey(teacher.Id))
                        _teacherBook.Add(teacher.Id, teacher.Name);
                }

                TeacherComboBox.ItemsSource = _teacherBook.Select(t => $"{t.Value}").ToList();

                var uniqueSubjects = allLessons
                    .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                    .Select(p => p.Name.Replace('\'', '`')) // заміна '
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

        var response = await _httpClient.GetAsync(url);
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

    private void Add_OnClick(object sender, RoutedEventArgs e)
    {
        if (TeacherComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("Оберіть викладача", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SubjectComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("Оберіть викладача", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }


        int idTeacher;

// Отримуємо ПІБ викладача з ComboBox
        string selectedFullName = TeacherComboBox.SelectedItem.ToString()!;

// Знаходимо GUID (ключ) за ПІБ у словнику
        string teacherGuid = _teacherBook.FirstOrDefault(x => x.Value == selectedFullName).Key;

        if (string.IsNullOrEmpty(teacherGuid))
        {
            MessageBox.Show("Не вдалося знайти викладача у словнику.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

// Перевіряємо, чи існує викладач з таким id_teacher у таблиці teachers
        string checkQuery =
            $"SELECT id_teacher FROM teachers WHERE teacher_code = '{teacherGuid}'";
        var checkCmd = new MySqlCommand(checkQuery, Db.Connection.Connect);
        object? result = checkCmd.ExecuteScalar();

        if (result != null)
        {
            // Якщо викладач знайдений, отримуємо його id
            idTeacher = Convert.ToInt32(result);
        }
        else
        {
            // Якщо викладача немає, додаємо його в таблицю
            string insertTeacherQuery =
                $"INSERT INTO teachers (last_name, first_name, middle_name,teacher_code) " +
                $"VALUES ('{selectedFullName.Split(' ')[0]}', '{selectedFullName.Split(' ')[1]}', '{selectedFullName.Split(' ')[2]}','{teacherGuid}'); SELECT LAST_INSERT_ID();";
            var insertCmd = new MySqlCommand(insertTeacherQuery, Db.Connection.Connect);
            idTeacher = Convert.ToInt32(insertCmd.ExecuteScalar());
        }


        // Додавання предмету
        string subjectName = SubjectComboBox.SelectedItem.ToString()!;

        string subjectCheckQuery =
            $"SELECT id_subject FROM subjects WHERE name = '{subjectName}' AND id_teacher = {idTeacher}";
        var subjectCheckCmd = new MySqlCommand(subjectCheckQuery, Db.Connection.Connect);
        object? subjectResult = subjectCheckCmd.ExecuteScalar();

        int idSubject;
        if (subjectResult != null)
        {
            idSubject = Convert.ToInt32(subjectResult);
        }
        else
        {
            string insertSubjectQuery =
                $"INSERT INTO subjects (name, id_teacher) VALUES ('{subjectName}', {idTeacher}); SELECT LAST_INSERT_ID();";
            var insertSubjectCmd = new MySqlCommand(insertSubjectQuery, Db.Connection.Connect);
            idSubject = Convert.ToInt32(insertSubjectCmd.ExecuteScalar());
        }

        // Додавання залежності студент-предмет
        string linkCheckQuery =
            $"SELECT 1 FROM subjects_for_one_student WHERE id_student = {_idStudent} AND id_subject = {idSubject}";
        var linkCheckCmd = new MySqlCommand(linkCheckQuery, Db.Connection.Connect);
        object? linkExists = linkCheckCmd.ExecuteScalar();

        if (linkExists == null)
        {
            string insertLinkQuery =
                $"INSERT INTO subjects_for_one_student (id_student, id_subject) VALUES ({_idStudent}, {idSubject});";
            var insertLinkCmd = new MySqlCommand(insertLinkQuery, Db.Connection.Connect);
            insertLinkCmd.ExecuteNonQuery();
        }

        MessageBox.Show("Предмет додано", "", MessageBoxButton.OK, MessageBoxImage.Information);

        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();

    }

    private void AddSubjectWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}

public class Lecturer
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
}

public class Lesson
{
    [JsonProperty("teacherName")] public string TeacherName { get; set; }
    [JsonProperty("lecturerId")] public string LecturerId { get; set; }
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("time")] public string Time { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("place")] public string Place { get; set; }
    [JsonProperty("tag")] public string Tag { get; set; }
}

public class GroupInfo
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
}

public class ScheduleResponse
{
    [JsonProperty("schedule")] public List<Lesson> Schedule { get; set; }
}

public class RozkladData
{
    [JsonProperty("groupCode")] public string GroupCode { get; set; }
    [JsonProperty("scheduleFirstWeek")] public List<ScheduleDay> ScheduleFirstWeek { get; set; }
    [JsonProperty("scheduleSecondWeek")] public List<ScheduleDay> ScheduleSecondWeek { get; set; }
}

public class ScheduleDay
{
    [JsonProperty("day")] public string Day { get; set; }
    [JsonProperty("pairs")] public List<Lesson> Pairs { get; set; }
}

    
    
