using Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

public class WebSitesService : IWebSitesService
{
  private readonly ILogger logger;
  private readonly MongoDbContext dbContext;

  public WebSitesService(MongoDbContext dbContext, ILogger<WebSitesService> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }
  public Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int from = 0, int limit = 20)
  {
    try
    {
      var query = from sites in dbContext.WebSites.AsEnumerable()
                  join parent in dbContext.WebSites on sites.ParentId equals parent.Id into parents
                  from sub in parents.DefaultIfEmpty()
                  orderby sites.CreatedAt descending
                  select sites;

      var webSites = query.Skip(from).Take(limit).ToArray();

      return Task.FromResult<(WebSiteModel[]? articles, ServiceError? err)>(
        (webSites.Select(s => s.ToModel()).ToArray(), null)
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error, listing webSites");
      return Task.FromResult<(WebSiteModel[]? articles, ServiceError? err)>(
        (null, new(Message: ex.Message))
      );
    }
  }
}
