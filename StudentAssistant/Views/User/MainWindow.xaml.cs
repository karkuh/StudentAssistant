using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using StudentAssistant.Services;
using MasterWork.Shared.Models;
using AddSubjectWindow = StudentAssistant.Views.Subject.AddSubjectWindow;
using AddTaskWindow = StudentAssistant.Views.Task.AddTaskWindow;
using ChangeTaskWindow = StudentAssistant.Views.Task.ChangeTaskWindow;
using DeleteSubjectWindow = StudentAssistant.Views.Subject.DeleteSubjectWindow;
using DeleteTaskWindow = StudentAssistant.Views.Task.DeleteTaskWindow;
using RozkladWindow = StudentAssistant.Views.Subject.RozkladWindow;

namespace StudentAssistant.Views.User;

public partial class MainWindow : Window
{
    private readonly int _idStudent;
    private readonly Dictionary<string, int> _status;

    public MainWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        _status = new Dictionary<string, int>();
        
        // Запускаємо асинхронне завантаження даних для DataGrid
        _ = Init_taskAsync();
    }

    private async System.Threading.Tasks.Task Init_taskAsync()
    {
        try
        {
            // 1. Отримуємо список предметів студента
            var subjects = await ApiClient.Client.GetFromJsonAsync<List<SubjectViewDto>>($"Subjects/Student/{_idStudent}/Views");
            if (subjects == null || !subjects.Any()) return;

            // 2. Отримуємо всі завдання студента
            var allTasks = await ApiClient.Client.GetFromJsonAsync<List<MasterWork.Shared.Models.Task>>($"Tasks/Student/{_idStudent}");
            if (allTasks == null) allTasks = new List<MasterWork.Shared.Models.Task>();

            var dataTable = new DataTable();
            dataTable.Columns.Add("№");

            // Створюємо заголовки колонок (Предмети та Викладачі)
            foreach (var subject in subjects)
            {
                string header = InsertNewLineEveryNCharacters("📝 " + subject.Name, 35).Replace('.', '﹒') + "\n   🎓 " +
                                subject.FullName.Trim().Replace('.', '﹒');
                
                // DataTable не дозволяє однакові назви колонок, тому додаємо пробіл, якщо такий предмет вже є
                if (!dataTable.Columns.Contains(header))
                    dataTable.Columns.Add(header, typeof(string));
                else
                    dataTable.Columns.Add(header + " ", typeof(string)); 
            }

            int maxRowsCount = allTasks.Any() ? allTasks.GroupBy(t => t.IdSubject).Max(g => g.Count()) : 0;

            for (var i = 0; i < maxRowsCount; i++)
            {
                dataTable.Rows.Add(i + 1);
            }

            // Заповнюємо рядки завданнями
            for (var i = 1; i <= subjects.Count; i++)
            {
                var subjectTasks = allTasks.Where(t => t.IdSubject == subjects[i - 1].IdSubject).ToList();

                for (int counter = 0; counter < subjectTasks.Count; counter++)
                {
                    var task = subjectTasks[counter];
                    
                    var status = task.IdStatus switch
                    {
                        1 => "❌(0%)",
                        2 => "25%",
                        3 => "50%",
                        4 => "75%",
                        5 => "✔(100%)",
                        _ => ""
                    };

                    // Безпечна обробка DateOnly?
                    string deadline = task.Deadline.HasValue ? task.Deadline.Value.ToString("yyyy-MM-dd") : "❌";

                    _status.Add($"{counter} {i}", task.IdStatus);
                    
                    // Обробка decimal
                    string cellData = "Ŧ→ " + task.Task1 +
                                      "\nĐ→ " + deadline +
                                      "\nŞ→ " + status +
                                      "\nM→ " + task.MaxScore;

                    // Обробка decimal?
                    if (task.IdStatus == 5 && task.EarnedScore.HasValue)
                        cellData += "\nĖ→ " + task.EarnedScore.Value;

                    dataTable.Rows[counter][i] = cellData;
                }
            }

            // Прив'язуємо DataTable до WPF DataGrid
            DataGrid.Columns.Clear();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = dataTable.Columns[i].ColumnName,
                    Binding = new Binding(dataTable.Columns[i].ToString())
                });
            }

            DataGrid.Items.Clear();
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var tempTable = new DataTable();
                var tempRow = tempTable.NewRow();
                for (var j = 0; j < dataTable.Columns.Count; j++)
                {
                    tempTable.Columns.Add(dataTable.Columns[j].ColumnName, typeof(string));
                    tempRow[j] = dataTable.Rows[i][j];
                }

                tempTable.Rows.Add(tempRow);
                DataGrid.Items.Add(tempTable);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження таблиці: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // --- Навігація між вікнами ---

    private void Add_subject(object sender, RoutedEventArgs e)
    {
        var addSubjectWindow = new AddSubjectWindow(_idStudent);
        addSubjectWindow.Show();
        Hide();
    }

    private void Delete_subject(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Columns.Count <= 1)
        {
            MessageBox.Show("У вас немає предметів для видалення", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var deleteSubjectWindow = new DeleteSubjectWindow(_idStudent);
        deleteSubjectWindow.Show();
        Hide();
    }

    private void AddTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Columns.Count <= 1)
        {
            MessageBox.Show("У вас немає предметів для додавання завдань", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var addTaskWindow = new AddTaskWindow(_idStudent);
        addTaskWindow.Show();
        Hide();
    }

    private void DeleteTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Items.Count == 0)
        {
            MessageBox.Show("У вас немає завдань для видалення", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var deleteTaskWindow = new DeleteTaskWindow(_idStudent);
        deleteTaskWindow.Show();
        Hide();
    }

    private void Rozklad_OnClick(object sender, RoutedEventArgs e)
    {
        var rozkladWindow = new RozkladWindow(_idStudent);
        rozkladWindow.Show();
        Hide();
    }

    private void ChangeTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Items.Count == 0)
        {
            MessageBox.Show("У вас немає завдань для зміни", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var changeTaskWindow = new ChangeTaskWindow(_idStudent);
        changeTaskWindow.Show();
        Hide();
    }

    private void Click_exit(object sender, RoutedEventArgs e)
    {
        // Видаляємо локальний файл сесії при виході, щоб наступний користувач міг увійти
        if (File.Exists("session.txt"))
        {
            File.Delete("session.txt");
        }
        
        var startWindow = new StartWindow();
        startWindow.Show();
        Hide();
    }

    // --- Логіка інтерфейсу (Фарбування комірок) ---

    private void CheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        for (var i = 1; i < DataGrid.Columns.Count; i++)
        {
            for (var j = 0; j < DataGrid.Items.Count; j++)
            {
                var row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromIndex(j);
                if (row == null || !row.IsLoaded) continue;
                var presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                    if (cell != null)
                    {
                        if (ShowStatusCheckBox.IsChecked == true)
                        {
                            DataGrid.SelectedIndex = -1;
                            if (_status.ContainsKey($"{j} {i}"))
                            {
                                var status = _status[$"{j} {i}"];
                                cell.Background = status switch
                                {
                                    1 => new SolidColorBrush(Color.FromRgb(228, 92, 94)),
                                    2 => new SolidColorBrush(Color.FromRgb(255, 157, 61)),
                                    3 => new SolidColorBrush(Color.FromRgb(226, 193, 95)),
                                    4 => new SolidColorBrush(Color.FromRgb(127, 209, 111)),
                                    5 => new SolidColorBrush(Color.FromRgb(58, 130, 44)),
                                    _ => new SolidColorBrush(Color.FromRgb(173, 174, 210))
                                };
                            }
                        }
                        else
                        {
                            DataGrid.SelectedIndex = -1;
                            cell.Background = new SolidColorBrush(Color.FromRgb(173, 174, 210));
                        }
                    }
                }
            }
        }
        DataGrid.UpdateLayout();
    }

    private TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
        where TChildItem : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is TChildItem item)
                return item;
            var childOfChild = FindVisualChild<TChildItem>(child);
            if (childOfChild != null)
                return childOfChild;
        }

        return null!;
    }

    private string InsertNewLineEveryNCharacters(string input, int charactersPerLine)
    {
        var result = new StringBuilder();
        var words = input.Split(' ');
        var currentLine = new StringBuilder("ㅤ");
        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > charactersPerLine)
            {
                result.AppendLine(currentLine.ToString().TrimEnd());
                currentLine.Clear();
                currentLine.Append("ㅤ");
            }

            currentLine.Append(word + " ");
        }

        if (currentLine.Length > 0)
        {
            result.Append(currentLine.ToString().TrimEnd());
        }

        return result.ToString();
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}