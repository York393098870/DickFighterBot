namespace CoreLibrary.DataBase;

public static class DatabaseConnectionManager
{
    private static string _connectionString;

    public static string ConnectionString
    {
        get
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                InitializeConnectionString();
            }

            return _connectionString;
        }
    }

    private static void InitializeConnectionString()
    {
        var databaseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DickFighterBot");
        if (!Directory.Exists(databaseFolderPath))
        {
            Directory.CreateDirectory(databaseFolderPath);
        }

        var dataBaseSource = Path.Combine(databaseFolderPath, "dickfightdatabase.db");
        _connectionString = $"Data Source={dataBaseSource};Version=3;";
        Console.WriteLine($"SQLite数据库已连接！文件路径：{dataBaseSource}");
    }
}