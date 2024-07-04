using System.Data.SQLite;

namespace CoreLibrary.DataBase;

public partial class DickFighterDataBase
{
    public async Task<int> GetCountOfTotalDicks()
    {
        //返回数据库中牛子的总数
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT COUNT(*) counts FROM main.BasicInformation"
        };

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            if (int.TryParse(reader["counts"].ToString(), out var count))
                return count;

        Logger.Error("无法获取数据库中牛子的总数！");
        throw new Exception("无法获取数据库当中的牛子总数！");
    }

    public async Task<int> GetCountOfTotalDicks(long group_id)
    {
        //返回数据库中单个群内牛子的总数
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT COUNT(*) counts FROM main.BasicInformation WHERE GroupNumber = @group_id"
        };
        command.Parameters.AddWithValue("@group_id", group_id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            if (int.TryParse(reader["counts"].ToString(), out var count))
                return count;

        Logger.Error("无法获取数据库中指定群牛子的总数！");
        throw new Exception("无法获取数据库中指定群牛子的总数！");
    }

    public async Task<List<Dick>> GetFirstNDicksByOrder(int n, int order = 0)
    {
        //根据给定的排序方式，返回数据库中的前n个牛子（或者后n个）
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        SQLiteCommand command;

        switch (order)
        {
            case 0:
                command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT * FROM main.BasicInformation ORDER BY Length DESC LIMIT @n"
                };
                break;
            case 1:
                command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT * FROM main.BasicInformation ORDER BY Length LIMIT @n"
                };
                break;
            default:
                Logger.Error("不允许的排序参数：{order}，排序参数只能为0或1！");
                throw new ArgumentOutOfRangeException(nameof(order), "排序方式只能为0或1");
        }

        command.Parameters.AddWithValue("@n", n);

        await using var reader = await command.ExecuteReaderAsync();

        var dickList = new List<Dick>();

        while (await reader.ReadAsync())
        {
            var dickBelongings = Convert.ToInt64(reader["DickBelongings"]);
            var nickName = reader["NickName"].ToString();
            var gender = Convert.ToInt32(reader["Gender"]);
            var length = Convert.ToDouble(reader["Length"]);
            var guid = Convert.ToString(reader["GUID"]);

            var dick = new Dick(dickBelongings, nickName, gender, length, guid);

            dickList.Add(dick);
        }

        return dickList;
    }

    public async Task<List<Dick>> GetFirstNDicksByOrder(int n, long group_id, int order = 0)
    {
        //根据给定的排序方式，返回指定群中的前n个牛子（或者后n个）
        await using var connection = new SQLiteConnection(DatabaseConnectionManager.ConnectionString);
        await connection.OpenAsync();

        SQLiteCommand command;

        switch (order)
        {
            case 0:
                command = new SQLiteCommand(connection)
                {
                    CommandText =
                        "SELECT * FROM main.BasicInformation WHERE GroupNumber= @group_id ORDER BY Length DESC LIMIT @n"
                };
                break;
            case 1:
                command = new SQLiteCommand(connection)
                {
                    CommandText =
                        "SELECT * FROM main.BasicInformation WHERE GroupNumber= @group_id ORDER BY Length LIMIT @n"
                };
                break;
            default:
                Logger.Error("不允许的排序参数：{order}，排序参数只能为0或1！");
                throw new ArgumentOutOfRangeException(nameof(order), "排序方式只能为0或1");
        }

        command.Parameters.AddWithValue("@n", n);
        command.Parameters.AddWithValue("@group_id", group_id);

        await using var reader = await command.ExecuteReaderAsync();

        var dickList = new List<Dick>();

        while (await reader.ReadAsync())
        {
            var dickBelongings = Convert.ToInt64(reader["DickBelongings"]);
            var nickName = reader["NickName"].ToString();
            var gender = Convert.ToInt32(reader["Gender"]);
            var length = Convert.ToDouble(reader["Length"]);
            var guid = Convert.ToString(reader["GUID"]);

            var dick = new Dick(dickBelongings, nickName, gender, length, guid);

            dickList.Add(dick);
        }

        return dickList;
    }
}