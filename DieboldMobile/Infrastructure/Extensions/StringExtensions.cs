using System;

public static class StringExtensions
{
    public static bool IsNumber(this String str)
    {
        double temp;
        return double.TryParse(str, out temp);
    }

    public static bool IsInteger(this String str)
    {
        int temp;
        return int.TryParse(str, out temp);
    }

    public static String RemoveSpaces(this String str)
    {
        return str.Replace(" ", "");
    }

    public static String SplitByUpperCase(this String str)
    {
        return SplitbyUpperCase(str);
    }

    public static String GetFirstUpperCaseRange(this String str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        if (str.ToUpper() == str)
            return str.Replace(" ", "");

        var strProcessed = SplitbyUpperCase(str);

        return strProcessed.Split(' ')[0];
    }

    private static String SplitbyUpperCase(this String str)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in str)
        {
            if (Char.IsUpper(c))
                sb.Append(' ');
            sb.Append(c);
        }

        return sb.ToString().Trim();
    }
}