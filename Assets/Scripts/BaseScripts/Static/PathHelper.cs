public static class PathHelper
{
    public static string SwitchSlash(bool p_UseForwardSlash, string p_path)
    {
        if (p_UseForwardSlash)
        {
            return p_path = p_path.Replace(@"\", "/");
        }
        else
        {
            return p_path = p_path.Replace(@"/", @"\");
        }
    }
}
