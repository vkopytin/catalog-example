namespace Db.Records;

public class AuthorizationTokenRecord : BaseEntity<Guid>
{
  public string AccessToken { get; set; }
  public string RefreshToken { get; set; }
  public DateTime Expiration { get; set; }
  public string SecurityGroupId { get; set; }
  public string[] Scopes { get; set; } = [];
  public string TokenType { get; set; } = "Bearer";
}
