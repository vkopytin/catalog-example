using Errors;
using Models;

namespace Services;

public interface IWebSitesService
{
  Task<(WebSiteModel[]? articles, ServiceError? err)> ListPublicWebSites();
  Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int from = 0, int limit = 20);
  Task<(WebSiteModel? webSite, ServiceError? err)> SelectWebSite(Guid siteId, string securityGroupId);
}
