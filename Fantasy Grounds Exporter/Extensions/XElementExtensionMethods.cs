using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Extensions
{
    public static class XElementExtensionMethods
    {
        public static void SortElements(this XElement root)
        {
            if (root == null) return;
            var sorted = root.Elements().OrderBy(e => e.Name.ToString()).ToList();
            root.RemoveNodes();
            root.Add(sorted);
        }
    }
}
