using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Common;

public static class StringUtils {
  public static bool IsEmpty(this string str) => string.IsNullOrWhiteSpace(str);
  public static string DefaultIfEmpty(this string str, string defaultStr) => !str.IsEmpty() ? str : defaultStr;
  public static bool EqualsIgnoreCase(this string str, string other) => string.Equals(str.Trim(), other.Trim(), StringComparison.OrdinalIgnoreCase);

  public static int ParseInt(this string str) => (int)str.ParseDouble();
  public static long ParseLong(this string str) => (long)str.ParseDouble();
  public static double ParseDouble(this string str) => double.TryParse(str, out double value) ? value : 0;
  public static DateTime ParseDateTime(this string str) => DateTime.TryParse(str, out DateTime value) ? value : default;
  public static DateOnly ParseDate(this string str) => str.ParseDateTime().ToDateOnly();
  public static bool ParseBool(this string str) => str?.ToUpper() == "TRUE" || str?.ToUpper() == "YES";
  public static string[] ParseList(this string str, string separator) => str.IsEmpty() ? [] : str.Split(separator);
  public static T ParseEnum<T>(this string str) where T : struct, Enum { return (T) Enum.Parse(typeof(T), str, true); }

  public static string ToSha512(this string str) => str.IsEmpty() ? "" : Encoding.UTF8.GetString(SHA512.HashData(Encoding.UTF8.GetBytes(str)));
}