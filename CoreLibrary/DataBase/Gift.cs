using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task CompensationOnEnergy(long groupId, int energyAdd)
    {
        //初始化数据库
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var dataBase = new DickFighterDataBase();
        var dickList = await dataBase.CheckDickWithGroupId(groupId);

        await using var transaction = (SQLiteTransaction)await connection.BeginTransactionAsync();

        foreach (var dick in dickList)
        {
            var command = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE Energy SET EnergyLastUpdate=EnergyLastUpdate" + energyAdd +
                              " WHERE DickGUID=@GUID"
            };

            command.Parameters.AddWithValue("@GUID", dick.GUID);
            command.Transaction = transaction;
            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }
}