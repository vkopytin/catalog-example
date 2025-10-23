using System.Security.Claims;

namespace Utils;

public static class GenericExtentions
{
  public static string TryGetOid(this ClaimsPrincipal user)
  {
    var oid = user.FindFirst("oid")?.Value;
    if (string.IsNullOrEmpty(oid))
    {
      throw new ArgumentException("User does not have 'oid' claim");
    }

    return oid;
  }

  public static (string result, string error) GetOid(this ClaimsPrincipal user)
  {
    var oid = user.FindFirst("oid")?.Value;
    if (string.IsNullOrEmpty(oid))
    {
      return (null, "User does not have 'oid' claim");
    }

    return (oid, null);
  }

  public static Task<(T? result, Y? error)> AsResult<T, Y>(this (T? result, Y? error) tuple)
  {
    return Task.FromResult(tuple);
  }
}
