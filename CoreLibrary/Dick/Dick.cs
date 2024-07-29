namespace CoreLibrary.Dick;

public class Dick
{
    private int _energy;

    public Dick(long belongings, string? nickName, double length, string? guid)
    {
        Belongings = belongings;
        NickName = nickName;
        Length = length;
        GUID = guid;
    }

    private readonly int maxEnergy = 360; //体力最大值

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
        get => Math.Clamp(_energy, 0, maxEnergy);
        set => _energy = Math.Clamp(value, 0, maxEnergy);
    }
}