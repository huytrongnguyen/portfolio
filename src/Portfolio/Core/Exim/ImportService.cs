using System.Data;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Portfolio.Common;

namespace Portfolio.Core;

public class ImportService(MasterDbContext _dbContext, IConfiguration _configuration, ILogger<ImportService> _logger) {
  public IEnumerable<ImportResult> Import(IFormCollection requestForm) {
    var totalFiles = requestForm.Files.Count;
    if (totalFiles < 1) {
      throw new Exception("Please select a file to import");
    }

    var importList = requestForm.Files.Select(file => {
      if (file.Length <= 0) {
        return new ImportForm { ViolationRules = ["Input File is required"] };
      }

      var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
      var form = new ImportForm {
        FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
        Scenario = requestForm["scenario"].ToString().Trim(),
        SheetName = requestForm["sheet_name"].ToString().Trim()
      };
      if (form.SheetName.IsEmpty()) form.ViolationRules.Add("Sheet name is required");

      if (form.ViolationRules.Count > 0) return form;

      // copy file to server
      form.FilePath = Path.Combine(uploadFolder, form.FileName);
      using (var stream = new FileStream(form.FilePath, FileMode.Create)) { file.CopyTo(stream); }

      return form;
    });

    return importList.Select(importForm => {
      if (importForm.ViolationRules.Count > 0) {
        return new ImportResult {
          FileName = importForm.FileName,
          Status = "IN_ERROR",
          Message = string.Join(",", importForm.ViolationRules)
        };
      }

      try {
        ProcessFile(importForm);
        return new ImportResult { FileName = importForm.FileName, Status = "PROCESSED", Message = "" };
      } catch (Exception e) {
        logger.Error(e.Message, e);
        return new ImportResult { FileName = importForm.FileName, Status = "IN_ERROR", Message = e.Message };
      }
    });
  }

  private void ProcessFile(ImportForm importForm) {
    var workbook = FileUtils.LoadExcel(importForm.FilePath);

    switch (importForm.Scenario) {
      case "Daily Reporting": ExtractDailyReporting(workbook, importForm.SheetName); break;
    }
  }

  private void ExtractDailyReporting(DataSet workbook, string sheetName) {
    var tbl = workbook.GetSheet(sheetName) ?? throw new Exception($"Cannot find '{sheetName}' in workbook");

    var dailyReportingLayout = new DailyReportingLayout();
    (int headerRowIndex, int headerColIndex) = tbl.FindRoot(dailyReportingLayout.Root);

    DateOnly? reportDate = null;

    for (var rowIndex = headerRowIndex + 2; rowIndex < tbl.Rows.Count; ++rowIndex) {
      var mediaSource = tbl.GetCellValue(rowIndex, headerColIndex - 1);
      if (mediaSource == "Total") {
        reportDate = tbl.GetCellValue(rowIndex, headerColIndex - 2).ParseDate();
        continue;
      }

      if (reportDate == null) continue;

      var lineItem = new DailyUserMetric { MediaSource = mediaSource, ReportDate = reportDate.Value };

      for (var colIndex = headerColIndex; colIndex < tbl.Columns.Count; ++ colIndex) {
        var header = tbl.GetCellValue(headerRowIndex, colIndex);
        var value = tbl.GetCellValue(rowIndex, colIndex);
        dailyReportingLayout.Process(header, value, lineItem);
      }

      dbContext.Set<DailyUserMetric>().AddOrUpdate(
        lineItem,
        onConflict: x => x.ReportDate == lineItem.ReportDate && x.MediaSource == lineItem.MediaSource,
        doUpdate: x => {
          x.Cost = lineItem.Cost;
          x.Impressions = lineItem.Impressions;
          x.Clicks = lineItem.Clicks;
          x.Installs = lineItem.Installs;
          x.Nru = lineItem.Nru;
          x.Dau = lineItem.Dau;
          x.Pu = lineItem.Pu;
          x.Npu = lineItem.Npu;
          x.Rev = lineItem.Rev;
          x.Revnpu = lineItem.Revnpu;
          x.Ruser01 = lineItem.Ruser01;
          x.Ruser03 = lineItem.Ruser03;
          x.Ruser07 = lineItem.Ruser07;
          x.Ruser14 = lineItem.Ruser14;
          x.Ruser21 = lineItem.Ruser21;
          x.Ruser30 = lineItem.Ruser30;
          x.Ruser60 = lineItem.Ruser60;
          x.Ruser90 = lineItem.Ruser90;
          x.Revrpi01 = lineItem.Revrpi01;
          x.Revrpi03 = lineItem.Revrpi03;
          x.Revrpi07 = lineItem.Revrpi07;
          x.Revrpi14 = lineItem.Revrpi14;
          x.Revrpi21 = lineItem.Revrpi21;
          x.Revrpi30 = lineItem.Revrpi30;
          x.Revrpi60 = lineItem.Revrpi60;
          x.Revrpi90 = lineItem.Revrpi90;
          x.Revrpi120 = lineItem.Revrpi120;
          x.Revrpi150 = lineItem.Revrpi150;
          x.Revrpi180 = lineItem.Revrpi180;
          x.Revnru01 = lineItem.Revnru01;
          x.Revnru03 = lineItem.Revnru03;
          x.Revnru07 = lineItem.Revnru07;
          x.Revnru14 = lineItem.Revnru14;
          x.Revnru21 = lineItem.Revnru21;
          x.Revnru30 = lineItem.Revnru30;
          x.Revnru60 = lineItem.Revnru60;
          x.Revnru90 = lineItem.Revnru90;
          x.Revnru120 = lineItem.Revnru120;
          x.Revnru150 = lineItem.Revnru150;
          x.Revnru180 = lineItem.Revnru180;
        }
      );
    }

    dbContext.SaveChanges();
  }

