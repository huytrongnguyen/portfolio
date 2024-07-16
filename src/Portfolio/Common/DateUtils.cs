namespace Portfolio.Common;

public static class DateUtils {
  public static DateOnly ToDateOnly(this DateTime dt) => DateOnly.FromDateTime(dt);
  public static DateTime Now() => DateTime.UtcNow;
}