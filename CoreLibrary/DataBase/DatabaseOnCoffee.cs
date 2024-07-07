using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task<bool> CheckIfCoffeeLineExisted(string guid)
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

            return await reader.ReadAsync();
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

            if (rowsAffected == 1)
            {
                return true;
            }

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
}