  private static string GetFileName(IFormFile file) => ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

  private static string GetSheetName(IFormCollection form, string enabled, string sheetName) {
   return form[enabled] == "on" && !form[sheetName].ToString().IsEmpty() ? form[sheetName].ToString().Trim() : null;
  }

  private readonly string uploadFolder = _configuration["UPLOAD_FOLDER"];
  private readonly ILogger logger = _logger;
  private readonly MasterDbContext dbContext = _dbContext;
}

public class ImportForm {
  public string FileName { get; set; }
  public string FilePath { get; set; }
  public string SheetName { get; set; }
  public string Scenario { get; set; }
  public List<string> ViolationRules { get; set; } = [];
}

public class ImportResult {
  public string FileName { get; set; }
  public string Status { get; set; }
  public string Message { get; set; }
}

public class DailyReportingLayout : ExcelFileLayout<DailyUserMetric> {
  public override string Root { get; } = "Cost";
  public override Dictionary<string, Action<string, DailyUserMetric>> Columns { get; } = new() {
    { "Cost", (value, item) => { item.Cost = value.ParseDouble(); } },
    { "Impressions", (value, item) => { item.Impressions = value.ParseLong(); } },
    { "Clicks", (value, item) => { item.Clicks = value.ParseLong(); } },
    { "Installs", (value, item) => { item.Installs = value.ParseLong(); } },
    { "NRU", (value, item) => { item.Nru = value.ParseLong(); } },
    { "DAU", (value, item) => { item.Dau = value.ParseLong(); } },
    { "PU", (value, item) => { item.Pu = value.ParseLong(); } },
    { "NPU", (value, item) => { item.Npu = value.ParseLong(); } },
    { "Revenue", (value, item) => { item.Rev = value.ParseDouble(); } },
    { "RevNPU", (value, item) => { item.Revnpu = value.ParseDouble(); } },
    { "Ruser01", (value, item) => { item.Ruser01 = value.ParseLong(); } },
    { "Ruser03", (value, item) => { item.Ruser03 = value.ParseLong(); } },
    { "Ruser07", (value, item) => { item.Ruser07 = value.ParseLong(); } },
    { "Ruser14", (value, item) => { item.Ruser14 = value.ParseLong(); } },
    { "Ruser21", (value, item) => { item.Ruser21 = value.ParseLong(); } },
    { "Ruser30", (value, item) => { item.Ruser30 = value.ParseLong(); } },
    { "Ruser60", (value, item) => { item.Ruser60 = value.ParseLong(); } },
    { "Ruser90", (value, item) => { item.Ruser90 = value.ParseLong(); } },
    { "RevRPI01", (value, item) => { item.Revrpi01 = value.ParseDouble(); } },
    { "RevRPI03", (value, item) => { item.Revrpi03 = value.ParseDouble(); } },
    { "RevRPI07", (value, item) => { item.Revrpi07 = value.ParseDouble(); } },
    { "RevRPI14", (value, item) => { item.Revrpi14 = value.ParseDouble(); } },
    { "RevRPI21", (value, item) => { item.Revrpi21 = value.ParseDouble(); } },
    { "RevRPI30", (value, item) => { item.Revrpi30 = value.ParseDouble(); } },
    { "RevRPI60", (value, item) => { item.Revrpi60 = value.ParseDouble(); } },
    { "RevRPI90", (value, item) => { item.Revrpi90 = value.ParseDouble(); } },
    { "RevRPI120", (value, item) => { item.Revrpi120 = value.ParseDouble(); } },
    { "RevRPI150", (value, item) => { item.Revrpi150 = value.ParseDouble(); } },
    { "RevRPI180", (value, item) => { item.Revrpi180 = value.ParseDouble(); } },
    { "RevNRU01", (value, item) => { item.Revnru01 = value.ParseDouble(); } },
    { "RevNRU03", (value, item) => { item.Revnru03 = value.ParseDouble(); } },
    { "RevNRU07", (value, item) => { item.Revnru07 = value.ParseDouble(); } },
    { "RevNRU14", (value, item) => { item.Revnru14 = value.ParseDouble(); } },
    { "RevNRU21", (value, item) => { item.Revnru21 = value.ParseDouble(); } },
    { "RevNRU30", (value, item) => { item.Revnru30 = value.ParseDouble(); } },
    { "RevNRU60", (value, item) => { item.Revnru60 = value.ParseDouble(); } },
    { "RevNRU90", (value, item) => { item.Revnru90 = value.ParseDouble(); } },
    { "RevNRU120", (value, item) => { item.Revnru120 = value.ParseDouble(); } },
    { "RevNRU150", (value, item) => { item.Revnru150 = value.ParseDouble(); } },
    { "RevNRU180", (value, item) => { item.Revnru180 = value.ParseDouble(); } },
  };
}