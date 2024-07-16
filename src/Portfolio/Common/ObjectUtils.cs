using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfolio.Common;

public static class ObjectUtils {
  #region JSON
  public static string Encode(this object value, bool pretty = false) { serializerOptions.WriteIndented = pretty; return JsonSerializer.Serialize(value, serializerOptions); }
  public static T Decode<T>(this string value) => JsonSerializer.Deserialize<T>(value, serializerOptions);
  public static T Transform<T>(this object value) => value.Encode().Decode<T>();

  private static readonly JsonSerializerOptions serializerOptions = new() { NumberHandling = JsonNumberHandling.AllowReadingFromString };
  #endregion
}