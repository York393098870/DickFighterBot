namespace CoreLibrary.DataBase;

public static class DatabaseConnectionManager
{
    private static string _connectionString;

    public static string ConnectionString
    {
        get
        {
            if (string.IsNullOrEmpty(_connectionString)) InitializeConnectionString();

            return _connectionString;
        }
    }

    private static void InitializeConnectionString()
    {
        if (!Directory.Exists(ProgramPath.MathPath)) Directory.CreateDirectory(ProgramPath.MathPath);

        var dataBaseSource = Path.Combine(ProgramPath.MathPath, "dickfightdatabase.db");
        _connectionString = $"Data Source={dataBaseSource};Version=3;";
        Console.WriteLine($"SQLite数据库已连接！文件路径：{dataBaseSource}");
    }
}