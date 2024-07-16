using System.Data;
using ExcelDataReader;

namespace Portfolio.Common;

public static class FileUtils {
  public static DataSet LoadExcel(string path, bool deleteFileAfterLoad = true) {


    DataSet ds;
    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
      ds = ExcelReaderFactory.CreateReader(stream).AsDataSet();
    }
    if (deleteFileAfterLoad) File.Delete(path);

    if (ds == null) {
      throw new Exception($"Cannot load '{path.Split("/").Last()}'");
    }

    return ds;
  }
}