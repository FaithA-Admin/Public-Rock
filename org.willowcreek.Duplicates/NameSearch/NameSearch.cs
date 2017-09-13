using System;
using System.Collections.Generic;
using System.Linq;

namespace org.willowcreek.Duplicates.NameSearch
{
    public class NameSearch
    {
        public static List<String> GetOtherFormsOfNames(String nameToCheck)
        {
            List<string> returnValue = new List<string>();
            foreach (String[] nameset in Names.names.Where(b => b.CaseInsensitiveContains(nameToCheck)))
            {
                returnValue.AddRange(nameset);
            }
            return returnValue;
        }

        public static bool AreTheSameName(string baseName, string nameToCheck)
        {
            if (GetOtherFormsOfNames(baseName).CaseInsensitiveContains(nameToCheck))
                return true;
            return false;
        }
    }

    internal static class CaseInsensitiveContainsExtension
    {
        public static bool CaseInsensitiveContains(this IEnumerable<String> source, string value)
        {
            foreach (String s in source)
            {
                if (s.ToUpper().Equals(value.ToUpper()))
                    return true;
            }
            return false;
        }
    }
}
