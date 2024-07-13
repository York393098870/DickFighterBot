using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task<(int gachaTickets, int dickType, int weaponType)> CheckGachaInfo(string guid)
    {
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();
        var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM GachaInformation WHERE GUID = @GUID"
        };
        command.Parameters.AddWithValue("@GUID", guid);
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            //存在相关记录，直接返回数据
            return (gachaTickets: Convert.ToInt32(reader["GachaTickets"]),
                dickType: Convert.ToInt32(reader["DickType"]),
                weaponType: Convert.ToInt32(reader["WeaponType"]));

        //自动初始化一条记录
        var initializeResult = await CreateNewGachaRecord(guid);
        if (initializeResult)
        {
            Logger.Info("没有查询到该牛子的抽卡信息，正在初始化...");
            return (gachaTickets: 2, dickType: 0, weaponType: 0);
        }

        Logger.Error("初始化抽卡信息失败！无法为新牛子插入初始记录！");
        throw new Exception("检查牛子信息时发生致命错误！");
    }

    

    private async Task<bool> CreateNewGachaRecord(string guid)
    {
        //为新牛子在数据库当中生成一条初始记录
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();
        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "INSERT INTO GachaInformation (GUID,GachaTickets,DickType,WeaponType) VALUES (@GUID, 2, 0, 0)"
        };
        command.Parameters.AddWithValue("@GUID", guid);
        await command.ExecuteNonQueryAsync();

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}