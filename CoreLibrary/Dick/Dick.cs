namespace CoreLibrary;

public class Dick
{
    private int _energy;

    public Dick(long belongings, string? nickName, int gender, double length, string? guid)
    {
        Belongings = belongings;
        NickName = nickName;
        Gender = (Gender)gender;
        Length = length;
        GUID = guid;
    }

    public long Belongings { get; private set; } //牛子所属的QQ号

    public string? NickName { get; set; }

    public Gender Gender { get; set; }

    public double Length { get; set; }

    public string? GUID { get; set; }

    public int GachaTickets { get; set; }

    public int DickType { get; set; }

    public int WeaponType { get; set; }

    public int Energy
    {
        get => Math.Clamp(_energy, 0, 240);
        set => _energy = Math.Clamp(value, 0, 240);
    }
}