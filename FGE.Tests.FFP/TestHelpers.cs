using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class TestHelpers
    {
        // This is needed because the modules that FGU exports include these escaped characters
        // but this exporter doesn't use them. So before we can test for equality between FGU's export
        // and this program's export, we have to replace these characters with their escaped equivalent.
        public static Dictionary<string, string> EscapedChars = new Dictionary<string, string>()
        {
            { "&#34;", "\""},
            { "&#160;", " " }
        };

        public static string ReplaceEscapedCharacters(string text)
        {
            foreach (var kvp in EscapedChars)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }
            return text;
        }
    }
}
