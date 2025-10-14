using System.ComponentModel.DataAnnotations;

namespace Db.Records;

public class SecurityGroupRecord
{
  [Key]
  public MongoDB.Bson.ObjectId Id { get; set; }
  public string GroupName { get; set; } = "";
  public Guid? SelectedSiteId { get; set; } = null;
}
