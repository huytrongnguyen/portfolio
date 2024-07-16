using Portfolio.Common;
using Portfolio.Core;

namespace Portfolio.Config;

public class AuthMiddleware(RequestDelegate _next) {
  public async Task Invoke(HttpContext context, AuthService authService, UserService userService) {
    var token = context.Request.GetAuthToken();

    if (token != null) {
      try {
        // attach user to context on successful jwt validation
        var authUser = authService.GetAuthUserFromToken(token);
        context.Items["User"] = authUser;

        if (authUser != null && !authUser.Username.IsEmpty()) {
          authUser.IsAdmin = userService.IsAdmin(authUser?.Username);
        }

      } catch {
        // do nothing if jwt validation fails
        // user is not attached to context so request won't have access to secure routes
      }
    }

    await next(context);
  }

  private readonly RequestDelegate next = _next;
}