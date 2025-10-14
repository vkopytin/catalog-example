using Db;

namespace Db.Records;

public class SecurityGroupRecord : BaseEntity<int>
{
  public string GroupName { get; set; } = "";
  public Guid SelectedSiteId { get; set; } = Guid.Empty;
}
