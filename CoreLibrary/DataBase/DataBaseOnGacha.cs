using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public static async Task CheckGachaInfo(string GUID)
    {
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();
        var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM GachaInformation WHERE GUID = @GUID"
        };
        command.Parameters.AddWithValue("@GUID", GUID);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            // GachaInformation exists
        }
        else
        {
            //自动初始化一条记录
        }
    }

    public static async Task InitializeGachaInfoForNewDick(string GUID)
    {
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();
        var command = new SQLiteCommand(connection)
        {
            CommandText = "INSERT INTO GachaInformation (GUID, GachaTicket,DickType,WeaponType) VALUES (@GUID, 2, 0, 0)"
        };
        command.Parameters.AddWithValue("@GUID", GUID);
        await command.ExecuteNonQueryAsync();
        
        var rowsAffected = await command.ExecuteNonQueryAsync();
        /*return rowsAffected > 0;*/
    }
}