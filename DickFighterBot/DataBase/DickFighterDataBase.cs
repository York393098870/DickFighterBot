using System.Data.SQLite;
using NLog;

namespace DickFighterBot.DataBase;

public partial class DickFighterDataBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task Initialize()
    {
        //初始化数据库
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText = """
                          
                                              CREATE TABLE IF NOT EXISTS BasicInformation (GUID TEXT PRIMARY KEY,
                                                  DickBelongings INTEGER,
                                                  NickName TEXT,
                                                  Length REAL,
                                                  GroupNumber INTEGER
                                              );CREATE TABLE IF NOT EXISTS Energy(
                                                  DickGUID TEXT PRIMARY KEY,
                                                  EnergyLastUpdate INTEGER,EnergyLastUpdateTime INTEGER);CREATE TABLE IF NOT EXISTS GachaInformation(GUID TEXT PRIMARY KEY,GachaCurrency INTEGER);CREATE TABLE IF NOT EXISTS CoffeeInformation(GUID TEXT PRIMARY KEY,LastDrinkTime INTEGER)
                          """ //创建数据库表
        };
        await command.ExecuteNonQueryAsync();
    }

    public static async Task UpdaterForProgram()
    {
        //用于程序更新时的数据库更新，可能会破坏性删除一些表
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var deleteList = new[] { "ExerciseRecord", "GachaInformation", "BattleRecord" };

        //删除不用的表
        foreach (var table in deleteList)
        {
            var deleteCommand = new SQLiteCommand(connection)
            {
                CommandText = "DROP TABLE IF EXISTS " + table
            };
            await deleteCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task<bool> GenerateNewDick(long userId, long groupId, Dick.Dick newDick)
    {
        //给定指定QQ号和群号以及牛子，在数据库当中写入新的牛子
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "INSERT INTO BasicInformation (GUID, DickBelongings, NickName, Length, GroupNumber) " +
                    "VALUES (@GUID, @DickBelongings, @NickName, @Length,  @GroupNumber);" +
                    "INSERT INTO Energy (DickGUID, EnergyLastUpdate, EnergyLastUpdateTime) " +
                    "VALUES (@DickGUID, @EnergyLastUpdate, @EnergyLastUpdateTime)"
            };
            command.Parameters.AddWithValue("@GUID", newDick.GUID);
            command.Parameters.AddWithValue("@DickBelongings", userId);
            command.Parameters.AddWithValue("@NickName", "软弱牛子");
            command.Parameters.AddWithValue("@Length", newDick.Length);
            command.Parameters.AddWithValue("@GroupNumber", groupId);

            command.Parameters.AddWithValue("@DickGUID", newDick.GUID);
            command.Parameters.AddWithValue("@EnergyLastUpdate", 240);
            command.Parameters.AddWithValue("@EnergyLastUpdateTime", DateTimeOffset.Now.ToUnixTimeSeconds());

            // 执行插入操作
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        }
        catch (Exception e)
        {
            // 处理异常，例如记录错误日志
            Logger.Error($"数据库操作：生成新牛子时发生错误：{e.Message}");
            return false;
        }
    }

    public async Task<Dick.Dick?> GetRandomDick(long groupid, string guid)
    {
        // 这个方法给定一个groupid和一个Guid，在数据库BasicInformation当中根据groupid随机返回一行数据，并确保返回的数据中不包含与Guid相同的行。
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        // 构造SQL语句，排除指定GUID
        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT * FROM BasicInformation WHERE GroupNumber = @GroupNumber AND GUID != @ExcludedGuid ORDER BY RANDOM() LIMIT 1"
        };

        // 添加参数
        command.Parameters.AddWithValue("@GroupNumber", groupid);
        command.Parameters.AddWithValue("@ExcludedGuid", guid);

        await using var reader = await command.ExecuteReaderAsync();

        // 处理查询结果
        if (!await reader.ReadAsync()) return null;

        var dick = new Dick.Dick
        {
            Belongings = (long)reader["DickBelongings"], NickName = reader["NickName"].ToString(),
            Length = (double)reader["Length"],
            GUID = reader["GUID"].ToString()
        };

        return dick;
    }

    public async Task<Dick.Dick?> GetRandomDick(string guid)
    {
        // 这个方法给定一个Guid，在数据库BasicInformation当中随机返回一行数据，并确保返回的数据中不包含与Guid相同的行。
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        // 构造SQL语句，排除指定GUID
        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT * FROM BasicInformation WHERE GUID != @ExcludedGuid ORDER BY RANDOM() LIMIT 1"
        };

        // 添加参数
        command.Parameters.AddWithValue("@ExcludedGuid", guid);

        await using var reader = await command.ExecuteReaderAsync();

        // 处理查询结果
        if (!await reader.ReadAsync()) return null;
        var dick = new Dick.Dick
        {
            Belongings = (long)reader["DickBelongings"], NickName = reader["NickName"].ToString(),
            Length = (double)reader["Length"],
            GUID = reader["GUID"].ToString()
        };
        return dick;
    }

    public async Task<(int globalRank, int globalTotal, int groupRank, int groupTotal)> GetLengthRanks(
        string guid, long groupNumber)
    {
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        // 获取指定GUID的Length
        var lengthCommand = new SQLiteCommand(connection)
        {
            CommandText = "SELECT Length FROM BasicInformation WHERE GUID = @GUID"
        };
        lengthCommand.Parameters.AddWithValue("@GUID", guid);

        var length = (double)(await lengthCommand.ExecuteScalarAsync() ?? throw new Exception("GUID not found"));

        // 获取全局排名和全局总人数
        var globalRankCommand = new SQLiteCommand(connection)
        {
            CommandText = "SELECT COUNT(*) + 1 FROM BasicInformation WHERE Length > @Length"
        };
        globalRankCommand.Parameters.AddWithValue("@Length", length);

        var globalRank = Convert.ToInt32(await globalRankCommand.ExecuteScalarAsync() ?? 0);

        var globalTotalCommand = new SQLiteCommand(connection)
        {
            CommandText = "SELECT COUNT(*) FROM BasicInformation"
        };

        var globalTotal = Convert.ToInt32(await globalTotalCommand.ExecuteScalarAsync() ?? 0);

        // 获取群内排名和群内总人数
        var groupRankCommand = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT COUNT(*) + 1 FROM BasicInformation WHERE GroupNumber = @GroupNumber AND Length > @Length"
        };
        groupRankCommand.Parameters.AddWithValue("@GroupNumber", groupNumber);
        groupRankCommand.Parameters.AddWithValue("@Length", length);

        var groupRank = Convert.ToInt32(await groupRankCommand.ExecuteScalarAsync() ?? 0);

        var groupTotalCommand = new SQLiteCommand(connection)
        {
            CommandText = "SELECT COUNT(*) FROM BasicInformation WHERE GroupNumber = @GroupNumber"
        };
        groupTotalCommand.Parameters.AddWithValue("@GroupNumber", groupNumber);

        var groupTotal = Convert.ToInt32(await groupTotalCommand.ExecuteScalarAsync() ?? 0);

        return (globalRank, globalTotal, groupRank, groupTotal);
    }
}