using System.Windows;
using MySql.Data.MySqlClient;

namespace StudentAssistant.DB;

public class DbConnection
{
    public MySqlConnection Connect { get; }

    public DbConnection(string connectionString)
    {
        Connect = new MySqlConnection(connectionString);
        ConnectionDb();
    }

    private void ConnectionDb()
    {
        try
        {
            Connect.Open();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            Connect.Clone();
        }
    }
}