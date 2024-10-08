﻿using DickFighterBot.Tools;
using NLog;

namespace DickFighterBot.DataBase;

public static class DatabaseConnectionManager
{
    private static string _connectionString;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

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
        if (!Directory.Exists(ProgramPath.LocalProgramPath)) Directory.CreateDirectory(ProgramPath.LocalProgramPath);

        var dataBaseSource = Path.Combine(ProgramPath.LocalProgramPath, "dickfightdatabase.db");
        _connectionString = $"Data Source={dataBaseSource};Version=3;";
        Logger.Info($"SQLite数据库已连接！文件路径：{dataBaseSource}");
    }
}