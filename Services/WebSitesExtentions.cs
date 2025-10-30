using Db.Records;
using Models;

namespace Services;

public static class WebSitesExtentions
{
  public static WebSiteModel ToModel(this WebSiteRecord webSite, int index = 0)
  {
    return new(
      Id: webSite.Id,
      ParentId: webSite.ParentId,
      CreatedAt: webSite.CreatedAt,
      Name: webSite.Name ?? string.Empty,
      HostName: webSite.HostName ?? string.Empty,
      AltHostName: webSite.AltHostName ?? string.Empty,
      Parent: index < 5 ? webSite.Parent?.ToModel(index + 1) : null
    );
  }
}
