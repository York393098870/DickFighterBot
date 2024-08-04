using System.Data.SQLite;

namespace DickFighterBot.DataBase;

public partial class DickFighterDataBase
{
    public async Task<(bool, long)> CheckCoffeeInformation(string guid)
    {
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "SELECT * FROM CoffeeInformation WHERE GUID = @GUID "
            };

            command.Parameters.AddWithValue("@GUID", guid);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return (false, -1);

            var timeResult = Convert.ToInt64(reader["LastDrinkTime"]);
            return (true, timeResult);
        }
        catch (Exception e)
        {
            Logger.Error("检测咖啡数据是否存在时出现错误！");
            Logger.Error(e.Message);
            throw;
        }
    }

    public async Task<bool> CreateNewCoffeeLine(string guid)
    {
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "INSERT INTO CoffeeInformation (GUID,LastDrinkTime) VALUES (@GUID, @LastDrinkTime)"
            };

            command.Parameters.AddWithValue("@GUID", guid);
            command.Parameters.AddWithValue("@LastDrinkTime", DateTimeOffset.Now.ToUnixTimeSeconds());

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 1) return true;

            Logger.Error("创建咖啡数据时出现错误！");
            return false;
        }
        catch (Exception e)
        {
            Logger.Error("检测咖啡数据是否存在时出现错误！");
            Logger.Error(e.Message);
            throw;
        }
    }

    public async Task<bool> DrinkCoffee(string guid)
    {
        try
        {
            await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
            await connection.OpenAsync();

            var command = new SQLiteCommand(connection)
            {
                CommandText =
                    "UPDATE CoffeeInformation SET LastDrinkTime = @LastDrinkTime WHERE GUID = @GUID"
            };
            command.Parameters.AddWithValue("@GUID", guid);
            command.Parameters.AddWithValue("@LastDrinkTime", DateTimeOffset.Now.ToUnixTimeSeconds());

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 1) return true;

            Logger.Error("饮用咖啡时出现错误！");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}