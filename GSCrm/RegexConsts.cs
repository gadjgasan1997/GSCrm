using System.Text.RegularExpressions;

namespace GSCrm
{
    public static class RegexConsts
    {
        public static readonly Regex ONLY_DIGITS = new Regex("[^0-9]", RegexOptions.Compiled);
        public static readonly Regex LATIN_LETTERS_AND_DIGITS = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
    }
}
