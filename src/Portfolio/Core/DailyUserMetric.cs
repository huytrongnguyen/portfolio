using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core;

[Table("daily_user_metric", Schema = "exmgr")]
public class DailyUserMetric {
  [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
  public DateOnly ReportDate { get; set; }
  public string MediaSource { get; set; }
  public double Cost { get; set; }
  public long Impressions { get; set; }
  public long Clicks { get; set; }
  public long Installs { get; set; }
  public long Nru { get; set; }
  public long Dau { get; set; }
  public long Pu { get; set; }
  public long Npu { get; set; }
  public double Rev { get; set; }
  public double Revnpu { get; set; }
  public long Ruser01 { get; set; }
  public long Ruser03 { get; set; }
  public long Ruser07 { get; set; }
  public long Ruser14 { get; set; }
  public long Ruser21 { get; set; }
  public long Ruser30 { get; set; }
  public long Ruser60 { get; set; }
  public long Ruser90 { get; set; }
  public double Revrpi01 { get; set; }
  public double Revrpi03 { get; set; }
  public double Revrpi07 { get; set; }
  public double Revrpi14 { get; set; }
  public double Revrpi21 { get; set; }
  public double Revrpi30 { get; set; }
  public double Revrpi60 { get; set; }
  public double Revrpi90 { get; set; }
  public double Revrpi120 { get; set; }
  public double Revrpi150 { get; set; }
  public double Revrpi180 { get; set; }
  public double Revnru01 { get; set; }
  public double Revnru03 { get; set; }
  public double Revnru07 { get; set; }
  public double Revnru14 { get; set; }
  public double Revnru21 { get; set; }
  public double Revnru30 { get; set; }
  public double Revnru60 { get; set; }
  public double Revnru90 { get; set; }
  public double Revnru120 { get; set; }
  public double Revnru150 { get; set; }
  public double Revnru180 { get; set; }
}