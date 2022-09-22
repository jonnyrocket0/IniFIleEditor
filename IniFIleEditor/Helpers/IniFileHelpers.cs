using IniFIleEditor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IniFIleEditor.Helpers
{
    public static class IniFileHelpers
    {
        public static LineType DetermineLineType(this string lineTypeStr)
        {
            return LineType.Debuffs;
        }

        public static string Cleanse (this string inputStr)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string returnStr;
            returnStr = inputStr.Replace(";", "").Replace("(On/Off)", "Toggle").Replace(" (Melee/Ranged/Off)", "").Replace(" (Char to Anchor to)", "").Replace("-", "");

            var dontTitleCase = new string[] { "CureAll", "AutoRadiant Toggle", "RadiantCure" };
            if (dontTitleCase.Contains(returnStr))
                return returnStr.Replace(" ", "");

            return textInfo.ToTitleCase(returnStr).Replace(" ", "");
        }
    }
}
