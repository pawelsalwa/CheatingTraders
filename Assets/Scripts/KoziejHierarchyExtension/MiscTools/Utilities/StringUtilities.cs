using System;
using System.Collections.Generic;

namespace AWI
{
    public static class StringUtilities
    {
        public static string CleanCopiedName(string name)
        {
            var split = name.Split('(');
            name = split[0];
            while (name[name.Length - 1] == ' ')
            {
                name = name.Remove(name.Length-1);
            }
            return name;
        }
    }
}