using System.Globalization;

namespace KuaforRandevu.Services;

public static class SalonFormat
{
    private static readonly string[] Months =
    [
        "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
        "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    ];

    public static string Money(decimal value)
    {
        var rounded = decimal.ToInt64(decimal.Round(value, 0, MidpointRounding.AwayFromZero));
        return $"{Group(rounded)} ₺";
    }

    public static string Duration(int minutes)
    {
        if (minutes <= 0)
            return "0 dk";
        if (minutes < 60)
            return $"{minutes} dk";

        var hours = minutes / 60;
        var rest = minutes % 60;
        return rest == 0 ? $"{hours} sa" : $"{hours} sa {rest} dk";
    }

    public static string Date(DateOnly date) => $"{date.Day} {Months[date.Month - 1]} {date.Year}";

    public static string Month(DateOnly date) => $"{Months[date.Month - 1]} {date.Year}";

    public static string Clock(TimeOnly time) => $"{time.Hour:00}:{time.Minute:00}";

    public static string Clock(DateTime time) => $"{time.Hour:00}:{time.Minute:00}";

    private static string Group(long value)
    {
        var digits = Math.Abs(value).ToString(CultureInfo.InvariantCulture);
        var parts = new List<string>();
        for (var index = digits.Length; index > 0; index -= 3)
        {
            var start = Math.Max(0, index - 3);
            parts.Add(digits[start..index]);
        }

        parts.Reverse();
        var grouped = string.Join('.', parts);
        return value < 0 ? $"-{grouped}" : grouped;
    }
}
