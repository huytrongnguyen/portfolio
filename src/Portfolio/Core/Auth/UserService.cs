using Portfolio.Common;

namespace Portfolio.Core;

public class UserService {
  public bool IsAdmin(string username) {
    if (username.IsEmpty()) return false;

    return true;
  }
}