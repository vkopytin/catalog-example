using Models;
using Errors;
using Db.Records;

namespace Services;

public interface IProfileService
{
  Task<(AuthClient[]?, ProfileError?)> ListClients(int from = 0, int limit = 10);
  Task<(AuthClient?, ProfileError?)> GetClient(string clientId);
  Task<(AuthClient?, ProfileError?)> AddClient(AuthClient client);
  Task<(AuthClient?, ProfileError?)> SaveClient(AuthClient client);

  Task<(AuthUser[]?, ProfileError?)> ListUsers(int from = 0, int limit = 10);
  Task<(AuthUser?, ProfileError?)> GetUser(string userId);
  Task<(AuthUser?, ProfileError?)> AddUser(AuthUser user);
  Task<(AuthUser?, ProfileError?)> SaveUser(AuthUser user);

  Task<(WebSiteModel?, ProfileError?)> GetUserWebSite(string securityGroupId);
  Task<(SecurityGroupRecord?, ProfileError?)> GetProfileBySecurityGroupId(string securityGroupId);
  Task<(SecurityGroupRecord?, ProfileError?)> GetPublicProfile();
  Task<(WebSiteArticleRecord?, ProfileError?)> PublishArticleToWebSite(ArticleModel article, WebSiteModel webSite);
}
