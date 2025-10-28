using Errors;
using Models;

namespace Services;

public interface IWebSitesService
{
  Task<(WebSiteModel[]? articles, ServiceError? err)> ListPublicWebSites();
  Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int from = 0, int limit = 20);
  Task<(WebSiteModel? webSite, ServiceError? err)> UpdateWebSiteById(Guid siteId, WebSiteModel webSite);
  Task<(WebSiteModel? webSite, ServiceError? err)> SelectWebSite(Guid siteId, string securityGroupId);
  Task<(WebSiteModel? webSite, ServiceError? err)> GetWebSiteById(Guid siteId);
  Task<(WebSiteModel? webSite, ServiceError? err)> SetParent(Guid siteId, Guid? parentId);

}
