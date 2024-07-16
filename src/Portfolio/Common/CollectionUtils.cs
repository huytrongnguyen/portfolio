using System.Data;

namespace Portfolio.Common;

public static class CollectionUtils {
  public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action) {
    foreach(var (item, index) in collection.Select((item, index) => (item, index))) {
      action(item, index);
    }
  }

  #region ===== DataSet =====
  public static DataTable GetSheet(this DataSet workbook, string sheetName) {
    if (sheetName.IsEmpty()) return null;

    foreach (DataTable tbl in workbook.Tables) {
      if (tbl.TableName.Trim() == sheetName.Trim()) {
        return tbl;
      }
    }

    return null;
  }

  public static string GetCellValue(this DataTable workbook, int rowIndex, int colIndex) {
    if (rowIndex >= workbook.Rows.Count || colIndex >= workbook.Columns.Count) return "";
    return workbook.Rows[rowIndex][colIndex]?.ToString()?.Trim() ?? "";
  }

  public static (int, int) FindRoot(this DataTable tbl, string cellValue) {
    for (var rowIndex = 0; rowIndex < tbl.Rows.Count; ++rowIndex) {
      for (var colIndex = 0; colIndex < tbl.Columns.Count; ++colIndex) {
        if (cellValue == tbl.GetCellValue(rowIndex, colIndex)) return (rowIndex, colIndex);
      }
    }

    return (-1, -1);
  }

  public static List<T> Extract<T>(this DataTable tbl, ExcelFileLayout<T> layout) where T : new() {
    (int headerRowIndex, int headerColIndex) = (0, 0);

    var list = new List<T>();

    for (var rowIndex = headerRowIndex + 1; rowIndex < tbl.Rows.Count; ++rowIndex) {
      var lineItem = new T();
      for (var colIndex = headerColIndex; colIndex < tbl.Columns.Count; ++ colIndex) {
        var header = tbl.GetCellValue(headerRowIndex, colIndex);
        var value = tbl.GetCellValue(rowIndex, colIndex);
        layout.Process(header, value, lineItem);
      }
      list.Add(lineItem);
    }

    return list;
  }
  #endregion
}