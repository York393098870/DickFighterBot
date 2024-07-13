using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task<(bool ifExisted, Dick.Dick? dick)> CheckDickWithIds(long userId, long groupId)
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

        Dick.Dick? dick;

        if (await reader.ReadAsync())
        {
            var dickBelongs = (long)reader["DickBelongings"];
            var nickName = (string)reader["NickName"];
            var length = (double)reader["Length"];
            var guid = (string)reader["GUID"];

            dick = new Dick.Dick(belongings: dickBelongs, nickName: nickName, length: length, guid: guid);
            return (ifExisted: true, dick);
        }

        dick = null;
        return (ifExisted: false, dick);
    }

    public async Task<int> CheckDickEnergyWithGuid(string guid)
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

        if (!await reader.ReadAsync()) throw new Exception("未查询到你的牛子数据！");
        var energyLastUpdate = Convert.ToInt64(reader["EnergyLastUpdate"]);
        var energyLastUpdateTime = Convert.ToInt64(reader["EnergyLastUpdateTime"]);

        var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        var timeDifference = currentTime - energyLastUpdateTime;

        var energyNow = Convert.ToInt32(energyLastUpdate + timeDifference / (6 * 60));
        return energyNow;
    }

    private async Task<List<Dick.Dick>> CheckDickWithGroupId(long groupId)
    {
        //给定指定群号，查询群内所有牛子的信息
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT * FROM BasicInformation WHERE GroupNumber = @GroupNumber"
        };
        command.Parameters.AddWithValue("@GroupNumber", groupId);

        await using var reader = await command.ExecuteReaderAsync();

        var dickList = new List<Dick.Dick>();

        while (await reader.ReadAsync())
        {
            var dickBelongs = (long)reader["DickBelongings"];
            var nickName = (string)reader["NickName"];
            var length = (double)reader["Length"];
            var guid = (string)reader["GUID"];

            var dick = new Dick.Dick(dickBelongs, nickName, length, guid);
            dickList.Add(dick);
        }

        return dickList;
    }
}