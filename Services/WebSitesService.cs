using Db;
using Errors;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

using static Consts.Consts;

public class WebSitesService : IWebSitesService
{
  private readonly ILogger logger;
  private readonly MongoDbContext dbContext;

  public WebSitesService(MongoDbContext dbContext, ILogger<WebSitesService> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }
  public async Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int skip = 0, int limit = 20)
  {
    await Task.Delay(1);

    try
    {
      var query = from sites in dbContext.WebSites.AsEnumerable()
                  join parent in dbContext.WebSites on sites.ParentId equals parent.Id into parents
                  from sub in parents.DefaultIfEmpty()
                  orderby sites.CreatedAt descending
                  select sites;

      var webSites = query.Skip(skip).Take(limit).Select(s => s.ToModel()).ToArray();

      return (webSites, null);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error, listing webSites");
      return (null, new(Message: ex.Message));
    }
  }

  public async Task<(WebSiteModel? webSite, ServiceError? err)> SelectWebSite(Guid siteId, string securityGroupId)
  {
    try
    {
      var site = await dbContext.WebSites.FindAsync(siteId);
      var securityGroup = await dbContext.SecurityGroups.FindAsync(MongoDB.Bson.ObjectId.Parse(securityGroupId));
      if (securityGroup is null)
      {
        return (null, new(Message: $"Security group with id: {securityGroupId} doesn't exist"));
      }

      securityGroup.SelectedSiteId = siteId;
      await dbContext.SaveChangesAsync();

      if (site is null)
      {
        return (null, new(Message: $"Site with id: {siteId} doesn't exist"));
      }

      return (site.ToModel(), null);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error, selecting webSite");
      return (null, new(Message: ex.Message));
    }
  }
}
