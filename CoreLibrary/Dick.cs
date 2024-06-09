using System.Net.Mail;
using CoreLibrary.Tools;

namespace CoreLibrary;

public class Dick
{
    //
    public Dick(long belongings, string nickName, int gender, double length)
    {
        Belongings = belongings;
        NickName = nickName;
        Gender = (Gender)gender;
        Length = length;
    }

    public long Belongings { get; private set; } //牛子所属的QQ号

    public string NickName { get; set; }

    public Gender Gender { get; set; }

    public double Length { get; set; }
    
    
}