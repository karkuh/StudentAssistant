using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Task;

public partial class ChangeTaskWindow
{
    private readonly int _idStudent;


    public ChangeTaskWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        InitListBox();
        SubjectsListBox.SelectedIndex = 0;
    }

    private void InitListBox()
    {
        var subjectsForStudentQuery =
            $"call SubjectsOfOneStudent ({_idStudent});";
        var commandSubjectsForStudent = new MySqlCommand(subjectsForStudentQuery, Db.Connection.Connect);
        var readerSubjectsForStudent = commandSubjectsForStudent.ExecuteReader();
        if (readerSubjectsForStudent.HasRows)
        {
            while (readerSubjectsForStudent.Read())
            {
                SubjectsListBox.Items.Add(readerSubjectsForStudent.GetInt32(0) + ". " +
                                          readerSubjectsForStudent.GetString(1) +
                                          " " + readerSubjectsForStudent.GetString(2));
            }
        }

        readerSubjectsForStudent.Close();
    }


    private void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        TasksListBox.Items.Clear();

        var tasksQuery =
            $"select id_task,task from tasks where " +
            $"id_subject = {SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0]} and " +
            $"id_student = {_idStudent};";
        var commandTasks = new MySqlCommand(tasksQuery, Db.Connection.Connect);
        var readerTasks = commandTasks.ExecuteReader();
        if (readerTasks.HasRows)
        {
            while (readerTasks.Read())
            {
                TasksListBox.Items.Add(readerTasks.GetInt32(0) + ". " + readerTasks.GetString(1));
            }
        }

        readerTasks.Close();

        TaskTextBox.Text = "";
        DeadlineDatePicker.Text = "";
        TasksListBox.SelectedIndex = 0;
    }

    private void TasksListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TasksListBox.Items.IsEmpty) return;
        
        TaskTextBox.Text = "";
        DeadlineDatePicker.Text = "";

        var infoTaskQuery =
            $"select task,deadline,id_status,max_score,earned_score from tasks  where " +
            $"id_task = {TasksListBox.SelectedItem.ToString()!.Split(". ")[0]} ;";
        var commandInfoTask = new MySqlCommand(infoTaskQuery, Db.Connection.Connect);
        var readerInfoTask = commandInfoTask.ExecuteReader();
        if (!readerInfoTask.HasRows) return;
        readerInfoTask.Read();
        
        TaskTextBox.Text = readerInfoTask.GetString(0);
        DeadlineDatePicker.Text = readerInfoTask.GetString(1);
        var index = readerInfoTask.GetInt32(2) - 1;
        MaxScoreTextBox.Text = readerInfoTask.GetString(3).Replace(',', '.');
        readerInfoTask.Close();
        StatusListBox.SelectedIndex = -1;
        StatusListBox.SelectedIndex = index;
    }

    private void StatusListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EarnedScoreTextBox.Visibility = StatusListBox.SelectedIndex == 4 ? Visibility.Visible : Visibility.Hidden;
        if (StatusListBox.SelectedIndex != 4) return;
        var earnedScoreQuery =
            $"select earned_score from tasks where " +
            $"id_task = {TasksListBox.SelectedItem.ToString()!.Split(". ")[0]};";
        var commandEarnedScore = new MySqlCommand(earnedScoreQuery, Db.Connection.Connect);
        var earnedScore = Convert.ToString(commandEarnedScore.ExecuteScalar())!;
        EarnedScoreTextBox.Text = earnedScore.Replace(',', '.');
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

    private void ChangeButton_OnClick(object sender, RoutedEventArgs e)
    {
        string updateQuery;
        if (StatusListBox.SelectedIndex == 4)
        {
            if (string.IsNullOrEmpty(TaskTextBox.Text) || string.IsNullOrEmpty(MaxScoreTextBox.Text) ||
                string.IsNullOrEmpty(EarnedScoreTextBox.Text))
            {
                MessageBox.Show("Заповніть усі необхідні поля","",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            updateQuery =
                $"update tasks " +
                $"SET task = '{TaskTextBox.Text}'," +
                $"deadline = '{StartWindow.TransformDateFormat(DeadlineDatePicker.Text)}'," +
                $"max_score = {MaxScoreTextBox.Text.Replace(',', '.')}," +
                $"earned_score = {EarnedScoreTextBox.Text.Replace(',', '.')}," +
                $"id_status = {StatusListBox.SelectedIndex + 1} where " +
                $"id_task = {TasksListBox.SelectedItem.ToString()!.Split(". ")[0]};";
        }
        else
        {
            if (string.IsNullOrEmpty(TaskTextBox.Text) || string.IsNullOrEmpty(MaxScoreTextBox.Text))
            {
                MessageBox.Show("Заповніть усі необхідні поля","",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            updateQuery =
                $"update tasks " +
                $"SET task = '{TaskTextBox.Text}'," +
                $"deadline = '{StartWindow.TransformDateFormat(DeadlineDatePicker.Text)}'," +
                $"max_score = {MaxScoreTextBox.Text.Replace(',', '.')}," +
                $"id_status = {StatusListBox.SelectedIndex + 1} where " +
                $"id_task = {TasksListBox.SelectedItem.ToString()!.Split(". ")[0]};";
        }


        if (!CheckNumber(MaxScoreTextBox.Text.Replace('.', ',')) || StatusListBox.SelectedIndex == 4 &&
            !CheckNumber(EarnedScoreTextBox.Text.Replace('.', ',')))
        {
            MessageBox.Show("Введіть коректні значення","",MessageBoxButton.OK,MessageBoxImage.Warning);
            return;
        }


        var commandUpdateQuery = new MySqlCommand(updateQuery, Db.Connection.Connect);
        commandUpdateQuery.ExecuteScalar();

        MessageBox.Show("Завдання оновлено","",MessageBoxButton.OK,MessageBoxImage.Information);
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
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