using System.Globalization;
using System.Linq;

public static class StringExtensions
{
    /// <summary>
    /// Use Linq to get the first character, capitalise it, then return the new string 
    /// with the capital letter
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string CapitaliseFirstLetter(this string s)
    {
        return s.First().ToString().ToUpper() + s.Substring(1);
    }

    /// <summary>
    /// Set our language to that of British English. (The one and only Kappa)
    /// </summary>
    static TextInfo textInfo = new CultureInfo("en-GB").TextInfo;

    /// <summary>
    /// Use ToTitleCase to capitalise each word in the string.
    /// (Word probably being defined as anything after a space, 'This Is A Word', 'Thiswouldprobablybeasingleword')
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string CapitaliseAllWords(this string s)
    {
        return textInfo.ToTitleCase(s);
    }
}
