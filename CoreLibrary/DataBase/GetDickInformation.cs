using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task<(bool ifExisted, Dick? dick)> CheckDickWithTwoId(long userId, long groupId)
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
            dick = new Dick((long)reader["DickBelongings"],
                reader["NickName"].ToString(),
                Convert.ToInt32(reader["Gender"]),
                (double)reader["Length"], reader["GUID"].ToString());
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
}