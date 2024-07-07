using System.Data.SQLite;
using NLog;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task InitializeDataBase()
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
                                                  Gender INTEGER,
                                                  GroupNumber INTEGER
                                              );CREATE TABLE IF NOT EXISTS Energy(
                                                  DickGUID TEXT PRIMARY KEY,
                                                  EnergyLastUpdate INTEGER,EnergyLastUpdateTime INTEGER);CREATE TABLE IF NOT EXISTS GachaInformation(GUID TEXT PRIMARY KEY,DickType INTEGER,WeaponType INTEGER,GachaTickets INTEGER);CREATE TABLE IF NOT EXISTS CoffeeInformation(GUID TEXT PRIMARY KEY,LastDrinkTime INTEGER)
                          """ //创建数据库表
        };
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> GenerateNewDick(long userId, long groupId, Dick newDick)
    {
        //给定指定QQ号和群号以及牛子，在数据库当中写入新的牛子
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            var command1 = new SQLiteCommand(connection)
            {
                CommandText =
                    "INSERT INTO BasicInformation (GUID, DickBelongings, NickName, Length, Gender, GroupNumber) " +
                    "VALUES (@GUID, @DickBelongings, @NickName, @Length, @Gender, @GroupNumber)"
            };
            command1.Parameters.AddWithValue("@GUID", newDick.GUID);
            command1.Parameters.AddWithValue("@DickBelongings", userId);
            command1.Parameters.AddWithValue("@NickName", "未改名的牛子");
            command1.Parameters.AddWithValue("@Length", newDick.Length);
            command1.Parameters.AddWithValue("@Gender", 1);
            command1.Parameters.AddWithValue("@GroupNumber", groupId);

            // 执行插入操作
            var rowsAffected1 = await command1.ExecuteNonQueryAsync();

            // 为新牛子提供体力值
            var command2 = new SQLiteCommand(connection)
            {
                CommandText =
                    "INSERT INTO Energy (DickGUID, EnergyLastUpdate, EnergyLastUpdateTime) " +
                    "VALUES (@DickGUID, @EnergyLastUpdate, @EnergyLastUpdateTime)"
            };
            command2.Parameters.AddWithValue("@DickGUID", newDick.GUID);
            command2.Parameters.AddWithValue("@EnergyLastUpdate", 240);
            command2.Parameters.AddWithValue("@EnergyLastUpdateTime", DateTimeOffset.Now.ToUnixTimeSeconds());

            var rowsAffected2 = await command2.ExecuteNonQueryAsync();

            if (rowsAffected1 > 0 && rowsAffected2 > 0)
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
            // 返回插入失败
            return false;
        }
    }

    public async Task<bool> UpdateDickNickName(long userId, long groupId, string newNickName)
    {
        // 根据GroupNumber和DickBelongings修改NickName
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "UPDATE BasicInformation SET NickName = @NickName WHERE DickBelongings = @DickBelongings AND GroupNumber = @GroupNumber"
            };
            command.Parameters.AddWithValue("@NickName", newNickName);
            command.Parameters.AddWithValue("@DickBelongings", userId);
            command.Parameters.AddWithValue("@GroupNumber", groupId);

            // 执行更新操作
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            // 处理异常，例如记录错误日志
            Logger.Error($"数据库操作：更新牛子昵称时发生错误：{e.Message}");
            // 返回更新失败
            return false;
        }
    }

    public async Task<bool> UpdateDickEnergy(int energy, string guid)
    {
        // 根据GUID更新体力
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "UPDATE Energy SET EnergyLastUpdate=@EnergyLastUpdate,EnergyLastUpdateTime=@EnergyLastUpdateTime  WHERE DickGUID = @DickGUID"
            };
            command.Parameters.AddWithValue("@EnergyLastUpdate", energy);
            command.Parameters.AddWithValue("@EnergyLastUpdateTime", DateTimeOffset.Now.ToUnixTimeSeconds());
            command.Parameters.AddWithValue("@DickGUID", guid);

            // 执行更新操作
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            // 处理异常，记录错误日志
            Logger.Error($"数据库操作：更新牛子体力时发生错误：{e.Message}");

            // 返回更新失败
            return false;
        }
    }

    public async Task<bool> UpdateDickLength(double length, string guid)
    {
        // 根据GUID更新体力
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "UPDATE BasicInformation SET Length=@Length  WHERE GUID = @GUID"
            };
            command.Parameters.AddWithValue("@Length", length);
            command.Parameters.AddWithValue("@GUID", guid);


            // 执行更新操作
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            // 处理异常，记录错误日志
            Logger.Error($"数据库操作：更新牛子长度时发生错误：{e.Message}");
            // 返回更新失败
            return false;
        }
    }

    public async Task<Dick?> GetRandomDick(long groupid, string guid)
    {
        // 这个方法给定一个groupid和一个excludedGuid，在数据库BasicInformation当中根据groupid随机返回一行数据，并确保返回的数据中不包含与excludedGuid相同GUID的行。
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
        if (await reader.ReadAsync())
        {
            var dick = new Dick(
                (long)reader["DickBelongings"],
                reader["NickName"].ToString(),
                Convert.ToInt32(reader["Gender"]),
                (double)reader["Length"],
                reader["GUID"].ToString()
            );
            return dick;
        }

        // 未找到符合条件的数据
        return null;
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