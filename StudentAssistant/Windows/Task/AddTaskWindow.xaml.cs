using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Task;

public partial class AddTaskWindow
{
    private readonly int _idStudent;

    public AddTaskWindow(int idStudent)
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
                SubjectsListBox.Items.Add(readerSubjectsForStudent.GetInt32(0) + ". " + readerSubjectsForStudent.GetString(1) +
                              " " + readerSubjectsForStudent.GetString(2));
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

    private void Add_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(TaskTextBox.Text))
        {
            MessageBox.Show("Введіть завдання","",MessageBoxButton.OK,MessageBoxImage.Warning);
            return;
        }

        var idSubject = SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0];

        var insertTaskQuery = string.IsNullOrEmpty(DeadLineDatePicker.Text)
            ? $"insert into tasks (id_student, id_subject, task,max_score)values ({_idStudent},{idSubject},'{TaskTextBox.Text}','{MaxScoreSlider.Value}'); "
            : $"insert into tasks (id_student, id_subject, task,deadline, max_score)values ({_idStudent},{idSubject},'{TaskTextBox.Text}','{StartWindow.TransformDateFormat(DeadLineDatePicker.Text)}','{MaxScoreSlider.Value}'); ";

        var commandInsertTask = new MySqlCommand(insertTaskQuery, Db.Connection.Connect);
        commandInsertTask.ExecuteScalar();

        MessageBox.Show("Завдання добавлене","",MessageBoxButton.OK,MessageBoxImage.Information);
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }
    
    private void AddTaskWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}