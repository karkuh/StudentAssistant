using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using StudentAssistant.Services;
using MainWindow = StudentAssistant.Views.User.MainWindow;

namespace StudentAssistant.Views.Subject;

public partial class RozkladWindow : Window
{
    private readonly int _idStudent;
    private List<Group> groups;

    public RozkladWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        Loaded += MainWindow_Loaded;
    }

    public class Schedule
    {
        public string TimeSlot { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
    }

    private async void LoadScheduleData(string groupId)
    {
        try
        {
            string url = $"https://api.campus.kpi.ua/schedule/lessons?groupId={groupId}";
            var response = await ApiClient.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var scheduleData = JsonConvert.DeserializeObject<ScheduleData>(json);

            var scheduleListFirstWeek = GenerateScheduleList(scheduleData.ScheduleFirstWeek);
            var scheduleListSecondWeek = GenerateScheduleList(scheduleData.ScheduleSecondWeek);

            dataGridFirstWeek.ItemsSource = scheduleListFirstWeek;
            dataGridSecondWeek.ItemsSource = scheduleListSecondWeek;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private List<Schedule> GenerateScheduleList(List<ScheduleDay> scheduleDays)
    {
        var scheduleList = new List<Schedule>();
        var pairs = new List<string> { "1 пара\n8:30", "2 пара\n10:25", "3 пара\n12:20", "4 пара\n14:15", "5 пара\n16:10", "6 пара\n18:30" };

        foreach (var pair in pairs)
        {
            var schedule = new Schedule { TimeSlot = pair };

            foreach (var day in scheduleDays)
            {
                foreach (var lesson in day.Pairs)
                {
                    if (lesson.Time == pair.Split("пара\n")[1])
                    {
                        switch (day.Day)
                        {
                            case "Пн": schedule.Monday = FormatLesson(lesson); break;
                            case "Вв": schedule.Tuesday = FormatLesson(lesson); break;
                            case "Ср": schedule.Wednesday = FormatLesson(lesson); break;
                            case "Чт": schedule.Thursday = FormatLesson(lesson); break;
                            case "Пт": schedule.Friday = FormatLesson(lesson); break;
                            case "Сб": schedule.Saturday = FormatLesson(lesson); break;
                        }
                    }
                }
            }
            scheduleList.Add(schedule);
        }
        return scheduleList;
    }

    private string FormatLesson(Lesson lesson)
    {
        return $"\u250c―――――――――――――――――\u2510\n" +
               $"{InsertNewLineEveryNCharacters(lesson.Name,35)}\n" +
               $"\u2514―――――――――――――――――\u2518\n" +
               $"   🎓 {lesson.TeacherName}\n"+
               $"   📝 {lesson.Type}";
    }
    
    private string InsertNewLineEveryNCharacters(string input, int charactersPerLine)
    {
        var result = new StringBuilder();
        var words = input.Split(' ');
        var currentLine = new StringBuilder("   ");
        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > charactersPerLine)
            {
                result.AppendLine(currentLine.ToString().TrimEnd());
                currentLine.Clear();
                currentLine.Append("   "); 
            }
            currentLine.Append(word + " ");
        }

        if (currentLine.Length > 0)
        {
            result.Append(currentLine.ToString().TrimEnd());
        }
        return result.ToString();
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            string url = "https://api.campus.kpi.ua/schedule/groups";
            HttpResponseMessage response = await ApiClient.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var groupList = JsonConvert.DeserializeObject<List<Group>>(responseBody);
            groups = groupList;
            comboBox.ItemsSource = groups;
            comboBox.DisplayMemberPath = "Name";
            
            // Отримуємо групу студента через API замість MySqlCommand
            string studentGroup = await ApiClient.Client.GetStringAsync($"Subjects/Student/{_idStudent}/GroupName");
            comboBox.Text = studentGroup;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (comboBox.SelectedItem is Group selectedGroup)
        {
            LoadScheduleData(selectedGroup.Id);
        }
    }

    public class Group { [JsonProperty("id")] public string Id { get; set; } [JsonProperty("name")] public string Name { get; set; } [JsonProperty("faculty")] public string Faculty { get; set; } }
    public class ScheduleData { [JsonProperty("groupCode")] public string GroupCode { get; set; } [JsonProperty("scheduleFirstWeek")] public List<ScheduleDay> ScheduleFirstWeek { get; set; } [JsonProperty("scheduleSecondWeek")] public List<ScheduleDay> ScheduleSecondWeek { get; set; } }
    public class ScheduleDay { [JsonProperty("day")] public string Day { get; set; } [JsonProperty("pairs")] public List<Lesson> Pairs { get; set; } }
    public class Lesson { [JsonProperty("teacherName")] public string TeacherName { get; set; } [JsonProperty("lecturerId")] public string LecturerId { get; set; } [JsonProperty("type")] public string Type { get; set; } [JsonProperty("time")] public string Time { get; set; } [JsonProperty("name")] public string Name { get; set; } [JsonProperty("place")] public string Place { get; set; } [JsonProperty("tag")] public string Tag { get; set; } }

    private void RozkladWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_idStudent);
        Hide();
        main.Show();
    }
}