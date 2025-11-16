using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Db.Records;

public class UserRecord
{
  [Key]
  public ObjectId Id { get; set; }

  public string? UserName { get; set; }
  public string? Name { get; set; }
  public string? Role { get; set; }
  public bool IsActive { get; set; }
  public string? Password { get; set; }
  public MongoDB.Bson.ObjectId? SecurityGroupId { get; set; }
  [ForeignKey("SecurityGroupId")]
  public SecurityGroupRecord? SecurityGroup { get; set; }
}
