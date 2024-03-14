using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Subject;

public partial class DeleteSubjectWindow
{
    private readonly int _idStudent;


    public DeleteSubjectWindow(int idStudent)
    {
        InitializeComponent();
        _idStudent = idStudent;
        InitListBox();
        SubjectsListBox.SelectedIndex = 0;
    }

    private void InitListBox()
    {
        // Виклик процедури, що повертає усі предмети студента
        var sqlExpression =
            $"call SubjectsOfOneStudent({_idStudent})";
        var command = new MySqlCommand(sqlExpression, Db.Connection.Connect);
        var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                SubjectsListBox.Items.Add(
                    reader.GetInt32(0) + ". " + reader.GetString(1) + " -> " + reader.GetString(2));
            }
        }

        reader.Close();
    }

    private void OK_OnClick(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show(
                $"Видалити {SubjectsListBox.Items[SubjectsListBox.SelectedIndex]}. Разом з предметом видаляться усі завдання",
                "",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        
        var id = SubjectsListBox.SelectedItem.ToString()!.Split(". ")[0];
        // Видалення предмета
        var deleteQuery =
            $"delete from subjects_for_one_student where id_subject = {id} and id_student = {_idStudent};";
        var commandDelete = new MySqlCommand(deleteQuery, Db.Connection.Connect);
        commandDelete.ExecuteScalar();
        var start = new MainWindow(_idStudent);
        Hide();
        start.Show();
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow(_idStudent);
        Hide();
        main.Show();
    }

    private void DeleteSubject_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}