using Db;
using Models;
using Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using Db.Records;
using Utils;

namespace Services;

public class ProfileService : IProfileService
{
  private readonly MongoDbContext dbContext;
  private readonly ILogger logger;

  public ProfileService(MongoDbContext dbContext, ILogger<ProfileService> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  public async Task<(AuthClient[]?, ProfileError?)> ListClients(int from = 0, int limit = 10)
  {
    var clients = await dbContext.AuthClients
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (clients.Select(c => c.ToModel()).ToArray(), null);
  }

  public async Task<(AuthClient?, ProfileError?)> GetClient(string clientId)
  {
    try
    {
      var client = await dbContext.AuthClients.Where(c => c.ClientId == clientId)
        .Take(1)
        .FirstOrDefaultAsync();

      if (client is null)
      {
        return (null, new(Message: $"Client with id: {clientId} doesn't exist"));
      }

      return (client.ToModel(), null);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while fetching clients from DB");
      return (null, new(Message: ex.Message));
    }
  }

  public async Task<(AuthClient?, ProfileError?)> AddClient(AuthClient client)
  {
    var exists = await dbContext.AuthClients.AnyAsync(c => c.ClientId == client.ClientId);

    if (exists)
    {
      return (null, new(Message: "Can't create. Client with same id exists"));
    }

    await dbContext.AuthClients.AddAsync(client.ToDataModel());

    await dbContext.SaveChangesAsync();

    return (client, null);
  }

  public async Task<(AuthClient?, ProfileError?)> SaveClient(AuthClient client)
  {
    var existingClient = await dbContext.AuthClients.Where(c => c.ClientId == client.ClientId)
      .Take(1)
      .FirstOrDefaultAsync();

    if (existingClient is null)
    {
      return (null, new(Message: "Can't update client. Client doesn't exist."));
    }

    existingClient.ClientId = client.ClientId;
    existingClient.ClientName = client.ClientName;
    existingClient.ClientSecret = client.ClientSecret;
    existingClient.GrantType = client.GrantType;
    existingClient.AllowedScopes = client.AllowedScopes;
    existingClient.ClientUri = client.ClientUri;
    existingClient.RedirectUri = client.RedirectUri;
    existingClient.IsActive = client.IsActive;

    await dbContext.SaveChangesAsync();

    return (existingClient.ToModel(), null);
  }

  public async Task<(AuthUser[]?, ProfileError?)> ListUsers(int from = 0, int limit = 10)
  {
    var users = await dbContext.Users
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (users.Select(c => c.ToModel()).ToArray(), null);
  }

  public async Task<(AuthUser?, ProfileError?)> GetUser(string userName)
  {
    var user = await dbContext.Users.Where(c => c.UserName == userName)
      .Take(1)
      .FirstOrDefaultAsync();

    if (user is null)
    {
      return (null, new(Message: $"User with username: {userName} doesn't exist"));
    }

    return (user.ToModel(), null);
  }

  public async Task<(AuthUser?, ProfileError?)> AddUser(AuthUser user)
  {
    var exists = await dbContext.Users.AnyAsync(c => c.UserName == user.UserName);

    if (exists)
    {
      return (null, new(Message: "Can't create. User with same username exists"));
    }

    await dbContext.Users.AddAsync(user.ToDataModel());

    await dbContext.SaveChangesAsync();

    return (user, null);
  }

  public async Task<(AuthUser?, ProfileError?)> SaveUser(AuthUser user)
  {
    var existingUser = await dbContext.Users.Where(c => c.UserName == user.UserName)
      .Take(1)
      .FirstOrDefaultAsync();

    if (existingUser is null)
    {
      return (null, new(Message: "Can't update user. User doesn't exist."));
    }

    existingUser.UserName = user.UserName;
    existingUser.Name = user.Name ?? string.Empty;
    existingUser.Role = user.Role;
    existingUser.IsActive = user.IsActive;

    await dbContext.SaveChangesAsync();

    return (existingUser.ToModel(), null);
  }

  public async Task<(WebSiteModel?, ProfileError?)> GetUserWebSite(string securityGroupId)
  {
    try
    {
      var securityGroup = await dbContext.SecurityGroups.FindAsync(MongoDB.Bson.ObjectId.Parse(securityGroupId));
      if (securityGroup is null)
      {
        return (null, new(Message: $"No security group found for the id: {securityGroupId}"));
      }

      var webSite = await dbContext.WebSites
        .Where(w => w.Id == securityGroup.SelectedSiteId)
        .Take(1)
        .FirstOrDefaultAsync();

      if (webSite is null)
      {
        return (null, new(Message: $"No website found for the security group id: {securityGroupId}"));
      }

      return (webSite.ToModel(), null);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while fetching website from DB");
      return (null, new(Message: ex.Message));
    }
  }

  public Task<(UserProfileModel?, ProfileError?)> GetPublicProfile()
  {
    try
    {
      var securityGroup = new UserProfileModel(
        Id: MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
        UserName: "Account",
        GroupName: "Public",
        FullName: string.Empty,
        SelectedSiteId: null
      );
      return (securityGroup, default(ProfileError?)).AsResult();
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while fetching public profile from DB");
      return (default(UserProfileModel?), new ProfileError(Message: ex.Message)).AsResult();
    }
  }

  public async Task<(UserProfileModel?, ProfileError?)> GetProfileBySecurityGroupId(string securityGroupId)
  {
    try
    {
      var securityGroup = await dbContext.SecurityGroups.FindAsync(MongoDB.Bson.ObjectId.Parse(securityGroupId));
      if (securityGroup is null)
      {
        return (null, new(Message: $"No security group found for the id: {securityGroupId}"));
      }

      var profile = await dbContext.Users.Where(u => u.UserName == securityGroup.GroupName)
        .Take(1)
        .FirstOrDefaultAsync();

      var userProfile = new UserProfileModel(
        Id: securityGroup.Id.ToString(),
        UserName: securityGroup.GroupName,
        GroupName: securityGroup.GroupName,
        FullName: profile?.Name ?? securityGroup.GroupName,
        SelectedSiteId: securityGroup.SelectedSiteId
      );

      return (userProfile, null);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while fetching website from DB");
      return (null, new(Message: ex.Message));
    }
  }

  public async Task<(WebSiteArticleRecord?, ProfileError?)> PublishArticleToWebSite(ArticleModel article, WebSiteModel webSite)
  {
    try
    {
      var site = await dbContext.WebSites.FindAsync(webSite.Id);
      if (site is null)
      {
        return (null, new(Message: $"No website found for the id: {webSite.Id}"));
      }

      var existing = await dbContext.WebSiteArticles.IgnoreQueryFilters().Where(a => a.WebSiteId == webSite.Id && a.ArticleId == article.Id)
        .Take(1)
        .FirstOrDefaultAsync();

      if (existing is null)
      {
        var newIntId = await this.dbContext.WebSiteArticles.Select(a => a.Id).MaxAsync() + 1;
        var siteArticle = new WebSiteArticleRecord
        {
          Id = newIntId,
          WebSiteId = webSite.Id,
          ArticleId = article.Id,
        };

        await dbContext.WebSiteArticles.AddAsync(siteArticle);

        return (siteArticle, null);
      }

      if (existing.DeletedAt is not null)
      {
        existing.DeletedAt = null;
        dbContext.WebSiteArticles.Update(existing);
      }

      await dbContext.SaveChangesAsync();

      return (existing, null);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while publishing article to website");
      return (null, new(Message: ex.Message));
    }
  }

  public async Task<(WebSiteArticleRecord?, ProfileError?)> UnpublishArticleFromWebSite(ArticleModel article, WebSiteModel webSite)
  {
    try
    {
      var site = await dbContext.WebSites.FindAsync(webSite.Id);
      if (site is null)
      {
        return (null, new(Message: $"No website found for the id: {webSite.Id}"));
      }

      var existing = await dbContext.WebSiteArticles.Where(a => a.WebSiteId == webSite.Id && a.ArticleId == article.Id)
        .Take(1)
        .FirstOrDefaultAsync();

      if (existing is not null)
      {
        existing.DeletedAt = DateTime.UtcNow;
        dbContext.WebSiteArticles.Update(existing);

        await dbContext.SaveChangesAsync();

        return (existing, null);
      }

      return (null, new(Message: "No published article found to unpublish."));
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while unpublishing article from website");
      return (null, new(Message: ex.Message));
    }
  }
}
