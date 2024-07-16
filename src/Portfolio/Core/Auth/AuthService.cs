using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Common;

namespace Portfolio.Core;

public class AuthService(ILogger<AuthService> _logger) {
  public AuthUser GetAuthUserFromToken(string token) {
    if (token.IsEmpty()) return null;

    try {
      var tokenHandler = new JwtSecurityTokenHandler();
      tokenHandler.ValidateToken(token, new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
          ValidateIssuer = false,
          ValidateAudience = false,
          // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
          ClockSkew = TimeSpan.Zero
      }, out SecurityToken validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var user = jwtToken.Claims.ToDictionary(x => x.Type, x => x.Value).Transform<AuthUser>();
      return user;
    } catch (Exception e) {
      logger.Error(e.Message);
      return null;
    }

  }

  private static readonly string SecretKey = "VersaillesContinent".ToSha512();
  private readonly ILogger logger = _logger;
}

public class AuthUser {
  public string Username { get; set; }
  public string Token { get; set; }
  public bool IsAdmin { get; set; }
}