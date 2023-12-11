using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Task;

public partial class DeleteTaskWindow
{
    private readonly int _idStudent;

    public DeleteTaskWindow(int idStudent)
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
                                          readerSubjectsForStudent.GetString(1) + " " +
                                          readerSubjectsForStudent.GetString(2));
            }
        }

        readerSubjectsForStudent.Close();
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }

    private void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TasksListBox.Items.Clear();
        var infoTaskQuery =
            $"select id_task,task,deadline from tasks where " +
            $"id_subject = {SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0]} and " +
            $"id_student = {_idStudent};";
        var commandInfoTask = new MySqlCommand(infoTaskQuery, Db.Connection.Connect);
        var readerInfoTask = commandInfoTask.ExecuteReader();
        if (readerInfoTask.HasRows)
        {
            while (readerInfoTask.Read())
            {
                TasksListBox.Items.Add(readerInfoTask.GetInt32(0) + ". " + readerInfoTask.GetString(1) +
                                       " -> " + readerInfoTask.GetString(2).Split(' ')[0]);
            }
        }

        readerInfoTask.Close();
    }
    
    private void DeleteTask_OnClick(object sender, RoutedEventArgs e)
    {
        var deleteQuery =
            $"delete from tasks where id_task = {TasksListBox.SelectedItem.ToString()!.Split(". ")[0]};";
        var commandDelete = new MySqlCommand(deleteQuery, Db.Connection.Connect);
        commandDelete.ExecuteScalar();
        
        MessageBox.Show("Завдання видалено","",MessageBoxButton.OK,MessageBoxImage.Information);
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }
    
    private void DeleteTaskWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}