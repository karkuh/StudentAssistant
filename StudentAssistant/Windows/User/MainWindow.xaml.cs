using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.Subject;
using StudentAssistant.Windows.Task;

namespace StudentAssistant.Windows.User;

public partial class MainWindow
{
    private readonly int _idStudent;

    private readonly Dictionary<string, int> _status;


    public MainWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        _status = new Dictionary<string, int>();
        Init_task();
    }

    private void Init_task()
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("№");

        var idSubjects = new List<int>(); // ідентифікатори предметів

        var columnHeaderQuery =
            $"select id_subject, name, full_name from student_subject_teacher_view where id_student = {_idStudent}; ";


        var commandColumnHeader = new MySqlCommand(columnHeaderQuery, Db.Connection.Connect);
        var readerColumnHeader = commandColumnHeader.ExecuteReader();

        if (readerColumnHeader.HasRows)
        {
            while (readerColumnHeader.Read())
            {
                idSubjects.Add(readerColumnHeader.GetInt32(0));
                dataTable.Columns.Add(
                    "✔ " + InsertNewLineEveryNCharacters(readerColumnHeader.GetString(1), 25) + "\n○ " +
                    readerColumnHeader.GetString(2).Trim().Replace('.', '。'),
                    typeof(string));
            }
        }

        readerColumnHeader.Close();

        //найбільша кількість рядків 
        var maxRowsCountQuery =
            $"select count(*) from tasks " +
            $"where id_student = {_idStudent} " +
            $"group by id_subject " +
            $"order by count(*) " +
            $"desc limit 1;";

        var commandMaxRowsCount = new MySqlCommand(maxRowsCountQuery, Db.Connection.Connect);
        var readerMaxRowsCount = commandMaxRowsCount.ExecuteReader();
        readerMaxRowsCount.Read();
        if (readerMaxRowsCount.HasRows)
        {
            var maxRowsCount = readerMaxRowsCount.GetInt32(0);
            readerMaxRowsCount.Close();

            for (var i = 0; i < maxRowsCount; i++)
            {
                dataTable.Rows.Add(i + 1);
            }

            for (var i = 1; i < dataTable.Columns.Count; i++)
            {
                var infoTaskQuery =
                    $"select task,deadline,id_status,max_score,earned_score from tasks where " +
                    $"id_subject = {idSubjects[i - 1]} and " +
                    $"id_student = {_idStudent};";
                var commandInfoTaskQuery = new MySqlCommand(infoTaskQuery, Db.Connection.Connect);
                var readerInfoTaskQuery = commandInfoTaskQuery.ExecuteReader();

                if (readerInfoTaskQuery.HasRows)
                {
                    var counter = 0;
                    while (readerInfoTaskQuery.Read())
                    {
                        var status = readerInfoTaskQuery.GetInt32(2) switch
                        {
                            1 => "❌(0%)",
                            2 => "25%",
                            3 => "50%",
                            4 => "75%",
                            5 => "✔(100%)",
                            _ => ""
                        };

                        string deadline;
                        try
                        {
                            deadline = readerInfoTaskQuery.GetString(1).Split(' ')[0];
                        }
                        catch (SqlNullValueException)
                        {
                            deadline = "❌";
                        }

                        _status.Add($"{counter} {i}", readerInfoTaskQuery.GetInt32(2));
                        dataTable.Rows[counter][i] =
                            "Ŧ→ " + readerInfoTaskQuery.GetString(0) +
                            "\nĐ→ " + deadline +
                            "\nŞ→ " + status +
                            "\nM→ " + readerInfoTaskQuery.GetString(3);

                        if (status == "✔(100%)")
                            dataTable.Rows[counter][i] += "\nĖ→ " + readerInfoTaskQuery.GetString(4);

                        counter++;
                    }
                }

                readerInfoTaskQuery.Close();
            }
        }

        readerMaxRowsCount.Close();
        // Прив'язка DataTable до DataGrid
        DataGrid.Columns.Clear();

        for (var i = 0; i < dataTable.Columns.Count; i++)
        {
            DataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = dataTable.Columns[i].ColumnName,
                Binding = new Binding(dataTable.Columns[i].ToString())
            });
        }

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

    private void Add_subject(object sender, RoutedEventArgs e)
    {
        var addSubjectWindow = new AddSubjectWindow(_idStudent);
        addSubjectWindow.Show();
        Hide();
    }

    private void Delete_subject(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Columns.Count == 1)
        {
            MessageBox.Show("У вас немає предметів для видалення", "", MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        var deleteSubjectWindow = new DeleteSubjectWindow(_idStudent);
        deleteSubjectWindow.Show();
        Hide();
    }
    
    private void AddTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Columns.Count == 1)
        {
            MessageBox.Show("У вас немає предметів для додавання завдань", "", MessageBoxButton.OK,
                MessageBoxImage.Warning);
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
            MessageBox.Show("У вас немає завдань для видалення", "", MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        var deleteTaskWindow = new DeleteTaskWindow(_idStudent);
        deleteTaskWindow.Show();
        Hide();
    }
    
    private void ChangeTask_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGrid.Items.Count == 0)
        {
            MessageBox.Show("У вас немає завдань для зміни", "", MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        var changeTaskWindow = new ChangeTaskWindow(_idStudent);
        changeTaskWindow.Show();
        Hide();
    }
    
    private void Click_exit(object sender, RoutedEventArgs e)
    {
        var startWindow = new StartWindow();
        startWindow.Show();
        Hide();
    }
    
    private void CheckBox_OnClick(object sender, RoutedEventArgs e)
    {
        for (var i = 1; i < DataGrid.Columns.Count; i++)
        {
            for (var j = 0; j < DataGrid.Items.Count; j++)
            {
                var row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromIndex(j);
                if (!row.IsLoaded) continue;
                var presenter = FindVisualChild<DataGridCellsPresenter>(row);
                {
                    var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                    if (ShowStatusCheckBox.IsChecked == true)
                    {
                        if (_status.ContainsKey($"{j} {i}"))
                        {
                            var status = _status[$"{j} {i}"];
                            cell.Background = status switch
                            {
                                1 => new SolidColorBrush(Color.FromRgb(228, 92, 94)),
                                2 => new SolidColorBrush(Color.FromRgb(255, 157, 61)),
                                3 => new SolidColorBrush(Color.FromRgb(226, 193, 95)),
                                4 => new SolidColorBrush(Color.FromRgb(127, 209, 111)),
                                5 =>new SolidColorBrush(Color.FromRgb(58, 130, 44)),
                                _ => new SolidColorBrush(Color.FromRgb(173, 174, 210))
                            };
                        }
                    }
                    else
                    {
                        cell.Background = new SolidColorBrush(Color.FromRgb(173, 174, 210));
                    }

                    // Оновлюємо відображення
                    DataGrid.UpdateLayout();
                }
            }
        }
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
            return childOfChild;
        }
    
        return null!;
    }

    private string InsertNewLineEveryNCharacters(string input, int charactersPerLine)
    {
        var result = new StringBuilder();

        for (var i = 0; i < input.Length; i += charactersPerLine)
        {
            var length = Math.Min(charactersPerLine, input.Length - i);
            result.Append(input.AsSpan(i, length));

            if (i + charactersPerLine < input.Length)
            {
                result.AppendLine();
            }
        }

        return result.ToString();
    }
    
    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}
