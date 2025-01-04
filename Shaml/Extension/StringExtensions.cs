using Shaml.Tokens;

namespace Shaml.Extension;

public static class StringExtensions
{
    public static string Substring(this string str, Mark mark)
    {
        return str.Substring(mark.Start, mark.Length);
    }
}