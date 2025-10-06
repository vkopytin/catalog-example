using Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

public class AuthorizationTokensService
{
  private readonly MongoDbContext dbContext;

  public AuthorizationTokensService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(string? token, ServiceError? err)> GetAccessToken(string openId)
  {
    var tokenRecord = await dbContext.AuthTokens
      .OrderByDescending(a => a.CreatedAt)
      .FirstOrDefaultAsync(t => t.SecurityGroupId == openId);

    if (tokenRecord is null)
    {
      return (null, new ServiceError("Token not found"));
    }

    return (tokenRecord.AccessToken, null);
  }
}
