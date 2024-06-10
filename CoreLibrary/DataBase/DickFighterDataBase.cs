using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public class DickFighterDataBase
{
    private const string ConnectionString = "Data Source=dickfightdatabase.db;Version=3;"; //定义连接地址

    public static async Task<(bool, Dick? dick)> CheckPersonalDick(long userId, long groupId)
    {
        await using var connection = new SQLiteConnection(ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText =
                "SELECT GUID FROM BasicInformation WHERE DickBelongings = @DickBelongings AND GroupNumber = @GroupNumber"
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
}