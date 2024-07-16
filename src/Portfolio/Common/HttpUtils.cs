namespace Portfolio.Common;

public static class HttpUtils {
  public static string GetAuthToken(this HttpRequest request) => request.Headers["x-auth-token"].FirstOrDefault();
}