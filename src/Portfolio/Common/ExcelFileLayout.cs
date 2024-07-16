namespace Portfolio.Common;

public class ExcelFileLayout<T> {
  public void Process(string header, string value, T item) {
    if (!Columns.TryGetValue(header, out Action<string, T>? extractFunc)) return;
    extractFunc(value, item);
  }

  public virtual string Root { get; }
  public virtual Dictionary<string, Action<string, T>> Columns { get; } = [];
}