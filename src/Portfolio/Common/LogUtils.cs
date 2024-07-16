namespace Portfolio.Common;

public static class LogUtils {
  public static void Info(this ILogger logger, string message) => logger.LogInformation("{message}", message);
  public static void Warning(this ILogger logger, string message) => logger.LogWarning("{message}", message);
  public static void Error(this ILogger logger, string message, Exception? exception = null) => logger.LogError(exception, "{message}", message);
}