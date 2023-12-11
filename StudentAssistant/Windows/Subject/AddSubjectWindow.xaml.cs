using System;
using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using StudentAssistant.DB;
using StudentAssistant.Windows.User;

namespace StudentAssistant.Windows.Subject;

public partial class AddSubjectWindow
{
    private readonly int _idStudent;

    public AddSubjectWindow(int idStudent)
    {
        InitializeComponent();
        InitListBox();
        InitComboBox();
        _idStudent = idStudent;
        OldRadioButton.IsChecked = true;
    }

    private void InitListBox()
    {
        const string teacherQuery = "select id_teacher, full_name from teacher_view;";
        var commandTeacher = new MySqlCommand(teacherQuery, Db.Connection.Connect);
        var readerTeacher = commandTeacher.ExecuteReader();
        if (readerTeacher.HasRows)
        {
            while (readerTeacher.Read())
            {
                OldTeacherListBox.Items.Add(readerTeacher.GetInt32(0) + ". " + readerTeacher.GetString(1));
            }
        }

        readerTeacher.Close();
    }

    private void InitComboBox()
    {
        var subjectQuery =
            $"select name from subjects where id_subject in ( " +
            $"select distinct id_subject from subjects_for_one_student where id_subject not in  (" +
            $"select distinct id_subject from subjects_for_one_student where id_student = {_idStudent}))";
        var commandSubject = new MySqlCommand(subjectQuery, Db.Connection.Connect);
        var readerSubject = commandSubject.ExecuteReader();
        if (readerSubject.HasRows)
        {
            while (readerSubject.Read())
            {
                SubjectTextBox.Items.Add(readerSubject.GetString(0));
            }
        }

        readerSubject.Close();
    }

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }
    
    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (OldRadioButton.IsChecked == true)
        {
            OldTeacherListBox.Visibility = Visibility.Visible;
            GridNewTeacher.Visibility = Visibility.Hidden;
        }
        else
        {
            OldTeacherListBox.Visibility = Visibility.Hidden;
            GridNewTeacher.Visibility = Visibility.Visible;
        }
    }

    private void Add_OnClick(object sender, RoutedEventArgs e)
    {
        if (OldRadioButton.IsChecked == true)
        {
            if (OldTeacherListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Оберіть викладача","",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) || string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(MiddleNameTextBox.Text))
            {
                MessageBox.Show("Заповінть усі поля","",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
        }

        if (string.IsNullOrEmpty(SubjectTextBox.Text))
        {
            MessageBox.Show("Введіть назву предмета","",MessageBoxButton.OK,MessageBoxImage.Warning);
            return;
        }

        int idTeacher;

        if (OldRadioButton.IsChecked == true)
        {
            idTeacher = Convert.ToInt32(OldTeacherListBox.SelectedItem.ToString()!.Split(". ")[0]);
        }
        else
        {
            var insertTeacherQuery =
                $"INSERT INTO teachers (last_name, first_name, middle_name) " +
                $"SELECT '{LastNameTextBox.Text}', '{FirstNameTextBox.Text}', '{MiddleNameTextBox.Text}' " +
                $"WHERE NOT EXISTS ( " +
                $"SELECT 1 " +
                $"FROM teachers " +
                $"WHERE last_name = '{LastNameTextBox.Text}' " +
                $"AND first_name = '{FirstNameTextBox.Text}' " +
                $"AND middle_name = '{MiddleNameTextBox.Text}');" +
                $"select id_teacher from teachers where last_name = '{LastNameTextBox.Text}' " +
                $"AND first_name = '{FirstNameTextBox.Text}' " +
                $"AND middle_name = '{MiddleNameTextBox.Text}';";
            var commandInsertTeacher = new MySqlCommand(insertTeacherQuery, Db.Connection.Connect);
            idTeacher = Convert.ToInt32(commandInsertTeacher.ExecuteScalar());
        }


        var insertSubjectQuery =
            $"insert into subjects(name, id_teacher)SELECT '{SubjectTextBox.Text}',{idTeacher} WHERE NOT EXISTS ( SELECT 1 FROM subjects WHERE name = '{SubjectTextBox.Text}' and id_teacher = {idTeacher});" +
            $"select id_subject from subjects where name = '{SubjectTextBox.Text}' and id_teacher = {idTeacher}; ";
        var commandInsertSubject = new MySqlCommand(insertSubjectQuery, Db.Connection.Connect);
        var idSubject = Convert.ToInt32(commandInsertSubject.ExecuteScalar());

        var insertSubjectForStudentQuery =
            $"insert into subjects_for_one_student(id_student, id_subject) values ({_idStudent},{idSubject});";
        var commandInsertSubjectForStudentQuery = new MySqlCommand(insertSubjectForStudentQuery, Db.Connection.Connect);
        commandInsertSubjectForStudentQuery.ExecuteScalar();

        MessageBox.Show("Предмет додано","",MessageBoxButton.OK,MessageBoxImage.Information);

        var mainWindow = new MainWindow(_idStudent);
        mainWindow.Show();
        Hide();
    }
    
    private void AddSubjectWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Close();
    }
}