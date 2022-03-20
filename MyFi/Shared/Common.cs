using System;
using System.Globalization;

namespace MyFi.Shared;
public static class Common
{
    public static string FormatUSD(object amount) {
        if (amount == null)
            return string.Empty;
        return string.Format(new CultureInfo("en-SG", false), "{0:c2}", amount);
    }
}

