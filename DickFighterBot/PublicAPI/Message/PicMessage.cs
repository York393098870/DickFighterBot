namespace DickFighterBot.PublicAPI.Message;

public class PicMessage
{
    public static string GeneratePicMessage(string base64Pic)
    {
        var outputMessage = $"[CQ:image,file=base64://{base64Pic}]";
        return outputMessage;
    }
}