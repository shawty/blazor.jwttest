using System.Net.Http;

namespace blazor.jwttest.Client.Exceptions
{
  public class HttpUnauthorizedException : HttpRequestException
  {
    public HttpUnauthorizedException() : base("401 (Unauthorized)") { }
  }
}
