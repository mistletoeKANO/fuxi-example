using FuXi.Editor;

[PlayerNamePriority(-1)]
public class CustomPlayerName : IPlayerNameDefine
{
    public string GetPlayerName(string version)
    {
        return "Custom";
    }
}