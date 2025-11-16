using Db.Records;

namespace Models;

public record UserProfileModel(
  string Id,
  string UserName,
  string GroupName,
  string FullName,
  Guid? SelectedSiteId,
  Guid? OwnSiteId,
  string UserId = ""
);
