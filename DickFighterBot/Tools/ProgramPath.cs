namespace DickFighterBot.Tools;

public class ProgramPath
{
    private static string? _path;

    public static string LocalProgramPath
    {
        get
        {
            if (_path != null) return _path;
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DickFighterBot");
            return _path;
        }
    }
}