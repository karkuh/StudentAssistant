namespace StudentAssistant.DB;

public class Db
{
    private const string ConnectionString = "server=localhost;uid=root;pwd=*987*321;database=studentassistant";
    public static DbConnection Connection = new(ConnectionString);
}