using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public class DickFighterDataBase
{
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
                                              );CREATE TABLE IF NOT EXISTS BattleRecord (
                                                  BattleID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                  ChallengerGUID TEXT,
                                                  DefenderGUID TEXT,
                                                  IsWin INTEGER,
                                                  Time INTEGER
                                              );CREATE TABLE IF NOT EXISTS ExerciseRecord (
                                                  ExerciseID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                  DickGUID TEXT,
                                                  ExerciseTime INTEGER,
                                                  ExerciseChange REAL,
                                                  NextExerciseTime INTEGER
                                              );CREATE TABLE IF NOT EXISTS Energy(
                                                  DickGUID TEXT PRIMARY KEY,
                                                  EnergyLastUpdate INTEGER,EnergyLastUpdateTime INTEGER);
                          """ //创建数据库表
        };
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<(bool, Dick? dick)> CheckPersonalDick(long userId, long groupId)
    {
        //给定指定QQ号和群号，查询牛子是否存在并返回结果
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT GUID, DickBelongings, NickName, Length, Gender FROM BasicInformation WHERE DickBelongings = @DickBelongings AND GroupNumber = @GroupNumber"
        };
        command.Parameters.AddWithValue("@DickBelongings", userId);
        command.Parameters.AddWithValue("@GroupNumber", groupId);

        await using var reader = await command.ExecuteReaderAsync();

        Dick? dick;

        if (await reader.ReadAsync())
        {
            dick = new Dick(belongings: (long)reader["DickBelongings"],
                nickName: reader["NickName"].ToString(),
                gender: Convert.ToInt32(reader["Gender"]),
                length: (double)reader["Length"], guid: reader["GUID"].ToString());
            return (true, dick);
        }

        dick = null;
        return (false, dick);
    }

    public static async Task<bool> GenerateNewDick(long userId, long groupId, Dick newDick)
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
            command1.Parameters.AddWithValue("@NickName", "不知名的牛子");
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

            Console.WriteLine($"RowsAffected1: {rowsAffected1}, RowsAffected2: {rowsAffected2}");

            if (rowsAffected1 > 0 && rowsAffected2 > 0)
            {
                await transaction.CommitAsync();
                return true;
            }
            else
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        catch (Exception e)
        {
            // 处理异常，例如记录错误日志
            Console.WriteLine($"插入数据时发生错误：{e.Message}");

            // 返回插入失败
            return false;
        }
    }

    public static async Task<bool> UpdateDickNickName(long userId, long groupId, string newNickName)
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
            Console.WriteLine($"更新数据时发生错误：{e.Message}");

            // 返回更新失败
            return false;
        }
    }

    public static async Task<int> CheckEnergy(string guid)
    {
        //给定指定QQ号和群号，查询牛子的体力值
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT EnergyLastUpdate, EnergyLastUpdateTime FROM Energy WHERE DickGUID = @DickGUID "
        };
        command.Parameters.AddWithValue("@DickGUID", guid);

        await using var reader = await command.ExecuteReaderAsync();


        if (await reader.ReadAsync())
        {
            var energyLastUpdate = Convert.ToInt64(reader["EnergyLastUpdate"]);
            var energyLastUpdateTime = Convert.ToInt64(reader["EnergyLastUpdateTime"]);

            var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var timeDifference = currentTime - energyLastUpdateTime;

            var energyNow = Convert.ToInt32(energyLastUpdate + timeDifference / (6 * 60));
            return energyNow;
        }

        throw new Exception("未查询到你的牛子数据！");
    }

    public static async Task<bool> UpdateDickEnergy(int energy, string guid)
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
            // 处理异常，例如记录错误日志
            Console.WriteLine($"更新数据时发生错误：{e.Message}");

            // 返回更新失败
            return false;
        }
    }
    
    public static async Task<bool> UpdateDickLength(double length, string guid)
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
            // 处理异常，例如记录错误日志
            Console.WriteLine($"更新数据时发生错误：{e.Message}");

            // 返回更新失败
            return false;
        }
    }
}